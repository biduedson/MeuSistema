using System.Text.Json.Serialization;
using MeuSistema.API.Extensions;
using MeuSistema.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

namespace MeuSistema.API
{
    public class Program
    {
        public static void Main(string[] args)
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
                .AddInfrastructure()
                .AddRepositories()
                .AddAppDbContext(builder.Environment);

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
            app.Run();
        }
    }
}
