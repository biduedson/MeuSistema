using System;

namespace MeuSistema.Domain.Shared.Primitives;

public interface IUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
}