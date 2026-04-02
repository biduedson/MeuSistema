
using MeuSistema.Domain.ValueObjects;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<bool> ExistsByEmailAsync(Email email);
}