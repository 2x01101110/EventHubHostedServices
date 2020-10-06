using Dapper;
using EventHubHostedServices.BuildingBlocks.Contracts;
using EventHubHostedServices.BuildingBlocks.Contracts.Data;
using EventHubHostedServices.BuildingBlocks.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utf8Json;

namespace EventHubHostedServices.BuildingBlocks.Infrastructure
{
    public class AlarmRuleRepository : IAlarmRuleRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly ILogger<AlarmRuleRepository> _logger;

        public AlarmRuleRepository(
            ISqlConnectionFactory sqlConnectionFactory,
            ILogger<AlarmRuleRepository> logger)
        {
            this._sqlConnectionFactory = sqlConnectionFactory;
            this._logger = logger;
        }

        public async Task<IEnumerable<SensorAlarmRules>> GetSensorAlarmRulesAsync()
        {
            var sql =
                "SELECT " +
                    "SA.SensorId, AR.Id AS AlarmRuleId, AR.Name AS AlarmRuleName, AR.Conditions " +
                "FROM " +
                    "SensorAlarmRules AS SA JOIN AlarmRules AS AR ON SA.AlarmRuleId = AR.Id " +
                "WHERE AR.Enabled = 1";

            var connection = this._sqlConnectionFactory.GetOpenConnection();

            var queryResult = await connection.QueryAsync<dynamic>(sql);

            var returnResult = queryResult
                .GroupBy(x => x.SensorId)
                .Select(x => new SensorAlarmRules
                ( 
                    sensorId: x.Key,
                    alarmRules: x.Select(a => new AlarmRule
                    ( 
                        alarmRuleId: a.AlarmRuleId,
                        alarmRuleName: a.AlarmRuleName,
                        conditions: JsonSerializer.Deserialize<IEnumerable<Condition>>(a.Conditions)
                    ))
                ));

            return returnResult;
        }

        public async Task<IEnumerable<AlarmRuleStatus>> GetSensorAlarmRuleStatuses()
        {
            var sql =
                "select " +
                    "SA.SensorId, SA.AlarmRuleId, CASE WHEN AA.AlarmActive IS NULL THEN 1 ELSE 0 END AS AlarmActive " +
                "FROM " +
                    "SensorAlarmRules AS SA " +
                "JOIN " +
                    "AlarmRules AS AR on SA.AlarmRuleId = AR.Id " +
                "outer apply " +
                    "(select count(*) as AlarmActive FROM Alarms AS A WHERE A.AlarmRuleId = SA.AlarmRuleId AND [End] is null) AS AA " +
                "WHERE AR.Enabled = 1";

            var connection = this._sqlConnectionFactory.GetOpenConnection();

            var queryResult = await connection.QueryAsync<dynamic>(sql);

            var returnResult = queryResult.Select(x => new AlarmRuleStatus(
                sensorId: x.SensorId, 
                alarmRuleId:  x.AlarmRuleId, 
                alarmActive: Convert.ToBoolean(x.AlarmActive)
            ));

            return returnResult;
        }
    }
}
