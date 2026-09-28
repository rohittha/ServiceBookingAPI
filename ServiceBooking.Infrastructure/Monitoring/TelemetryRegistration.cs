using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBooking.Infrastructure.Monitoring;
public static class TelemetryRegistration
{
    public static IServiceCollection AddInfrastructureTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["Telemetry:ServiceName"] ?? "ServiceBooking.API";

        services.AddOpenTelemetry()
            .UseAzureMonitor(options =>
            {
                options.ConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            })
            .ConfigureResource(resourceBuilder =>
                resourceBuilder.AddAttributes(new Dictionary<string, object> {
                    { "service.name", serviceName },
                    { "service.instance.id", Environment.MachineName }
                }));

        return services;
    }
}
