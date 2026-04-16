

using MeuSistema.Infrastructure.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.UnitTests.Fixtures;

public class EfSqliteFixture : IAsyncLifetime, IDisposable
{
    private const string ConnectionString = "Data Source=:memory";
    private readonly SqliteConnection _connection;
    public AppDbContext Context { get; }

    public EfSqliteFixture()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        var builder = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(ConnectionString);
        Context = new AppDbContext(builder.Options); 

    }

    #region IAsyncLifeTime

    public async Task InitializeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();     
    }

    public  Task DisposeAsync() => Task.CompletedTask;

    #endregion

    #region IDisposable

    private bool _disposed;

    ~EfSqliteFixture() => Dispose(false);

    public  void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if(disposing)
        {
            _connection.Dispose();
            Context?.Dispose();
        }

        _disposed = true;
    }
    #endregion
}
