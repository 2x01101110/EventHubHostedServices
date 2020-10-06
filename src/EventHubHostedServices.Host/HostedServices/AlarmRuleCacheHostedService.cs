using EventHubHostedServices.BuildingBlocks.Contracts;
using EventHubHostedServices.BuildingBlocks.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubHostedServices.Host.HostedServices
{
    public class AlarmRuleCacheHostedService : IHostedService
    {
        private readonly IAlarmRuleRepository _alarmRuleRepository;
        private readonly ILogger<AlarmRuleCacheHostedService> _logger;
        private readonly ICachingService _cache;

        public AlarmRuleCacheHostedService(
            IAlarmRuleRepository alarmRuleRepository,
            ILogger<AlarmRuleCacheHostedService> logger,
            ICachingService cache)
        {
            this._alarmRuleRepository = alarmRuleRepository;
            this._logger = logger;
            this._cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"[{DateTime.Now}] Starting {nameof(AlarmRuleCacheHostedService)} hosted service");

            Task.Run(() => CachingProcess(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public async Task CachingProcess(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"[{DateTime.Now}] Starting {nameof(AlarmRuleCacheHostedService)} hosted service");

            this._logger.LogInformation($"[{DateTime.Now}] Refreshing sensor alarm rule statuses");

            foreach (var alarmRuleStatus in await this._alarmRuleRepository.GetSensorAlarmRuleStatuses())
            {
                if (!this._cache.TryGetValue(alarmRuleStatus.ToString(), out AlarmRuleStatus _))
                {
                    this._cache.SetValue(alarmRuleStatus.ToString(), alarmRuleStatus);
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                this._logger.LogInformation($"[{DateTime.Now}] Refreshing alarm rule cache");

                foreach (var sensor in await this._alarmRuleRepository.GetSensorAlarmRulesAsync())
                {
                    this._cache.SetValue(sensor.SensorId, sensor);
                }

                await Task.Delay(600000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"[{DateTime.Now}] Stopping {nameof(AlarmRuleCacheHostedService)} hosted service");

            return Task.CompletedTask;
        }
    }
}
