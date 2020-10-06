using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using EventHubHostedServices.BuildingBlocks.Contracts;
using EventHubHostedServices.BuildingBlocks.Models;
using EventHubHostedServices.Host.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubHostedServices.Host.HostedServices
{
    public class EventHubTelemetryHostedService : IHostedService
    {
        private readonly IAlarmNotificationService _alarmNotificationService;
        private readonly ILogger<AlarmRuleCacheHostedService> _logger;
        private readonly EventProcessorClient _eventProcessorClient;
        private readonly ICachingService _cache;

        public EventHubTelemetryHostedService(
            IAlarmNotificationService alarmNotificationService,
            ILogger<AlarmRuleCacheHostedService> logger,
            IConfiguration configuration,
            ICachingService cache)
        {
            var blobContainerClient = new BlobContainerClient(
                configuration["BlobStorage:ConnectionString"],
                configuration["BlobStorage:ContainerName"]);

            this._eventProcessorClient = new EventProcessorClient(
                blobContainerClient,
                configuration["EventHub:ConsumerGroup"],
                configuration["EventHub:EventHubNamespaceConnectionString"],
                configuration["EventHub:EventHubName"]);

            this._eventProcessorClient.PartitionInitializingAsync += PartitionInitializingAsync;
            this._eventProcessorClient.ProcessEventAsync += this.ProcessEventAsync;
            this._eventProcessorClient.ProcessErrorAsync += this.ProcessErrorAsync;

            this._alarmNotificationService = alarmNotificationService;
            this._logger = logger;
            this._cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"[{DateTime.Now}] Starting {nameof(EventHubTelemetryHostedService)} hosted service");

            Task.Run(() => this._eventProcessorClient.StartProcessing(cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        private Task PartitionInitializingAsync(PartitionInitializingEventArgs arg)
        {
            arg.DefaultStartingPosition = EventPosition.Latest;
            return Task.CompletedTask;
        }

        public async Task ProcessEventAsync(ProcessEventArgs args)
        {
            var message = args.GetSerializedMessage();

            foreach (Reading reading in message.Readings)
            {
                if (this._cache.TryGetValue(reading.SensorId, out SensorAlarmRules sensorAlarmRules))
                {
                    // Get sensor alarm rule statuses against sensor reading
                    IEnumerable<AlarmRuleStatus> transientStatuses =
                        SensorAlarmRules.GetAlarmRuleStatuses(reading.SensorId, sensorAlarmRules.AlarmRules, reading.Value.Value);
                    // Get cached alarm rule statuses
                    IEnumerable<AlarmRuleStatus> persistedStatuses =
                        transientStatuses.Select(x => this._cache.GetValue<AlarmRuleStatus>(x.ToString()));

                    var changedStatuses = SensorAlarmRules.GetAlarmRuleStatusChanges(persistedStatuses, transientStatuses, this._logger);

                    if (changedStatuses.Count() > 0)
                    {
                        await this._alarmNotificationService
                            .SendAlarmRuleStatusNotificationsAsync(changedStatuses, reading.Value.Value, message.Timestamp, message.DeviceId);

                        foreach (var status in changedStatuses)
                        {
                            this._cache.SetValue(status.ToString(), status);
                        }
                    }
                }
            }
        }

        public Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            this._logger.LogError(args.Exception.Message);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._eventProcessorClient.StopProcessingAsync(cancellationToken);
            this._eventProcessorClient.ProcessEventAsync -= ProcessEventAsync;
            this._eventProcessorClient.ProcessErrorAsync -= ProcessErrorAsync;
        }
    }
}
