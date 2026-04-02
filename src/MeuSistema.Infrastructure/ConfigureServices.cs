using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.Infrastructure.Data.Repositories;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace MeuSistema.Infrastructure;

public static class ConfigureServices
{
   
   
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
