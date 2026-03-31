using System;
using System.Threading.Tasks;
using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<bool> ExistsByEmailAsync(Email email);
}