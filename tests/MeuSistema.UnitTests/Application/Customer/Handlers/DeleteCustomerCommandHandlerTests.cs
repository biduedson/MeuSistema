

using Bogus;
using FluentAssertions;
using MediatR;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Application.Customer.Handlers;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data;
using MeuSistema.Infrastructure.Data.Repositories;
using MeuSistema.SharedKernel.Primitives;
using MeuSistema.UnitTests.Fixtures;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NSubstitute;
using Xunit.Categories;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class DeleteCustomerCommandHandlerTests(EfSqliteFixture fixture) : IClassFixture<EfSqliteFixture>
{
    private readonly DeleteCustomerCommandValidator _validator = new();

    [Fact]
    public async Task Delete_ValidCustomerId_ShoudReturnSucessResult()
    {
        var customer = new Faker<MeuSistema.Domain.Entities.CustumerAggregate.Customer>()
            .CustomInstantiator(faker => MeuSistema.Domain.Entities.CustumerAggregate.Customer.Create(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.PickRandom<EGender>(),
                faker.Person.Email,
                faker.Person.DateOfBirth
                ))
            .Generate();

        var repositoty = new CustomerRepository(fixture.Context);

        repositoty.Add(customer);

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Clear();

        var unitOfWork = new UnitOfWork(
            fixture.Context,
            Substitute.For<IEventStoreRepository>(),
            Substitute.For<IMediator>(),
            Substitute.For<ILogger<UnitOfWork>>()
            );


        var handler = new DeleteCustomerCommandHandler(
            _validator,
            new CustomerRepository(fixture.Context),
            unitOfWork
            );

        var command = new DeleteCustomerCommand(customer.Id);

        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.SuccessMessage.Should().Be("Customer deletado com sucesso");
    }

    [Fact]
    public async Task Delete_NotFoundCustomer_ShouldReturnsFailResu()
    {
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        var handler = new DeleteCustomerCommandHandler(
            _validator,
            new CustomerRepository(fixture.Context),
            Substitute.For<IUnitOfWork>()
            );

        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == $"Nenhum customer encontrado com o id: {command.Id}");
    }

    [Fact]
    public async Task Delete_InvalidCommand_ShouldReturnsFailResult()
    {
        var handler = new DeleteCustomerCommandHandler(
            _validator,
            Substitute.For<ICustomerRepository>(),
            Substitute.For<IUnitOfWork>());

        var act = await handler.Handle(new DeleteCustomerCommand(Guid.Empty), CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();

    }
}

// -----------------------------------------------------------------------------
// Explicação detalhada dos testes de DeleteCustomerCommandHandler:
//
// • Delete_ValidCustomerId_ShoudReturnSucessResult
//   - Cenário feliz: cria um cliente válido, salva no banco em memória,
//     e depois tenta deletar.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeTrue(); → a operação deve ter sido bem-sucedida.
//       act.SuccessMessage.Should().Be("Customer deletado com sucesso");
//         → a mensagem de sucesso deve ser exatamente essa.
//
// • Delete_NotFoundCustomer_ShouldReturnsFailResu
//   - Cenário de erro de negócio: tenta deletar um cliente que não existe.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.Errors.Should().NotBeNullOrEmpty() → deve haver mensagens de erro.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//       .And.Contain(errorMessage => errorMessage == $"Nenhum customer encontrado com o id: {command.Id}")
//         → deve conter exatamente a mensagem informando que o ID não foi encontrado.
//
// • Delete_InvalidCommand_ShouldReturnsFailResult
//   - Cenário de erro de validação: o comando é inválido (Guid.Empty).
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.ValidationErrors.Should().NotBeNullOrEmpty() → deve haver erros de validação.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//
// -----------------------------------------------------------------------------
// Em resumo:
// - O primeiro teste cobre SUCESSO (cliente válido deletado).
// - O segundo cobre ERRO DE NEGÓCIO (cliente não encontrado).
// - O terceiro cobre ERRO DE VALIDAÇÃO (comando inválido).
// -----------------------------------------------------------------------------
