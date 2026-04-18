using FluentAssertions;
using MeuSistema.SharedKernel;
using MeuSistema.SharedKernel.AppSettings;
using MeuSistema.SharedKernel.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Categories;

namespace MeuSistema.UnitTests.SharedKernel.Extensions;

[UnitTest]
public class ServicesCollectionExtensionsTests
{
    [Fact]
    public void Should_ReturnClassOptions_WhenConfigureAppSettings()
    {
        const string postGreeSqlConnection = "connection string test";

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ConnectionStrings:PostGreeSqlConnection", postGreeSqlConnection},
        }!);

        var configuration = configurationBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(_ => configuration);
        services.ConfigureAppSettings();
        var serviceProvider = services.BuildServiceProvider(true);

        var act = serviceProvider.GetOptions<ConnectionOptions>();

        act.Should().NotBeNull();
        act.PostGreeSqlConnection.Should().Be(postGreeSqlConnection);
    }
}

// ---------------- COMENTÁRIOS EXPLICATIVOS ----------------
//
// using FluentAssertions → "usando FluentAssertions". Biblioteca para escrever verificações de teste de forma legível.
// using MeuSistema.SharedKernel → importa o núcleo compartilhado do sistema.
// using MeuSistema.SharedKernel.AppSettings → contém a classe ConnectionOptions.
// using MeuSistema.SharedKernel.Extensions → contém o método de extensão ConfigureAppSettings().
// using Microsoft.Extensions.Configuration → biblioteca oficial do .NET para configuração.
// using Microsoft.Extensions.DependencyInjection → biblioteca oficial do .NET para injeção de dependência (DI).
// using Xunit.Categories → categoriza testes (ex.: [UnitTest]).
//
// namespace MeuSistema.UnitTests.SharedKernel.Extensions → define o espaço de nomes do teste.
//
// [UnitTest] → marca a classe como teste unitário.
// public class ServicesCollectionExtensionsTests → classe de teste.
//
// [Fact] → atributo do xUnit, indica que o método é um teste.
// public void Should_ReturnClassOptions_WhenConfigureAppSettings() → nome do método: "Deve retornar opções da classe quando configurar AppSettings".
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
// var services = new ServiceCollection(); 
// → cria uma coleção de serviços (DI container).
//
// services.AddSingleton<IConfiguration>(_ => configuration); 
// → registra IConfiguration como singleton no container, usando a configuração criada.
//
// services.ConfigureAppSettings(); 
// → método de extensão que registra as opções tipadas (ConnectionOptions) no container.
//
// var serviceProvider = services.BuildServiceProvider(true); 
// → constrói o provedor de serviços (ServiceProvider). É quem entrega instâncias configuradas.
// ServiceProvider = provedor de serviços, container de DI.
//
// var act = serviceProvider.GetOptions<ConnectionOptions>(); 
// → obtém a classe ConnectionOptions a partir do ServiceProvider.
//
// act.Should().NotBeNull(); 
// → verifica que o objeto não é nulo.
//
// act.PostGreeSqlConnection.Should().Be(postGreeSqlConnection); 
// → verifica que a propriedade PostGreeSqlConnection tem exatamente o valor esperado.
//
// ----------------------------------------------------------
// Em resumo: esse teste garante que, ao registrar IConfiguration e chamar ConfigureAppSettings(),
// o ServiceProvider consegue fornecer corretamente uma instância de ConnectionOptions com os valores da configuração.
