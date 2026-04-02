using MeuSistema.SharedKernel.Primitives;
using Microsoft.Extensions.Configuration;

namespace MeuSistema.SharedKernel.Extensions;

public static class ConfigurationExtensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : class, IAppOptions
    {
        return configuration
            .GetRequiredSection(TOptions.ConfigSectionPath)
            .Get<TOptions>(options => options.BindNonPublicProperties = true)!;
    }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO LINHA A LINHA 🔹
// -----------------------------------------
/*
Linha 1: using MeuSistema.SharedKernel.Primitives;
 - Importa o namespace onde está definida a interface IAppOptions.
 - Essa interface é usada como contrato para classes de configuração tipadas.

Linha 2: using Microsoft.Extensions.Configuration;
 - Importa o namespace da infraestrutura de configuração do .NET.
 - Permite acessar valores do appsettings.json, variáveis de ambiente, etc.

Linha 4: namespace MeuSistema.SharedKernel.Extensions;
 - Define o namespace onde ficará a extensão. Mantém o código organizado.

Linha 6: public static class ConfigurationExtensions
 - Cria uma classe estática para conter métodos de extensão.
 - Métodos de extensão permitem adicionar funcionalidades a tipos existentes (como IConfiguration).

Linha 8: public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
 - Define o método genérico GetOptions.
 - Ele é um método de extensão de IConfiguration (note o "this IConfiguration").
 - O tipo genérico TOptions representa a classe de configuração que você quer carregar.

Linha 9: where TOptions : class, IAppOptions
 - Restrição do genérico: TOptions deve ser uma classe (não struct) e implementar IAppOptions.
 - Isso garante que TOptions terá a propriedade estática ConfigSectionPath, usada para localizar a seção correta no appsettings.json.

Linha 11: return configuration
 - Começa a usar o objeto IConfiguration recebido como parâmetro.

Linha 12: .GetRequiredSection(TOptions.ConfigSectionPath)
 - Busca dentro do appsettings.json a seção cujo caminho está definido em TOptions.ConfigSectionPath.
 - "Required" significa que se a seção não existir, o sistema lança uma exceção (não deixa passar configuração faltando).

Linha 13: .Get<TOptions>(options => options.BindNonPublicProperties = true)!
 - Converte os valores da seção em uma instância da classe TOptions.
 - O lambda "options => options.BindNonPublicProperties = true" permite que até propriedades privadas sejam preenchidas.
 - O operador "!" no final indica que o resultado não deve ser nulo (força o compilador a aceitar isso).

👉 Lógica geral:
- Você cria classes de configuração que implementam IAppOptions.
- Cada classe define em ConfigSectionPath qual seção do appsettings.json ela representa.
- Esse método genérico lê a seção, instancia a classe e preenche todas as propriedades automaticamente.
- Assim, você não precisa escrever código manual de binding para cada configuração.
*/
