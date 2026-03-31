

using MeuSistema.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeuSistema.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)

    {
        services.Configure<ConnectionOptions>
            (configuration.GetSection(ConnectionOptions.ConfigSectionPath));

        return services;
    }

        
}
