using System.Reflection;
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
        var assenbly = Assembly.GetAssembly(typeof(IApplicationMarker));

        return services
            .AddValidatorsFromAssembly(assenbly, ServiceLifetime.Singleton)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assenbly!)
                 .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>)));
    }
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA DO CÓDIGO 🔹
// ------------------------------------------------------
// 1. using System.Reflection → Permite inspecionar assemblies e localizar classes em tempo de execução.
// 2. using FluentValidation → Biblioteca para validar objetos (ex.: comandos) com regras claras e reutilizáveis.
// 3. using MediatR → Implementa o padrão Mediator, que organiza a comunicação entre comandos, queries e eventos.
// 4. using MeuSistema.Application.Abstractions → Contém a interface IApplicationMarker, usada como "marcador" para localizar o assembly da aplicação.
// 5. using MeuSistema.Application.Behaviors → Onde está definido o LoggingBehavior, que intercepta requisições.
// 6. using Microsoft.Extensions.DependencyInjection → Permite registrar serviços na injeção de dependência do .NET.
//
// Classe ConfigureServices:
// - É estática e serve para centralizar a configuração dos serviços da aplicação.
//
// Método AddCommandHandlers:
// - É um método de extensão para IServiceCollection, usado na inicialização da aplicação.
// - Localiza o assembly da aplicação com Assembly.GetAssembly(typeof(IApplicationMarker)).
// - Registra todos os validadores do FluentValidation encontrados nesse assembly (AddValidatorsFromAssembly).
// - Registra todos os handlers do MediatR (commands, queries, notifications) com RegisterServicesFromAssembly.
// - Adiciona o LoggingBehavior ao pipeline do MediatR com AddBehavior.
//   → O LoggingBehavior funciona como um "middleware interno": antes e depois de cada comando/query,
//     ele executa lógica de logging (tempo de execução, erros, etc.).
//
// Benefícios:
// - Código mais limpo: handlers focam apenas na regra de negócio.
// - Modularidade: comportamentos transversais (logging, validação, etc.) ficam centralizados.
// - Escalabilidade: fácil adicionar novos behaviors sem alterar os handlers.
// ------------------------------------------------------
