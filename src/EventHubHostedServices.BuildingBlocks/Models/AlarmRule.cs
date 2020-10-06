using System;
using System.Collections.Generic;

namespace EventHubHostedServices.BuildingBlocks.Models
{
    public class AlarmRule
    {
        public Guid AlarmRuleId { get; }
        public string AlarmRuleName { get; }
        public IEnumerable<Condition> Conditions { get; }

        public AlarmRule(
            Guid alarmRuleId,
            string alarmRuleName,
            IEnumerable<Condition> conditions)
        {
            this.AlarmRuleId = alarmRuleId;
            this.AlarmRuleName = alarmRuleName;
            this.Conditions = conditions;
        }
    }
}
