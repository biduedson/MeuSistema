using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MeuSistema.API.Extensions;
using MeuSistema.Application;
using MeuSistema.Infrastructure;
using MeuSistema.SharedKernel;
using MeuSistema.SharedKernel.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
       
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                  .Configure<JsonOptions>(jsonOptions => jsonOptions.JsonSerializerOptions.Configure());
            
            builder.Services.AddOpenApi();
            builder.Services.AddControllers()
                 .ConfigureApiBehaviorOptions( behaiorOptions =>
                 {
                     behaiorOptions.SuppressMapClientErrors = true;
                     behaiorOptions.SuppressModelStateInvalidFilter = true;
                 }).AddJsonOptions(_ => { });


            // Adicionando os serviços da aplicação no ASP.NET Core DI.
            builder.Services
                .ConfigureAppSettings()
                .AddInfrastructure()
                .AddCommandHandlers()
                .AddAppDbContext(builder.Environment)
                .AddRepositories();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference("/docs", options =>
                {
                    options.DarkMode = true;
                    options.Title = "MeuSistema API";
                });

            }
            app.UseErrorHanDling(); 
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            await  app.RunAppAsync();


// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------

/*
 * ✅ builder.Services → ServiceCollection (coleção de serviços)
 *    Cada AddControllers/AddOpenApi/AddRepositories insere descrições de serviços (ServiceDescriptor) na lista.
 *    Tradução literal: "coleção de serviços" = lista onde você registra dependências.
 *
 * ✅ builder.Configuration → ConfigurationRoot (raiz da configuração)
 *    Já vem preenchido com appsettings.json, variáveis de ambiente e argumentos de linha de comando.
 *    Tradução literal: "raiz da configuração" = objeto que centraliza todas as fontes de configuração.
 *
 * ✅ app.Services → ServiceProvider (provedor de serviços)
 *    Criado no Build(), entrega instâncias dos serviços registrados no ServiceCollection.
 *    Tradução literal: "provedor de serviços" = container que fornece os objetos quando solicitados.
 *
 * ✅ app → WebApplication
 *    Implementa IApplicationBuilder (configuração de middlewares),
 *    IEndpointRouteBuilder (mapeamento de rotas),
 *    IHost (ciclo de vida da aplicação),
 *    IDisposable/IAsyncDisposable (liberação de recursos).
 *    Tradução literal: "aplicação web" = objeto que junta tudo e executa.
 *
 * ✅ Pipeline HTTP → Define como as requisições são processadas:
 *    - Tratamento de erros (UseErrorHanDling)
 *    - Redirecionamento HTTPS
 *    - Autorização
 *    - Controllers mapeados
 *
 * ✅ Ambiente de desenvolvimento → Se for Development, ativa OpenAPI e Scalar Docs.
 *
 * ✅ Execução → app.RunAppAsync() inicia o servidor Kestrel e usa o ServiceProvider para resolver dependências.
 *
 * ✅ Top-level statements → A partir do .NET 6 não é mais necessário declarar a classe Program e o método Main.
 *    Tradução literal: "declarações de nível superior" = código direto no Program.cs sem precisar de classe.
 *    O compilador gera automaticamente Program/Main por baixo dos panos.
 */