using Microsoft.Extensions.DependencyInjection;

namespace MetalFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // This is where application-specific services like MediatR, AutoMapper,
        // or application services/use cases would be registered.
        return services;
    }
}