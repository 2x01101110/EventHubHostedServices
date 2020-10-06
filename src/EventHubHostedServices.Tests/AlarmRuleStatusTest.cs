using EventHubHostedServices.BuildingBlocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EventHubHostedServices.Tests
{
    public class AlarmRuleStatusTest
    {
        [Fact]
        public void active_when_reading_value_equals_to_alarm_rule_condition_value()
        {
            var alarmRules = new List<AlarmRule>
            {
                new AlarmRule(Guid.NewGuid(), "Equals", new List<Condition>()
                {
                    new Condition("==", string.Empty, 100)
                })
            };

            var status = SensorAlarmRules.GetAlarmRuleStatuses(1, alarmRules, 100);

            Assert.True(status.All(x => x.AlarmActive));
        }

        [Fact]
        public void active_when_reading_value_greater_than_alarm_rule_condition_value()
        {
            var alarmRules = new List<AlarmRule>
            {
                new AlarmRule(Guid.NewGuid(), "Equals", new List<Condition>()
                {
                    new Condition(">", string.Empty, 100),
                })
            };

            var status = SensorAlarmRules.GetAlarmRuleStatuses(1, alarmRules, 110);

            Assert.True(status.All(x => x.AlarmActive));
        }

        [Fact]
        public void active_when_reading_value_greater_or_equal_to_alarm_rule_condition_value()
        {
            var alarmRules = new List<AlarmRule>
            {
                new AlarmRule(Guid.NewGuid(), "Equals", new List<Condition>()
                {
                    new Condition(">=", string.Empty, 100),
                })
            };

            var status = SensorAlarmRules.GetAlarmRuleStatuses(1, alarmRules, 110);

            Assert.True(status.All(x => x.AlarmActive));
        }

        [Fact]
        public void active_when_reading_value_less_than_alarm_rule_condition_value()
        {
            var alarmRules = new List<AlarmRule>
            {
                new AlarmRule(Guid.NewGuid(), "Equals", new List<Condition>()
                {
                    new Condition("<", string.Empty, 100),
                })
            };

            var status = SensorAlarmRules.GetAlarmRuleStatuses(1, alarmRules, 99);

            Assert.True(status.All(x => x.AlarmActive));
        }
    }
}
