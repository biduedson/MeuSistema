using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Abstractions;

public  interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel, Guid>
{
    Task<IReadOnlyList<CustomerQueryModel>> GetAllAsync();
}
