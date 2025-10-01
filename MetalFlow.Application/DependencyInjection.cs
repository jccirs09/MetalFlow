using Microsoft.Extensions.DependencyInjection;

namespace MetalFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services here (e.g., MediatR, AutoMapper, etc.)
        return services;
    }
}