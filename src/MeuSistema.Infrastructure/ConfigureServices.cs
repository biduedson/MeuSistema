using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Infrastructure.Configurations;
using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
   
    public static IServiceCollection AddInfrastructureConfiguration(
        this IServiceCollection services,
        IConfiguration configuration) =>
   services.Configure<ConnectionOptions>(
            configuration.GetSection(ConnectionOptions.ConfigSectionPath));
    
 
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services) =>
        services
            .AddScoped<AppDbContext>()
            .AddScoped<EventStoreDbContext>();
    
    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IEventStoreRepository, EventStoreRepository>();
}
