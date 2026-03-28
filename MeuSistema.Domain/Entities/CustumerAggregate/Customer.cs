
using MeuSistema.Domain.Entities.CustumerAggregate.Events;
using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public class Customer : BaseEntity, IAggregateRoot
{

    private bool _isDeleted;
    public Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;

        AddDomainEvent(new CustomerCreatedEvent(Id, firstName, lastName, gender, email.Address, dateOfBirth));
    }

    public Customer() { }

    public string FirstName { get; }
    public string LastName { get; }
    public EGender Gender { get; }
    public Email Email { get; private set; }
    public DateTime DateOfBirth { get; }

    public void ChangeEmail(Email newEmail)
    {
        if (Email.Equals(newEmail)) return;
        Email = newEmail;
        AddDomainEvent(new CustomerUpdatedEvent(Id, FirstName,LastName,Gender, newEmail.Address, DateOfBirth));
    }

    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;

        AddDomainEvent(new CustomerDeletedEvent(Id, FirstName,LastName,Gender,Email.Address,DateOfBirth));
    }

}
  
