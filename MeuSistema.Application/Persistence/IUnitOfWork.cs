using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeuSistema.Application.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}