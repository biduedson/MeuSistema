

namespace MeuSistema.SharedKernel.Primitives;

public interface IUnitOfWork : IDisposable
{
    Task  SaveChangesAsync();
}
