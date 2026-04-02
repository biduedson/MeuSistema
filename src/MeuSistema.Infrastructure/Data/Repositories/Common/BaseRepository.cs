using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Repositories.Common;

internal abstract class BaseRepository<TEntity, TKey>(AppDbContext dbContext)
    : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private static readonly Func<AppDbContext, TKey, Task<TEntity>> GetByIdCompiledAsync =
        EF.CompileAsyncQuery((AppDbContext dbcontext, TKey id) =>
            dbcontext
             .Set<TEntity>()
             .AsNoTrackingWithIdentityResolution()
             .FirstOrDefault(entity => entity.Id.Equals(id)))!;

    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
    protected readonly AppDbContext DbContext = dbContext;

    public async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await _dbSet.AsNoTrackingWithIdentityResolution().ToListAsync();

    public void Add(TEntity entity) =>
        _dbSet.Add(entity);

    public void Update(TEntity entity) =>
       _dbSet.Update(entity);

    public void Remove(TEntity entity) =>
       _dbSet.Remove(entity);

    public async Task<TEntity> GetByIdAsync(TKey id) =>
        await GetByIdCompiledAsync(DbContext, id);

    #region IDisposable

    private bool _disposed;
    ~BaseRepository() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            DbContext.Dispose();
        }

        _disposed = true;
    }

    #endregion
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. Declaração da classe  
   - `BaseRepository<TEntity, TKey>` é genérico e funciona para qualquer entidade (`TEntity`) com chave (`TKey`).  
   - Restrições: entidade deve implementar `IEntity<TKey>` e a chave precisa ser comparável (`IEquatable<TKey>`).  
   - O `AppDbContext` é injetado no construtor primário.

2. `GetByIdCompiledAsync`  
   - Usa `EF.CompileAsyncQuery` para compilar a consulta uma vez e reaproveitar, ganhando performance.  
   - `Set<TEntity>()` acessa o conjunto da entidade.  
   - `AsNoTrackingWithIdentityResolution()` retorna objetos sem rastreamento, mas garante que referências de navegação apontem para o mesmo objeto (evita duplicação).  
   - `FirstOrDefault` busca pela chave primária.

3. Campos internos  
   - `_dbSet` guarda o conjunto da entidade.  
   - `DbContext` é protegido para uso em classes derivadas.

4. Métodos CRUD  
   - `GetAllAsync()` → retorna todas as entidades sem tracking.  
   - `Add()` → adiciona entidade ao contexto.  
   - `Update()` → marca entidade como modificada.  
   - `Remove()` → remove entidade.  
   - `GetByIdAsync()` → busca entidade por Id usando a query compilada.

5. Implementação de `IDisposable`  
   - `_disposed` evita descarte duplicado.  
   - `~BaseRepository()` é o **destrutor** (finalizer).  
     - O símbolo `~` define um método especial chamado automaticamente pelo **Garbage Collector (GC)** quando o objeto é destruído.  
     - Aqui ele chama `Dispose(false)`, liberando apenas recursos **não gerenciados**, porque os gerenciados já serão tratados pelo GC.  
   - `Dispose()` libera recursos manualmente e chama `GC.SuppressFinalize(this)` para evitar que o GC rode o destrutor.  
   - `Dispose(bool disposing)` → se `true`, libera recursos gerenciados (ex.: `DbContext.Dispose()`); se `false`, apenas não gerenciados.

---

✅ Em resumo:  
Esse `BaseRepository` fornece operações básicas de CRUD, otimiza consultas com `CompileAsyncQuery`, usa `AsNoTrackingWithIdentityResolution` para consistência em consultas de leitura e implementa corretamente o padrão `IDisposable`.  
O destrutor (`~BaseRepository`) é a garantia final de que, mesmo se você esquecer de chamar `Dispose()`, o GC vai liberar os recursos não gerenciados.  
Assim, ele garante performance, organização e segurança no acesso a dados.
*/
