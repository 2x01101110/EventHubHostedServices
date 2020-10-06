using EventHubHostedServices.BuildingBlocks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubHostedServices.BuildingBlocks.Contracts
{
    public interface IAlarmRuleRepository
    {
        Task<IEnumerable<SensorAlarmRules>> GetSensorAlarmRulesAsync();
        Task<IEnumerable<AlarmRuleStatus>> GetSensorAlarmRuleStatuses();
    }
}
