
using Bogus;
using Castle.Core.Logging;
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
using NSubstitute;
using Xunit.Categories;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class UpdateCustomerCommandHandlerTest(EfSqliteFixture fixture) : IClassFixture<EfSqliteFixture>
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public async Task Update_ValidCommand_ShouldReturnsSuccessResult()
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

        await  fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var unitOfwork = new UnitOfWork(
            fixture.Context,
            Substitute.For<IEventStoreRepository>(),
            Substitute.For<IMediator>(),
            Substitute.For<ILogger<UnitOfWork>>());
        
        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customer.Id)
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .Generate();

        var handler = new UpdateCustomerCommandHandler(
            _validator,
            repository,
            unitOfwork
            );

        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.SuccessMessage.Should().Be("Email do cliente atualizado com sucesso.");
                
    }

    [Fact]
    public async Task Update_DuplicateEmailCommand_ShouldReturnsFailResult()
    {
        var customers= new Faker<MeuSistema.Domain.Entities.CustumerAggregate.Customer>()
               .CustomInstantiator(faker => MeuSistema.Domain.Entities.CustumerAggregate.Customer.Create(
                   faker.Person.FirstName,
                   faker.Person.LastName,
                   faker.PickRandom<EGender>(),
                   faker.Person.Email,
                   faker.Person.DateOfBirth))
               .Generate(2);

        var repository = new CustomerRepository(fixture.Context);

        foreach(var customer in customers)
        {
            repository.Add(customer);
        }

        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, customers[0].Id)
            .RuleFor(command => command.Email, customers[1].Email.Address)
            .Generate();

        var handler = new UpdateCustomerCommandHandler(
           _validator,
            repository,
            Substitute.For<IUnitOfWork>());
        
        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == "Já existe um cliente cadastrado com esse email.");
    }

    [Fact]
    public async Task Update_NotFoundCustomer_ShouldReturnsFailResult()
    {
        var command = new Faker<UpdateCustomerCommand>()
            .RuleFor(command => command.Id, faker => faker.Random.Guid())
            .RuleFor(command => command.Email, Faker => Faker.Person.Email)
            .Generate();

        var handler = new UpdateCustomerCommandHandler(
            _validator,
            new CustomerRepository(fixture.Context),
            Substitute.For<IUnitOfWork>());
            
         var act = await handler.Handle(command,CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado com o Id: {command.Id}");

    }

    [Fact]
    public async Task Update_InvalidCommand_ShouldReturnsFailResult()
    {
        // Arrange
        var handler = new UpdateCustomerCommandHandler(
             new UpdateCustomerCommandValidator(),
             Substitute.For<ICustomerRepository>(),
             Substitute.For<IUnitOfWork>());

        // Act
        var act = await handler.Handle(new UpdateCustomerCommand(), CancellationToken.None);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }

}


// -----------------------------------------------------------------------------
// Explicação detalhada dos testes de UpdateCustomerCommandHandler:
//
// • Update_ValidCommand_ShouldReturnsSuccessResult
//   - Cenário feliz: cria um cliente válido, salva no banco em memória,
//     e depois atualiza o email.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeTrue(); → a operação deve ter sido bem-sucedida.
//       act.SuccessMessage.Should().Be("Email do cliente atualizado com sucesso.");
//         → a mensagem de sucesso deve ser exatamente essa.
//
// • Update_DuplicateEmailCommand_ShouldReturnsFailResult
//   - Cenário de erro de negócio: tenta atualizar o email de um cliente
//     para um email que já pertence a outro cliente.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.Errors.Should().NotBeNullOrEmpty() → deve haver mensagens de erro.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//       .And.Contain(errorMessage => errorMessage == "Já existe um cliente cadastrado com esse email.");
//         → deve conter exatamente essa mensagem de erro.
//
// • Update_NotFoundCustomer_ShouldReturnsFailResult
//   - Cenário de erro de negócio: tenta atualizar um cliente que não existe.
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.Errors.Should().NotBeNullOrEmpty() → deve haver mensagens de erro.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//       .And.Contain(errorMessage => errorMessage == $"Nenhum cliente encontrado com o Id: {command.Id}")
//         → deve conter exatamente a mensagem informando que o ID não foi encontrado.
//
// • Update_InvalidCommand_ShouldReturnsFailResult
//   - Cenário de erro de validação: o comando é inválido (sem Id, sem Email).
//   - Asserts:
//       act.Should().NotBeNull(); → o resultado não deve ser nulo.
//       act.IsSuccess.Should().BeFalse(); → a operação deve ter falhado.
//       act.ValidationErrors.Should().NotBeNullOrEmpty() → deve haver erros de validação.
//       .And.OnlyHaveUniqueItems() → não pode haver mensagens duplicadas.
//
// -----------------------------------------------------------------------------
// Em resumo:
// - O primeiro teste cobre SUCESSO (cliente válido atualizado).
// - O segundo cobre ERRO DE NEGÓCIO (email duplicado).
// - O terceiro cobre ERRO DE NEGÓCIO (cliente não encontrado).
// - O quarto cobre ERRO DE VALIDAÇÃO (comando inválido).
// -----------------------------------------------------------------------------
