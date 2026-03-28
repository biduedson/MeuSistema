

using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.Domain.Factories
{
    public static class CustomerFactory
    {
        public static DomainResult<Customer> Create(
            string firstName,
            string lastName,
            EGender gender,
            string email,
            DateTime dateOfBirth)
        {
          var emailResult = Email.Create(email);
            return !emailResult.IsSuccess
                 ? DomainResult<Customer>.Failure(emailResult.Error!)
                 : DomainResult<Customer>.Success(new Customer(firstName, lastName, gender, emailResult.Value!, dateOfBirth));
                    
        }

        public static Customer Create(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
             => new(firstName, lastName, gender, email, dateOfBirth);
    }
}
