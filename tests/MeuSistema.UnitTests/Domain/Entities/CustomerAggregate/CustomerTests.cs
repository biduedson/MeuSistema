using Bogus;
using FluentAssertions;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.Entities.CustumerAggregate.Events;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.UnitTests.Domain.Entities.CustomerAggregate;

public class CustomerTests
{
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

        var act= customerFaker.Generate();

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
