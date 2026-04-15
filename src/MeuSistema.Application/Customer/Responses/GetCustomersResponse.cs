

using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Customer.Responses;

public class GetCustomersResponse(IEnumerable<CustomerQueryModel> customers) : IResponse
{
    public IEnumerable<CustomerQueryModel> Customers { get; } = customers;
}
