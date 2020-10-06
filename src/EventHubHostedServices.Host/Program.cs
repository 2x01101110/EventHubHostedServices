using EventHubHostedServices.BuildingBlocks.Contracts;
using EventHubHostedServices.BuildingBlocks.Contracts.Data;
using EventHubHostedServices.BuildingBlocks.Infrastructure;
using EventHubHostedServices.Host.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventHubHostedServices.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IAlarmNotificationService, AlarmNotificationService>();
                    services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>();
                    services.AddTransient<IAlarmRuleRepository, AlarmRuleRepository>();
                    services.AddTransient<ICachingService, CachingService>();
                    services.AddMemoryCache();

                    services.AddHostedService<AlarmRuleCacheHostedService>();
                    services.AddHostedService<EventHubTelemetryHostedService>();

                    services.AddApplicationInsightsTelemetry();
                });
        }
    }
}
