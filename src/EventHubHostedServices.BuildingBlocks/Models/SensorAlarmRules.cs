using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EventHubHostedServices.BuildingBlocks.Models
{
    public class SensorAlarmRules
    {
        public int SensorId { get; }
        public IEnumerable<AlarmRule> AlarmRules { get; }

        public SensorAlarmRules(
            int sensorId, 
            IEnumerable<AlarmRule> alarmRules)
        {
            this.SensorId = sensorId;
            this.AlarmRules = alarmRules;
        }

        public static IEnumerable<AlarmRuleStatus> GetAlarmRuleStatuses(
            int sensorId,
            IEnumerable<AlarmRule> alarmRules,
            double? readingValue)
        {
            var alarmRuleStatuses = new List<AlarmRuleStatus>();

            // Do something with null values later
            if (readingValue == null) return alarmRuleStatuses;

            foreach (var alarmRule in alarmRules)
            {
                foreach (var condition in alarmRule.Conditions)
                {
                    switch (condition.Operator)
                    {
                        case "==":
                            alarmRuleStatuses.Add(new AlarmRuleStatus(sensorId, alarmRule.AlarmRuleId, readingValue == condition.Value));
                            break;
                        case ">=":
                            alarmRuleStatuses.Add(new AlarmRuleStatus(sensorId, alarmRule.AlarmRuleId, readingValue >= condition.Value));
                            break;
                        case "<=":
                            alarmRuleStatuses.Add(new AlarmRuleStatus(sensorId, alarmRule.AlarmRuleId, readingValue <= condition.Value));
                            break;
                        case "<":
                            alarmRuleStatuses.Add(new AlarmRuleStatus(sensorId, alarmRule.AlarmRuleId, readingValue < condition.Value));
                            break;
                        case ">":
                            alarmRuleStatuses.Add(new AlarmRuleStatus(sensorId, alarmRule.AlarmRuleId, readingValue > condition.Value));
                            break;
                    }
                }
            }

            return alarmRuleStatuses;
        }

        public static IEnumerable<AlarmRuleStatus> GetAlarmRuleStatusChanges(
            IEnumerable<AlarmRuleStatus> persistedSource,
            IEnumerable<AlarmRuleStatus> transientSource,
            ILogger logger = null)
        {
            foreach (AlarmRuleStatus transientStatus in transientSource)
            {
                var persistedStatus = persistedSource.FirstOrDefault(x => x.AlarmRuleId == transientStatus.AlarmRuleId);

                if (persistedStatus == null || persistedStatus?.AlarmActive != transientStatus.AlarmActive)
                {
                    logger?.Log(LogLevel.Information, $"[{DateTime.Now}] {(persistedStatus == null ? "Unknown" : "Changed")} " +
                        $"{transientStatus.AlarmRuleId} {transientStatus.Status} alarm rule status for sensor {transientStatus.SensorId}");

                    yield return transientStatus;
                }
                else
                {
                    logger?.Log(LogLevel.Information, $"[{DateTime.Now}] Continued {transientStatus.AlarmRuleId} {transientStatus.Status} " +
                        $"alarm rule status for sensor {transientStatus.SensorId}");
                }
            }
        }
    }
}
