

using Bogus;
using FluentAssertions;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.Entities.CustumerAggregate.Events;
using MeuSistema.Domain.ValueObjects;

namespace  MeuSistema.UnitTests.Domain.Entities.CustomerAggregate;

public class CustomerTests
{

    [Fact]
    public  void Should_CreateCustomerIsValid()
    {
        var customerFaker = new Faker<Customer>()
            .CustomInstantiator(faker => Customer.Create(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.PickRandom<EGender>(),
                faker.Internet.Email(),
                faker.Date.Past(30, DateTime.Now.AddYears(-18))
            ));
        var act = customerFaker.Generate();
        act.Should().NotBeNull();
        act.Id.Should().NotBeEmpty();
        act.FirstName.Should().NotBeNullOrWhiteSpace();
        act.LastName.Should().NotBeNullOrWhiteSpace();
        act.Email.Address.Should().NotBeNullOrWhiteSpace();
        act.DateOfBirth.Should().BeBefore(DateTime.Now.AddYears(-18));
    }

    [Fact]
    public void Should_ThrowValidationException_WhenInvalidData()
    {
        var customerFaker = new Faker<Customer>()
            .CustomInstantiator(faker => Customer.Create(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.PickRandom<EGender>(),
                faker.Internet.Email(),
                faker.Date.Past(30, DateTime.Now.AddYears(12))
            ));
        var act = () => customerFaker.Generate();

        act.Should().Throw<MeuSistema.Domain.Exceptions.ValidationException>()
            .WithMessage("O cliente deve ter pelo menos 18 anos.");
    }

    [Theory]
    [InlineData("", "Jose")]
    [InlineData("Maria", "")]
    [InlineData("", "")]
    public void Should_TrhowValidationException_WhenEmptyFistOrLastName(string firstName, string lastName)
    {
        
            Action act = () => Customer.Create(
                firstName,
                lastName,
                EGender.Male,
                "teste@email.com",             
                DateTime.Now.AddYears(-20)     
            );

            act.Should().Throw<MeuSistema.Domain.Exceptions.ValidationException>()
            .WithMessage("Nome e sobrenome não podem ser vazios.");
    }

    [Fact]
    public void Shoud_CustomerCreatedEvent_WhenCreate()
    {
        var customerFaker = new Faker<Customer>()
            .CustomInstantiator(faker => Customer.Create(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.PickRandom<EGender>(),
                faker.Internet.Email(),
                faker.Date.Past(30, DateTime.Now.AddYears(-18))
            ));

        var act = customerFaker.Generate();

       act.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<CustomerCreatedEvent>();

    }

    [Fact]
    public void Shoud_CustomerUpdatedEvent_WhenChangeEmail()
    {
        var customerEntity = new Faker<Customer>()
            .CustomInstantiator(faker => Customer.Create(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.PickRandom<EGender>(),
                faker.Internet.Email(),
                faker.Date.Past(30, DateTime.Now.AddYears(-18))
            ))
            .Generate();

        
        var newEmail = Email.Create(new Faker().Internet.Email());

       
        customerEntity.ChangeEmail(newEmail.Address);

       
        customerEntity.DomainEvents.Should()
            .ContainItemsAssignableTo<CustomerUpdatedEvent>();
    }

    [Fact]
    public void Shoud_CustomerDeletedEvent_WhenDelete()
    {
        var customerEntity = new Faker<Customer>()
            .CustomInstantiator(faker => Customer.Create(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.PickRandom<EGender>(),
                faker.Internet.Email(),
                faker.Date.Past(30, DateTime.Now.AddYears(-18))
            ))
            .Generate();
        
        customerEntity.Delete();
       
        customerEntity.DomainEvents.Should()
            .NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.ContainItemsAssignableTo<CustomerDeletedEvent>();
    }

}

/*
--------------------------------------------------------
📌 Guia dos testes e termos usados:

1. [Fact]
   - Indica que o método é um teste unitário simples.
   - É executado isoladamente e não recebe parâmetros.

2. [Theory] + [InlineData]
   - Permite rodar o mesmo teste com diferentes entradas.
   - Cada [InlineData] fornece um conjunto de parâmetros.
   - Útil para validar múltiplos cenários sem duplicar código.

3. Faker (Bogus)
   - Biblioteca para gerar dados falsos (nomes, emails, datas).
   - Usada para criar objetos válidos de forma automática.
   - Evita ter que escrever valores fixos em cada teste.

4. FluentAssertions
   - Biblioteca que deixa as asserções mais legíveis.
   - Exemplo: `act.Should().Throw<ValidationException>()`
     significa "a ação deve lançar uma exceção do tipo ValidationException".
   - O método `Should()` é um *extension method* que adiciona
     uma sintaxe fluente (quase como ler em português/inglês natural).

5. .Throw<T>()
   - Verifica se uma exceção do tipo T foi lançada.
   - Exemplo: `.Throw<ValidationException>()` garante que
     a regra de negócio disparou a exceção correta.

6. .WithMessage("...")
   - Além de verificar o tipo da exceção, também valida
     se a mensagem corresponde ao esperado.
   - Isso ajuda a garantir que a regra de negócio está clara
     e comunicando o motivo certo.

7. .NotBeNull(), .NotBeEmpty(), .NotBeNullOrWhiteSpace()
   - Asserções que garantem que valores não estão nulos,
     vazios ou apenas com espaços.
   - Usadas para validar integridade dos dados do objeto criado.

8. Eventos de domínio (CustomerCreatedEvent, CustomerUpdatedEvent, CustomerDeletedEvent)
   - São disparados pelo agregado `Customer` quando certas ações ocorrem.
   - Os testes verificam se esses eventos estão presentes em `DomainEvents`.
   - Isso garante que o comportamento do domínio está sendo seguido.

👉 Em resumo:
- `[Fact]` = teste único.
- `[Theory]` = teste parametrizado.
- `Faker` = dados falsos válidos.
- `FluentAssertions` = sintaxe fluente para validar resultados.
- `.Should()` = ponto de partida para todas as asserções.
- `.Throw<T>()` = espera exceção.
- `.WithMessage()` = valida mensagem da exceção.
- Eventos = comportamento do agregado além do estado interno.

--------------------------------------------------------
*/
