using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Infrastructure.Monitoring;
using ServiceBooking.Infrastructure.Persistence;
using ServiceBooking.Infrastructure.Persistence.Repositories;

namespace ServiceBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Register Telemetry first
        services.AddInfrastructureTelemetry(configuration); // 1. Get the connection string from configuration

        var cosmosAccountEndpoint = configuration["CosmosDb:AccountEndpoint"];
        var cosmosAccountKey = configuration["CosmosDb:AccountKey"];
        var databaseName = configuration["CosmosDb:DatabaseName"];

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseCosmos(
                cosmosAccountEndpoint!,
                cosmosAccountKey!,
                databaseName!));

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}