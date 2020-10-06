using EventHubHostedServices.BuildingBlocks.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHubHostedServices.BuildingBlocks.Contracts
{
    public interface IAlarmNotificationService
    {
        Task SendAlarmRuleStatusNotificationsAsync(
            IEnumerable<AlarmRuleStatus> alarmRuleStatusNotifications, double readingValue, DateTime timestamp, int? deviceId);
    }
}
