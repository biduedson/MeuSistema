

using Ardalis.Result;
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
using NSubstitute;
using Xunit.Categories;

namespace MeuSistema.UnitTests.Application.Customer.Handlers;

[UnitTest]
public class CreateCustomerCommandHandlerTests(EfSqliteFixture fixture) : IClassFixture<EfSqliteFixture>
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public async Task Add_ValidCommand_ShouldReturnsCreatedResult()
    {

        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .RuleFor(command => command.BirthDate, faker => faker.Person.DateOfBirth)
            .Generate();

        var unitOfWork = new UnitOfWork(
            fixture.Context,
            Substitute.For<IEventStoreRepository>(),
            Substitute.For<IMediator>(),
            Substitute.For<ILogger<UnitOfWork>>());

        var handler = new CreateCustomerCommandHandler(
        _validator,
        new CustomerRepository (fixture.Context),
        unitOfWork);

        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsCreated().Should().BeTrue();
        act.Value.Should().NotBeNull();
        act.Value.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Add_DuplicateEmailCommand_ShouldReturnsFailResult()
    {
        var command = new Faker<CreateCustomerCommand>()
            .RuleFor(command => command.FirstName, faker => faker.Person.FirstName)
            .RuleFor(command => command.LastName, faker => faker.Person.LastName)
            .RuleFor(command => command.Gender, faker => faker.PickRandom<EGender>())
            .RuleFor(command => command.Email, faker => faker.Person.Email.ToLowerInvariant())
            .RuleFor(command => command.BirthDate, faker => faker.Person.DateOfBirth)
            .Generate();

        var repository = new CustomerRepository(fixture.Context);
        var customer = MeuSistema.Domain.Entities.CustumerAggregate.Customer.Create(
            command.FirstName,
            command.LastName,
            command.Gender,
            command.Email,
            command.BirthDate);

        repository.Add(customer);

        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var handler = new CreateCustomerCommandHandler(
            _validator,
            repository,
            Substitute.For<IUnitOfWork>());

        var act = await handler.Handle(command, CancellationToken.None);

        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Errors.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.Contain(errorMessage => errorMessage == "O endereço de e-mail informado já está em uso.");

    }

    [Fact]
    public async Task Add_InvalidCommand_ShouldReturnsFailResult()
    {
        
        var handler = new CreateCustomerCommandHandler(
            _validator,
            Substitute.For<ICustomerRepository>(),
            Substitute.For<IUnitOfWork>());

        
        var act = await handler.Handle(new CreateCustomerCommand(), CancellationToken.None);

      
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.ValidationErrors.Should().NotBeNullOrEmpty().And.OnlyHaveUniqueItems();
    }
}

// -----------------------------------------
// 🔹 COMENTÁRIOS EXPLICATIVOS 🔹
// -----------------------------------------

/*
Bibliotecas usadas:
- Ardalis.Result → fornece um tipo de retorno padronizado (Success, Fail, Created).
- Bogus → gera dados falsos realistas (nomes, e-mails, datas).
- FluentAssertions → facilita escrever asserções nos testes com sintaxe fluida.
- MediatR → usado para publicar eventos de domínio.
- NSubstitute → cria objetos falsos (mocks) para testes.
- Xunit.Categories → categoriza testes (ex: [UnitTest]).

Classe de teste:
- CreateCustomerCommandHandlerTests → testa o comportamento do handler que cria clientes.
- Usa EfSqliteFixture → cria um banco SQLite em memória para simular persistência.

Teste 1: Add_ValidCommand_ShouldReturnsCreatedResult
- Cria um comando válido com dados falsos (Bogus).
- Instancia UnitOfWork e CustomerRepository.
- Executa o handler.
- Verifica que o resultado não é nulo, foi criado com sucesso e o cliente tem Id válido.

Teste 2: Add_DuplicateEmailCommand_ShouldReturnsFailResult
- Cria um comando com e-mail falso.
- Adiciona manualmente um cliente com o mesmo e-mail no banco.
- Salva e limpa o ChangeTracker.
- Executa o handler com o comando duplicado.
- Verifica que o resultado falhou, retornou erros e contém a mensagem "O endereço de e-mail informado já está em uso."

Teste 3: Add_InvalidCommand_ShouldReturnsFailResult
- Cria o handler com mocks.
- Executa o handler com um comando vazio (sem dados).
- Verifica que o resultado falhou e retornou erros de validação.

Resumo:
- Esses testes garantem que o handler de criação de clientes:
  1. Cria corretamente quando os dados são válidos.
  2. Bloqueia criação quando o e-mail já existe.
  3. Retorna falha quando os dados são inválidos.
*/
