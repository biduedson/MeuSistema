using System.Net.NetworkInformation;
using FluentAssertions;
using MeuSistema.SharedKernel.Extensions;
using Xunit.Categories;

namespace MeuSistema.UnitTests.SharedKernel.Extensions;

[UnitTest]
public class JsonExtensionsTests
{
    private const string UserJson =
        "{\"email\":\"john.doe@hotmail.com\",\"userName\":\"John Doe\",\"status\":\"active\"}";

    [Fact]
    public void Should_ReturnJsonString_WhenSerialize()
    {
        var user = new User("John Doe", "john.doe@hotmail.com", EStatus.Active);

        var act = user.ToJson();

        act.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo(UserJson);
    }

    [Fact]
    public void Should_ReturnEntity_WhenDeserializeTyped()
    {
        var expectdeUser = new User("John Doe", "john.doe@hotmail.com", EStatus.Active);

        var act = UserJson.FromJson<User>();

        act.Should().NotBeNull().And.BeEquivalentTo(expectdeUser);
        act.UserName.Should().NotBeNullOrWhiteSpace();
        act.Email.Should().NotBeNullOrWhiteSpace();
        act.Status.Should().Be(EStatus.Active);
    }

    [Fact]
    public void Should_ReturnNull_WhenSerializeNullValue()
    {
        User user = null!;

        var act = user.ToJson();

        act.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnNull_WhenDeserializeNullValueTyped()
    {
        const string strJson = null!;

        var act = strJson.FromJson<User>();

        act.Should().BeNull();
    }

    private enum EStatus
    {
        Active = 0,
        Inactive = 1
    }

    private record User(string UserName, string Email, EStatus Status)
    {
        public string Email { get; } = Email;
        public string UserName { get; } = UserName;
        public EStatus Status { get; } = Status;
    }
}

/*
📌 Explicação detalhada dos testes:

1. Should_ReturnJsonString_WhenSerialize
   - Cria um objeto User com dados válidos.
   - Chama a extensão ToJson(), que serializa o objeto para JSON.
   - Verifica que a string resultante não é nula/vazia e é equivalente ao JSON esperado (UserJson).
   - Garante que a serialização está funcionando corretamente.

2. Should_ReturnEntity_WhenDeserializeTyped
   - Define um objeto User esperado.
   - Pega a string JSON (UserJson) e chama FromJson<User>() para deserializar.
   - Verifica que o objeto resultante não é nulo e é equivalente ao esperado.
   - Faz asserts adicionais: UserName e Email não são nulos/vazios, Status é Active.
   - Garante que a deserialização está funcionando e que os dados foram mapeados corretamente.

3. Should_ReturnNull_WhenSerializeNullValue
   - Define uma variável User como null.
   - Chama ToJson() sobre esse valor nulo.
   - Verifica que o resultado é null.
   - Testa o comportamento seguro da extensão ao lidar com objetos nulos.

4. Should_ReturnNull_WhenDeserializeNullValueTyped
   - Define uma string JSON como null.
   - Chama FromJson<User>() sobre essa string nula.
   - Verifica que o resultado é null.
   - Testa o comportamento seguro da extensão ao lidar com strings nulas.

📌 Tipos auxiliares:
- EStatus: enum com valores Active e Inactive. No JSON, é serializado como string em camelCase ("active"/"inactive").
- User: record que representa o usuário nos testes, com propriedades UserName, Email e Status.
  As propriedades são camelCase no JSON por causa da configuração global JsonNamingPolicy.CamelCase.

✅ Em resumo:
Esses testes garantem que as extensões ToJson e FromJson funcionam corretamente tanto em cenários normais (serialização e deserialização de objetos válidos) quanto em cenários de borda (valores nulos). Isso assegura robustez e consistência no uso de JSON dentro da aplicação.
*/
