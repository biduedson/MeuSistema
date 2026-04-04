using MeuSistema.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.API.Extensions
{
    // 🔹 Classe estática de extensões para WebApplication
    internal static class WebApplicationExtensions
    {
        // Método de inicialização principal da aplicação
        public static async Task RunAppAsync(this WebApplication app)
        {
            // Cria um escopo de serviços para resolver dependências
            await using var serviceScope = app.Services.CreateAsyncScope();

            // Executa migrações de banco de dados
            await app.MigrateDataBaseAsync(serviceScope);

            // Inicia a aplicação (escuta requisições HTTP)
            app.Logger.LogInformation("----- Iniciando a aplicação...");
            await app.RunAsync();
        }

        // Método que aplica migrações nos bancos relacionais
        public static async Task MigrateDataBaseAsync(this WebApplication app, AsyncServiceScope serviceScope)
        {
            // Resolve os DbContexts do container de DI
            await using var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            await using var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

            try
            {
                // Aplica migrações no banco principal
                await app.MigrateDbContextAsync(appDbContext);

                // Aplica migrações no banco de eventos
                await app.MigrateDbContextAsync(eventStoreDbContext);
            }
            catch (Exception ex)
            {
                // Loga erro caso algo dê errado
                app.Logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados: {Message}", ex.Message);

                // Re-lança a exceção para ser tratada pelo middleware global
                throw;
            }
        }

        // Método genérico para aplicar migrações em qualquer DbContext
        public static async Task MigrateDbContextAsync<TDContext>(this WebApplication app, TDContext dbContext)
            where TDContext : DbContext
        {
            // Obtém o nome do banco de dados atual
            var dbName = dbContext.Database.GetDbConnection().Database;

            app.Logger.LogInformation("----- {DbName}: verificando migrações pendentes...", dbName);

            // Lista migrations reais que ainda não foram aplicadas
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                // Caso existam migrations criadas e não aplicadas
                app.Logger.LogInformation(
                    "----- {DbName}: aplicando {Count} migrations pendentes...",
                    dbName,
                    pendingMigrations.Count()
                );

                await dbContext.Database.MigrateAsync();

                app.Logger.LogInformation("----- {DbName}: banco de dados migrado com sucesso!", dbName);
            }
            else if (dbContext.Database.HasPendingModelChanges())
            {
                // Caso o modelo tenha mudado mas nenhuma migration foi criada
                app.Logger.LogWarning(
                    "----- {DbName}: houve mudanças no modelo, mas nenhuma migration foi encontrada. Crie uma migration para aplicar essas alterações.",
                    dbName
                );
            }
            else
            {
                // Caso não haja migrations pendentes e o modelo esteja sincronizado
                app.Logger.LogInformation(
                    "----- {DbName}: banco de dados já está atualizado, nenhuma migration pendente.",
                    dbName
                );
            }
        }
    }
}
