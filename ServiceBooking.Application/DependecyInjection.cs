using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ServiceBooking.Application.Common.Behaviors;

namespace ServiceBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Register MediatR
        // This scans the current assembly to find all Classes that implement IRequestHandler
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // 2. Register Pipeline Behaviors
            // These act like "Middleware" for your application requests
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // 3. Register FluentValidation
        // This scans the assembly for any class that inherits from AbstractValidator
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}