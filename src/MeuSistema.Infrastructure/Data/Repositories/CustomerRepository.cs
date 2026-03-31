using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.ValueObjects;
using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.Infrastructure.Data.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Repositories;

internal class CustomerRepository(AppDbContext dbContext)
    : BaseRepository<Customer, Guid>(dbContext), ICustomerRepository
{
    private static readonly Func<AppDbContext, string, Task<bool>> ExistsByEmailCompiledAsync =
        EF.CompileAsyncQuery((AppDbContext dbContext, string email) =>
            dbContext
                .Customers
                .AsNoTracking()
                .Any(customer => customer.Email.Address == email));

    public Task<bool> ExistsByEmailAsync(Email email) =>
        ExistsByEmailCompiledAsync(DbContext, email.Address);
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. Declaração da classe  
   - `CustomerRepository` é um repositório específico para a entidade `Customer`.  
   - Ele herda de `BaseRepository<Customer, Guid>`, ou seja, já possui os métodos básicos de CRUD prontos.  
   - Implementa `ICustomerRepository`, garantindo que siga o contrato definido para operações de clientes.  
   - O `AppDbContext` é injetado no construtor primário.

2. `ExistsByEmailCompiledAsync`  
   - Usa `EF.CompileAsyncQuery` para compilar a consulta uma vez e reaproveitar, melhorando performance em chamadas repetitivas.  
   - `dbContext.Customers` acessa diretamente o conjunto de clientes.  
   - `AsNoTracking()` → consulta sem rastreamento, ideal para verificações simples de existência.  
   - `Any(customer => customer.Email.Address == email)` → retorna `true` se existir algum cliente com o e-mail informado.  

3. `ExistsByEmailAsync(Email email)`  
   - Método público que recebe um `ValueObject` `Email`.  
   - Internamente chama a query compilada (`ExistsByEmailCompiledAsync`) passando o `DbContext` e o endereço de e-mail.  
   - Retorna um `Task<bool>` indicando se o cliente existe ou não.

---

✅ Em resumo:  
Esse `CustomerRepository` é um repositório especializado que, além dos métodos básicos herdados do `BaseRepository`, adiciona uma operação específica: verificar se já existe um cliente com determinado e-mail.  
O uso de `EF.CompileAsyncQuery` garante performance, e `AsNoTracking` evita overhead desnecessário, já que não há necessidade de rastrear entidades em uma simples verificação de existência.
*/
