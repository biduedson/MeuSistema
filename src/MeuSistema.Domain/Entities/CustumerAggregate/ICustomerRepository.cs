
using MeuSistema.Domain.ValueObjects;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Domain.Entities.CustumerAggregate;

public interface ICustomerRepository : IWriteRepository<Customer, Guid>
{
    Task<bool> ExistsByEmailAsync(Email email);

    Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
}


// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Interface ICustomerWriteOnlyRepository → Define um repositório de escrita para a entidade Customer. 
✅ Herança de IRepository<Customer, Guid> → Garante que apenas operações de escrita são permitidas. 
✅ Método ExistsByEmailAsync(Email email) → Verifica se um cliente já existe baseado no e-mail. 
✅ Método ExistsByEmailAsync(Email email, Guid currentId) → Faz a mesma verificação, mas exclui o cliente atual da busca, útil para atualizações. 
*/
