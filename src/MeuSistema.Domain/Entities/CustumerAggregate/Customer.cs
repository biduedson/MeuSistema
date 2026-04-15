
using MeuSistema.Domain.Entities.CustumerAggregate.Events;
using MeuSistema.Domain.Exceptions;
using MeuSistema.Domain.ValueObjects;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public class Customer : BaseEntity, IAggregateRoot
{

    private bool _isDeleted;
    private Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
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


    public static Customer Create(string firstName, string lastName, EGender gender, string email, DateTime dateOfBirth)
    {
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ValidationException("Nome e sobrenome não podem ser vazios.");

            var emailCreated = Email.Create(email);

            if (dateOfBirth > DateTime.Now.AddYears(-18))
                throw new ValidationException("O cliente deve ter pelo menos 18 anos.");

            return new Customer(firstName, lastName, gender, emailCreated, dateOfBirth);
        }

    }

    public void ChangeEmail(string newEmail)
    {
        if (Email.Address.Equals(newEmail, StringComparison.OrdinalIgnoreCase)) return;

        
        Email = Email.Create(newEmail);

        AddDomainEvent(new CustomerUpdatedEvent(Id, FirstName, LastName, Gender, Email.Address, DateOfBirth));
    }
    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;

        AddDomainEvent(new CustomerDeletedEvent(Id, FirstName,LastName,Gender,Email.Address,DateOfBirth));
    }

}
  
