using System;

namespace EventHubHostedServices.BuildingBlocks.Models
{
    public class AlarmRuleStatus
    {
        public int SensorId { get; }
        public Guid AlarmRuleId { get; }
        public bool AlarmActive { get; }

        public AlarmRuleStatus(
            int sensorId,
            Guid alarmRuleId, 
            bool alarmActive)
        {
            this.SensorId = sensorId;
            this.AlarmRuleId = alarmRuleId;
            this.AlarmActive = alarmActive;
        }

        public string Status => this.AlarmActive ? "active" : "inactive";
        public override string ToString() => $"{this.SensorId}:{this.AlarmRuleId}";
    }
}
