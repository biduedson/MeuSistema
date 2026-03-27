using System;
using System.Reflection;
using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public class Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth) : BaseEntity, IAggregateRoot
{
    private bool _isDeleted;

    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public EGender Gender { get; } = gender;
    public Email Email { get; private set; } = email;
    public DateTime DateOfBirth { get; } = dateOfBirth;


    public void ChangeEmail(Email email)
    {
        if(Email.Equals(email)) return;

        Email = email;
    }
}