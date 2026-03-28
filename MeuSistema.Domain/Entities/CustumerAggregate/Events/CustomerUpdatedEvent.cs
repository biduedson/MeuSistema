

namespace MeuSistema.Domain.Entities.CustumerAggregate.Events
{
    public class CustomerUpdatedEvent(
        Guid id,
        string firstName,
        string lastName,  
        EGender gender,
        string email,
        DateTime dateOfBirth
        ) : CustomerBaseEvent(id, firstName,lastName,gender,email,dateOfBirth);
   
}
