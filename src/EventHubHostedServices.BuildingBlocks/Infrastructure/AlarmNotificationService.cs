using EventHubHostedServices.BuildingBlocks.Contracts;
using EventHubHostedServices.BuildingBlocks.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHubHostedServices.BuildingBlocks.Infrastructure
{
    public class AlarmNotificationService : IAlarmNotificationService
    {
        private readonly ILogger<AlarmNotificationService> _logger; 
        private readonly ITopicClient _topicClient;

        public AlarmNotificationService(IConfiguration configuration, ILogger<AlarmNotificationService> logger)
        {
            this._topicClient = new TopicClient(configuration["ServiceBusConnectionString"], "alarm", RetryPolicy.Default);
            this._logger = logger;
        }

        public async Task SendAlarmRuleStatusNotificationsAsync(
            IEnumerable<AlarmRuleStatus> alarmRuleStatusNotifications, double readingValue, DateTime timestamp, int? deviceId)
        {
            var alarmRuleStatusNotification = alarmRuleStatusNotifications
                .Select(x => new
                {
                    timestamp,
                    deviceId,
                    readingValue,
                    alarmActive = x.AlarmActive,
                    alarmRuleId = x.AlarmRuleId,
                    sensorId = x.SensorId
                })
                .ToList();

            var serviceBusMessageJson = Utf8Json.JsonSerializer.Serialize(alarmRuleStatusNotification);

            var serviceBusMessage = new Microsoft.Azure.ServiceBus.Message(serviceBusMessageJson);

            await this._topicClient.SendAsync(serviceBusMessage);
        }
    }
}
