using FluentAssertions;
using MeuSistema.SharedKernel.AppSettings;
using MeuSistema.SharedKernel.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit.Categories;

namespace MeuSistema.UnitTests.SharedKernel.Extensions;

[UnitTest]
public class ConfigurationExtensionsTests
{
    [Fact]
    public void Should_ReturnsClassOptions_WhenGetOptions()
    {
        const string postGreeSqlConnection = "connection string test";

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ConnectionStrings:PostGreeSqlConnection", postGreeSqlConnection},
        }!);

        var configuration = configurationBuilder.Build();

        var act = configuration.GetOptions<ConnectionOptions>();

        act.Should().NotBeNull();
        act.PostGreeSqlConnection.Should().Be(postGreeSqlConnection);

    }
}

// ---------------- COMENTÁRIOS EXPLICATIVOS ----------------
//
// using FluentAssertions → "usando FluentAssertions". Biblioteca para escrever verificações de teste de forma legível.
// using MeuSistema.SharedKernel.AppSettings → importa namespace do sistema, contém a classe ConnectionOptions.
// using MeuSistema.SharedKernel.Extensions → importa namespace com o método de extensão GetOptions<T>().
// using Microsoft.Extensions.Configuration → biblioteca oficial do .NET para configuração (appsettings.json, variáveis de ambiente).
// using Xunit.Categories → biblioteca para categorizar testes (ex.: [UnitTest]).
//
// namespace MeuSistema.UnitTests.SharedKernel.Extensions → define o espaço de nomes onde o teste está.
//
// [UnitTest] → atributo que marca a classe como teste unitário.
// public class ConfigurationExtensionsTests → classe de teste.
//
// [Fact] → atributo do xUnit, indica que o método é um teste.
// public void Should_ReturnsClassOptions_WhenGetOptions() → nome do método: "Deve retornar opções da classe quando chamar GetOptions".
//
// const string postGreeSqlConnection = "connection string test"; 
// → constante string, valor de teste para string de conexão.
//
// var configurationBuilder = new ConfigurationBuilder(); 
// → cria um "Construtor de Configuração". Builder = construtor em português literal.
//
// configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> {...}); 
// → adiciona uma coleção em memória simulando appsettings.json.
// Dictionary = dicionário (estrutura chave/valor).
// Chave: "ConnectionStrings:PostGreeSqlConnection". Valor: a string de conexão.
//
// var configuration = configurationBuilder.Build(); 
// → constrói o objeto final IConfiguration. Build = construir.
//
// var act = configuration.GetOptions<ConnectionOptions>(); 
// → chama método de extensão para obter opções tipadas. Retorna objeto ConnectionOptions.
//
// act.Should().NotBeNull(); 
// → usando FluentAssertions. Verifica que o objeto não é nulo.
//
// act.PostGreeSqlConnection.Should().Be(postGreeSqlConnection); 
// → verifica que a propriedade PostGreeSqlConnection tem exatamente o valor esperado.
//
// ----------------------------------------------------------
// Em resumo: o teste cria uma configuração em memória, simula uma string de conexão,
// usa GetOptions<T>() para mapear para a classe ConnectionOptions e verifica se o valor foi carregado corretamente.
