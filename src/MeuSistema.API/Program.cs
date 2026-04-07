using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MeuSistema.API.Extensions;
using MeuSistema.Application;
using MeuSistema.Infrastructure;
using MeuSistema.SharedKernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace MeuSistema.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
            .Configure<JsonOptions>(jsonOptions =>
            {
            jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddOpenApi();
            builder.Services.AddControllers();


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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            await  app.RunAppAsync();
        }
    }
}
