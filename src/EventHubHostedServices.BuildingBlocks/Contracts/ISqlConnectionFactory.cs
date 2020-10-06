using System.Data;

namespace EventHubHostedServices.BuildingBlocks.Contracts.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
