using MeuSistema.Infrastructure.Data.Context;          // Contexto principal da aplicação (AppDbContext, EventStoreDbContext)
using MeuSistema.SharedKernel.AppSettings;            // Classe ConnectionOptions com configurações de conexão
using MeuSistema.SharedKernel.Extensions;             // Extensões como GetOptions<T>
using Microsoft.EntityFrameworkCore;                  // EF Core base
using Microsoft.EntityFrameworkCore.Diagnostics;       // Eventos e diagnósticos do EF Core
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System;
// Logging

namespace MeuSistema.API.Extensions;

internal static class ServicesCollectionExtensions
{
    private const int DbMaxRetryCount = 3;             // Número máximo de tentativas automáticas em caso de falha
    private const int DbCommandTimeout = 30;           // Tempo limite (segundos) para execução de comandos SQL
    private const string DbMigrationAssemblyName = "MeuSistema.API"; // Assembly onde ficam as migrations
    private static readonly string[] DbRelationalTags = ["database", "ef-core", "Postgree", "relational"]; // Tags para health checks
    private const string TestingEnvironmentName = "Testing"; // Nome do ambiente de testes

    // Método de extensão para registrar os DbContexts da aplicação
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IWebHostEnvironment environment)
    {
        // Só registra se não estiver em ambiente de testes
        if (!environment.IsEnvironment(TestingEnvironmentName))
        {
            // Contexto principal (AppDbContext) com tracking completo
            services.AddDbContextPool<AppDbContext>((serviceProvider, optionsBuilder) =>
                ConfigureDbContext<AppDbContext>(
                    serviceProvider, optionsBuilder, QueryTrackingBehavior.TrackAll));

            // Contexto de eventos (EventStoreDbContext) com tracking otimizado (NoTrackingWithIdentityResolution)
            services.AddDbContextPool<EventStoreDbContext>((serviceProvider, optionsBuilder) =>
                ConfigureDbContext<EventStoreDbContext>(
                    serviceProvider, optionsBuilder, QueryTrackingBehavior.NoTrackingWithIdentityResolution));
        }

        return services;
    }

    // Configuração genérica para qualquer DbContext
    private static void ConfigureDbContext<TDbContext>(
        IServiceProvider serviceProvider,
        DbContextOptionsBuilder optionsBuilder,
        QueryTrackingBehavior queryTrackingBehavior)
        where TDbContext : DbContext
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TDbContext>>(); // Logger para registrar warnings
        var options = serviceProvider.GetOptions<ConnectionOptions>();          // Pega opções de conexão do appsettings.json
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>(); // Descobre ambiente atual
        var envIsDevelopment = environment.IsDevelopment();                       // Verifica se é ambiente de desenvolvimento

        optionsBuilder
            .UseNpgsql(options.PostGreeSqlConnection, npgsqlOptions =>           // Configura PostgreSQL
            {
                npgsqlOptions
                    .MigrationsAssembly(DbMigrationAssemblyName)   // Define onde estão as migrations
                    .EnableRetryOnFailure(DbMaxRetryCount)         // Habilita retry automático em falhas de conexão
                    .CommandTimeout(DbCommandTimeout);             // Define timeout para comandos
            })
            .EnableDetailedErrors(envIsDevelopment)                // Ativa erros detalhados só em dev
            .EnableSensitiveDataLogging(envIsDevelopment)          // Loga dados sensíveis só em dev (cuidado em produção)
            .UseQueryTrackingBehavior(queryTrackingBehavior)       // Define comportamento de tracking (TrackAll ou NoTracking)
            .LogTo((eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying, eventData =>
            {
                // Loga tentativas de retry do EF Core
                if (eventData is not ExecutionStrategyEventData retryEventData)
                    return;

                var exceptions = retryEventData.ExceptionsEncountered;

                logger.LogWarning(
                    "----- DbContext: Tentativa #{Count} com atraso de {Delay} devido ao erro: {Message}",
                    exceptions.Count,
                    retryEventData.Delay,
                    exceptions[^1].Message);
            });

        // Em desenvolvimento, loga todas as queries no console
        if (envIsDevelopment)
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}
