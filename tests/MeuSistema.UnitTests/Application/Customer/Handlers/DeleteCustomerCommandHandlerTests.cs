

using System.Runtime.InteropServices;
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
using Microsoft.VisualBasic;
using NSubstitute;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

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

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(error => error.ErrorMessage == "O ID do cliente é obrigatório.");
            
    }
}
