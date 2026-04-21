

using Bogus;
using FluentAssertions;
using MeuSistema.Application.Customer.Queries.GetCustomers;
using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data.Repositories;
using MeuSistema.UnitTests.Fixtures;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

public class GetCustomersQueryHandlerTests(EfSqliteFixture fixture) : IClassFixture<EfSqliteFixture>
{
    [Fact]
    public async Task GetAll_Customer_ShouldReturnsSuccessResult()
    {
        var customers = new Faker<MeuSistema.Domain.Entities.CustumerAggregate.Customer>()
            .CustomInstantiator(faker => MeuSistema.Domain.Entities.CustumerAggregate.Customer.Create(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                faker.Person.Email,
                faker.Person.DateOfBirth))
            .Generate(4);

        var repository = new CustomerRepository(fixture.Context);

        foreach (var custumer in customers)
        {
            repository.Add(custumer);
        }
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var  readOnlyResopistory = new CustomerReadOnlyRepository(fixture.Context); 

        var queryHandler = new GetCustomersQueryHandler(
            readOnlyResopistory
            );

        var query = new GetCustomersQuery();
        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IReadOnlyList<CustomerQueryModel>>();

    }

    [Fact]
    public async Task GetAll_Customer_Empty_ShouldReturnsSuccessResultWithEmptyList()
    {
        var readonlyRepository = new CustomerReadOnlyRepository(fixture.Context);

        fixture.Context.Customers.RemoveRange(fixture.Context.Customers);

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Clear();

        var queryHandler = new GetCustomersQueryHandler(readonlyRepository);
        var query = new GetCustomersQuery();

        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IReadOnlyList<CustomerQueryModel>>();
        act.Value.Should().BeEmpty(); 
    }

}
// -----------------------------------------------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA DOS TESTES 🔹
// -----------------------------------------------------------------------------
/*
✅ Contexto geral
→ Esses testes validam o comportamento do GetCustomersQueryHandler,
   responsável por retornar a lista de clientes (lado de leitura do CQRS).

→ O handler utiliza:
   - ICustomerReadOnlyRepository (leitura)
   - CustomerQueryModel (modelo de retorno)
   - Ardalis.Result para padronização da resposta

-------------------------------------------------------------------------------

✅ GetAll_Customer_ShouldReturnsSuccessResult
→ Cenário de sucesso com dados existentes.

🔹 Passos:
- Gera 4 clientes usando Bogus (dados fake realistas).
- Persiste os clientes no banco em memória via CustomerRepository (write side).
- Executa SaveChangesAsync para garantir persistência.
- Limpa o ChangeTracker para evitar cache do EF Core.
- Instancia o CustomerReadOnlyRepository (read side).
- Executa o handler com a query GetCustomersQuery.

🔹 Asserts:
- act.Should().NotBeNull()
  → Garante que o retorno não é nulo.

- act.IsSuccess.Should().BeTrue()
  → A operação deve ser bem-sucedida.

- act.Value.Should().BeAssignableTo<IReadOnlyList<CustomerQueryModel>>()
  → Valida que o retorno é uma coleção compatível com IReadOnlyList<CustomerQueryModel>.
  (List<T> implementa IReadOnlyList<T>, por isso usamos BeAssignableTo ao invés de BeOfType)

→ Esse teste valida o fluxo completo de leitura com dados existentes.

-------------------------------------------------------------------------------

✅ GetAll_Customer_Empty_ShouldReturnsSuccessResultWithEmptyList
→ Cenário de sucesso com base de dados vazia.

🔹 Passos:
- Remove todos os clientes do banco.
- Executa SaveChangesAsync para persistir a remoção.
- Limpa o ChangeTracker.
- Instancia o repositório de leitura.
- Executa o handler.

🔹 Asserts:
- act.Should().NotBeNull()
  → O retorno não deve ser nulo.

- act.IsSuccess.Should().BeTrue()
  → Mesmo sem dados, a operação deve ser considerada sucesso.

- act.Value.Should().BeAssignableTo<IReadOnlyList<CustomerQueryModel>>()
  → O retorno continua sendo uma coleção válida.

- act.Value.Should().BeEmpty()
  → Confirma que não há clientes retornados.

→ Esse teste garante que uma lista vazia não é tratada como erro.

-------------------------------------------------------------------------------

✅ Observação importante (FluentAssertions)
→ Foi necessário usar:
   BeAssignableTo<IReadOnlyList<CustomerQueryModel>>()

→ Porque:
   - O método retorna List<CustomerQueryModel>
   - List<T> implementa IReadOnlyList<T>
   - BeOfType<T>() exige tipo exato
   - BeAssignableTo<T>() aceita tipos compatíveis

-------------------------------------------------------------------------------

✅ Conclusão
→ Os testes garantem que:
   - O handler retorna sucesso com dados
   - O handler retorna sucesso sem dados
   - O tipo de retorno está correto
   - A separação CQRS (read side) está funcionando corretamente
*/