
using MeuSistema.Application.Abstractions;
using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Repositories;

internal class CustomerReadOnlyRepository(AppDbContext dbContext) 
     :  ICustomerReadOnlyRepository
{
    public async Task<CustomerQueryModel?> GetByIdAsync(Guid id) =>
       await dbContext.Customers
           .AsNoTracking()
           .Where(customer => customer.Id == id)
           .Select(customer => new CustomerQueryModel(
               customer.Id,
               customer.FirstName,
               customer.LastName,
               customer.Gender.ToString(),
               customer.Email.Address,
               customer.DateOfBirth))
           .FirstOrDefaultAsync();

    public async Task<IReadOnlyList<CustomerQueryModel>> GetAllAsync() =>
        await dbContext.Customers
        .AsNoTracking()
        .Select(customer => new CustomerQueryModel(
              customer.Id,
               customer.FirstName,
               customer.LastName,
               customer.Gender.ToString(),
               customer.Email.Address,
               customer.DateOfBirth
            )).ToListAsync();
}
