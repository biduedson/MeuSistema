using FluentValidation;
using MediatR;
using MeuSistema.Application.Abstractions;
using MeuSistema.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MeuSistema.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        var assembly = typeof(IApplicationMarker).Assembly;

        return services
            .AddValidatorsFromAssembly(assembly, ServiceLifetime.Singleton)
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });
    }
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Esse método registra automaticamente os componentes da camada Application no DI.
//
// 1) IApplicationMarker
// → Interface vazia usada apenas para localizar o assembly (projeto) correto.
// → Serve como referência para o .NET saber onde procurar as classes.
//
// 2) Assembly
// → Representa o projeto compilado (Application).
// → A partir dele, o sistema consegue escanear e encontrar classes automaticamente.
//
// 3) FluentValidation
// → AddValidatorsFromAssembly registra todos os validators do projeto.
// → Evita registrar manualmente cada AbstractValidator<T>.
//
// 4) MediatR
// → RegisterServicesFromAssembly registra automaticamente todos os handlers:
//    - Commands (IRequest)
//    - Queries
//    - Notifications
//
// 5) LoggingBehavior
// → Adicionado ao pipeline do MediatR.
// → Funciona como um middleware interno:
//    antes e depois de cada request ele pode executar lógica (log, tempo, erros, etc.)
//
// 6) Resultado final
// → Menos código manual
// → Mais organização (Clean Architecture / CQRS)
// → Fácil de escalar e manter
// ------------------------------------------------------