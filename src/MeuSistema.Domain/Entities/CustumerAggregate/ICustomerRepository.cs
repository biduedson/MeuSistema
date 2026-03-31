using System;
using System.Threading.Tasks;
using MeuSistema.Domain.Shared.Primitives;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> GetByEmailAsync(string email);
}