using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleLocalization.Internal;

namespace SimpleLocalization.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleLocalization(
        this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        LocalizationStore.Initialize(assemblies);

        services.AddLocalization();
        
        return services;
    }
}