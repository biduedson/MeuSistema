

namespace MeuSistema.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task  SaveChangesAsync();
}
