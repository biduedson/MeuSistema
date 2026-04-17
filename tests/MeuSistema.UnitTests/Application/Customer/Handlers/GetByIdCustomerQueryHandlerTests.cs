

using Bogus;
using FluentAssertions;
using MediatR;
using MeuSistema.Application.Customer.Queries.GetCustomers;
using MeuSistema.Application.Customer.Responses;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data;
using MeuSistema.Infrastructure.Data.Repositories;
using MeuSistema.SharedKernel.Primitives;
using MeuSistema.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Categories;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class GetByIdCustomerQueryHandlerTests(EfSqliteFixture fixture) : IClassFixture<EfSqliteFixture>
{
    private readonly GetByIdCustomerQueryValidator _validator = new();

    [Fact]
    public async Task Get_CustomerById_ShouldReturnsSuccessResult()
    {
        var customer = new Faker<MeuSistema.Domain.Entities.CustumerAggregate.Customer>()
          .CustomInstantiator(faker => MeuSistema.Domain.Entities.CustumerAggregate.Customer.Create(
              faker.Person.FirstName,
              faker.Person.LastName,
              faker.PickRandom<EGender>(),
              faker.Person.Email,
              faker.Person.DateOfBirth))
          .Generate();

        var repository = new CustomerRepository(fixture.Context);
        repository.Add(customer);

        var unitOfWork = new UnitOfWork(
           fixture.Context,
           Substitute.For<IEventStoreRepository>(),
           Substitute.For<IMediator>(),
           Substitute.For<ILogger<UnitOfWork>>());

        await fixture.Context.SaveChangesAsync();   
        fixture.Context.ChangeTracker.Clear();

        var queryHandler = new GetByIdCustomerQueryHandler(
            _validator,
            new CustomerRepository(fixture.Context));

        var query = new GetByIdCustomerQuery(customer.Id);


        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<GetByIdCustomerResponse>();

    }

    [Fact]
    public async Task Get_CustomerById_NotFound_ShouldReturnsFailResult()
    {
       
        var query = new GetByIdCustomerQuery(Guid.NewGuid());

        var queryHandler = new GetByIdCustomerQueryHandler(
            _validator,
            new CustomerRepository(fixture.Context)
            );
        var act = await queryHandler.Handle(query, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado com o id {query.Id}");
    }
    [Fact]
    public async Task Get_CustomerById_InvalidId_ShouldReturnsFailResult()
    {
        var queryHandler = new GetByIdCustomerQueryHandler(
            _validator,
           Substitute.For<ICustomerRepository>()
            );
        var act = await queryHandler.Handle(new GetByIdCustomerQuery(Guid.Empty), CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }
}


// -----------------------------------------------------------------------------
// Explicação detalhada dos testes de GetByIdCustomerQueryHandler:
//
// • Get_CustomerById_ShouldReturnsSuccessResult
//   - Cenário feliz: cria e salva um cliente válido no banco em memória,
//     e depois consulta pelo Id.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeTrue(); → a operação deve ter sido bem-sucedida.
//       act.Value.Should().BeOfType<GetByIdCustomerResponse>();
//         → o objeto retornado deve ser do tipo GetByIdCustomerResponse,
//           garantindo que veio a resposta completa.
//
// • Get_CustomerById_NotFound_ShouldReturnsFailResult
//   - Cenário de erro de negócio: tenta buscar um cliente que não existe.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.Errors.Should().NotBeNullOrEmpty() → deve haver mensagens de erro.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//       .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado com o id {query.Id}")
//         → deve conter exatamente a mensagem informando que o ID não foi encontrado.
//
// • Get_CustomerById_InvalidId_ShouldReturnsFailResult
//   - Cenário de erro de validação: o comando é inválido (Id vazio).
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.ValidationErrors.Should().NotBeNullOrEmpty() → deve haver erros de validação.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//
// -----------------------------------------------------------------------------
// Em resumo:
// - O primeiro teste cobre SUCESSO (cliente encontrado).
// - O segundo cobre ERRO DE NEGÓCIO (cliente não encontrado).
// - O terceiro cobre ERRO DE VALIDAÇÃO (Id inválido).
// -----------------------------------------------------------------------------
