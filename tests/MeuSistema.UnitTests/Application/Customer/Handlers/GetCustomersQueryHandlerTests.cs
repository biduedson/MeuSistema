

using Bogus;
using FluentAssertions;
using MeuSistema.Application.Customer.Queries.GetCustomers;
using MeuSistema.Application.Customer.Responses;
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

        await  fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var queryHandler = new GetCustomersQueryHandler(
            repository
            );

        var query = new GetCustomersQuery();
        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<GetCustomersResponse>();

    }

    [Fact]
    public async Task GetAll_Customer_Empty_ShouldReturnsSuccessResultWithEmptyList()
    {
        var repository = new CustomerRepository(fixture.Context);

        fixture.Context.Customers.RemoveRange(fixture.Context.Customers);

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Clear();

        var queryHandler = new GetCustomersQueryHandler(repository);
        var query = new GetCustomersQuery();

        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<GetCustomersResponse>();
        act.Value.Customers.Should().BeEmpty(); 
    }

}

}

// -----------------------------------------------------------------------------
// Explicação detalhada dos testes de GetCustomersQueryHandler:
//
// • GetAll_Customer_ShouldReturnsSuccessResult
//   - Cenário feliz: cria 4 clientes com Bogus, adiciona no repositório e salva.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeTrue(); → a operação deve ter sido bem-sucedida.
//       act.Value.Should().BeOfType<GetCustomersResponse>();
//         → o objeto retornado deve ser do tipo GetCustomersResponse,
//           garantindo que veio a resposta correta com a lista de clientes.
//
// • GetAll_Customer_Empty_ShouldReturnsSuccessResultWithEmptyList
//   - Cenário de lista vazia: remove todos os clientes do contexto antes de executar.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeTrue(); → a operação deve ter sido bem-sucedida,
//         mesmo sem clientes.
//       act.Value.Should().BeOfType<GetCustomersResponse>();
//         → o objeto retornado deve ser do tipo GetCustomersResponse.
//       act.Value.Customers.Should().BeEmpty();
//         → a lista de clientes deve estar vazia, confirmando que não há registros.
//
// -----------------------------------------------------------------------------
// Em resumo:
// - O primeiro teste cobre SUCESSO com clientes existentes.
// - O segundo cobre SUCESSO com lista vazia (sem clientes).
// -----------------------------------------------------------------------------
