using MeuSistema.SharedKernel.AppSettings;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace MeuSistema.SharedKernel;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services) =>
        services
        .AddOptionsWithValidation<ConnectionOptions>();


    private static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services)
        where TOptions : class, IAppOptions
    {
        return services
             .AddOptions<TOptions>()
             .BindConfiguration(TOptions.ConfigSectionPath, binderOptions => binderOptions.BindNonPublicProperties = true)
             .ValidateDataAnnotations()
             .ValidateOnStart()
             .Services;
    }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO LINHA A LINHA 🔹
// -----------------------------------------
/*
Linha 1-3: Importa namespaces necessários.
 - AppSettings → contém classes de configuração como ConnectionOptions.
 - Primitives → contém a interface IAppOptions, usada como contrato para opções.
 - DependencyInjection → fornece métodos para registrar serviços e opções no container DI.

Linha 5: Define o namespace SharedKernel, agrupando código comum da aplicação.

Linha 7: Cria a classe estática ConfigureServices, usada para centralizar configuração de serviços.

Linha 9-11: Método ConfigureAppSettings.
 - É um método de extensão para IServiceCollection.
 - Chama AddOptionsWithValidation<ConnectionOptions>, registrando a classe ConnectionOptions como opções de configuração já com validação automática.

Linha 14: Método privado AddOptionsWithValidation<TOptions>.
 - É genérico: funciona para qualquer classe de configuração.
 - Restrição "where TOptions : class, IAppOptions":
   → garante que TOptions seja uma classe (não struct).
   → garante que implemente IAppOptions, que define ConfigSectionPath (o caminho da seção no appsettings.json).

Linha 17: services.AddOptions<TOptions>()
 - Registra TOptions como opções no DI, permitindo injetar IOptions<TOptions> em qualquer lugar.

Linha 18: .BindConfiguration(TOptions.ConfigSectionPath, binderOptions => binderOptions.BindNonPublicProperties = true)
 - Liga os valores da seção do appsettings.json (definida em ConfigSectionPath) à classe TOptions.
 - binderOptions.BindNonPublicProperties = true → permite preencher até propriedades privadas, útil para encapsular lógica.

Linha 19: .ValidateDataAnnotations()
 - Aplica validação baseada em atributos como [Required], [Range], etc. definidos na classe TOptions.

Linha 20: .ValidateOnStart()
 - Força a validação logo na inicialização da aplicação.
 - Se houver configuração inválida ou faltando, a aplicação falha no startup, evitando erros em runtime.

Linha 21: .Services
 - Retorna a IServiceCollection para continuar encadeando chamadas.

👉 Em resumo: esse código cria um padrão para registrar classes de configuração tipadas (como ConnectionOptions) no DI, vinculando-as ao appsettings.json e validando automaticamente os valores antes da aplicação iniciar.
*/
