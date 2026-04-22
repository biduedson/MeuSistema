using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Repositories.Common;

internal abstract class BaseWriteOnlyRepository<TEntity, TKey>(AppDbContext dbContext)
    : IWriteRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private static readonly Func<AppDbContext, TKey, Task<TEntity?>> GetByIdCompiledAsync =
        EF.CompileAsyncQuery((AppDbContext dbContext, TKey id) =>
            dbContext
                .Set<TEntity>()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(entity => entity.Id.Equals(id)));

    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
    protected readonly AppDbContext DbContext = dbContext;

    public void Add(TEntity entity) =>
        _dbSet.Add(entity);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Remove(TEntity entity) =>
        _dbSet.Remove(entity);

    public async Task<TEntity?> GetByIdAsync(TKey id) =>
        await GetByIdCompiledAsync(DbContext, id);

    #region IDisposable

    private bool _disposed;
    ~BaseWriteOnlyRepository() => Dispose(false);

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
------------------------------------------------------------
🔹 EXPLICAÇÃO DETALHADA - BASE WRITE REPOSITORY 🔹
------------------------------------------------------------

✅ 1. Papel da classe

- `BaseWriteOnlyRepository<TEntity, TKey>` é a base genérica
  para repositórios de escrita.

- Ela centraliza o comportamento comum que qualquer agregado
  ou entidade pode reutilizar no lado de persistência.

------------------------------------------------------------

✅ 2. GetByIdCompiledAsync

- Essa é uma consulta compilada com `EF.CompileAsyncQuery`.

- Tradução literal:
  → consulta pré-compilada / pré-preparada para reuso.

- Objetivo:
  → deixar a busca por Id pronta para qualquer entidade
    que herde dessa base, evitando repetir a mesma estrutura
    de consulta em vários repositórios concretos.

- Como a busca por Id é uma operação genérica e recorrente,
  ela foi centralizada aqui para reaproveitamento.

------------------------------------------------------------

✅ 3. Por que usa AsNoTrackingWithIdentityResolution()

- `AsNoTracking`
  → busca sem rastreamento pelo EF Core.

- `WithIdentityResolution`
  → evita duplicação de instâncias em memória caso a mesma
    entidade apareça repetida em uma consulta mais complexa.

- No contexto desta base, a ideia é:
  → oferecer uma busca comum e leve por Id
    sem obrigar o tracking em todos os cenários.

👉 Se algum command ou caso de uso precisar da entidade rastreada,
   o repositório concreto da entidade deve expor um método específico
   para isso.

Exemplo:
- base genérica → GetByIdAsync sem tracking
- repositório concreto → GetTrackedByIdAsync com tracking

------------------------------------------------------------

✅ 4. Métodos de escrita

- `Add`
  → adiciona uma nova entidade ao contexto.

- `Update`
  → marca uma entidade como modificada.

- `Remove`
  → remove a entidade do contexto.

👉 A proposta aqui é que o fluxo comum de escrita use essas operações
   explícitas de persistência.

------------------------------------------------------------

✅ 5. GetByIdAsync

- Busca a entidade pelo identificador usando a consulta compilada.

- Mesmo estando no write repository, esse método não obriga tracking.
  Ele representa a busca genérica e reutilizável.

- Se o fluxo precisar de monitoramento automático de mudanças,
  o repositório específico da entidade pode oferecer uma consulta própria.

------------------------------------------------------------

✅ 6. Estratégia arquitetural adotada

Esse desenho segue a ideia de:

✔ a base genérica cobre o fluxo comum
✔ a entidade herda operações básicas prontas
✔ o repositório concreto só adiciona o que for específico
✔ tracking especial fica no repositório específico quando necessário

Exemplo:
- `CustomerRepository` pode ter `GetTrackedByIdAsync(Guid id)`
- `OrderRepository` pode ter consultas próprias do agregado
- a base continua enxuta e reaproveitável

------------------------------------------------------------

✅ 7. IDisposable

- A classe implementa `IDisposable`
  para permitir liberação explícita de recursos.

- `Dispose()`
  → libera recursos gerenciados.

- `~BaseWriteOnlyRepository()`
  → finalizer executado pelo GC caso o Dispose não seja chamado manualmente.

------------------------------------------------------------

✅ RESUMO FINAL

`BaseWriteOnlyRepository` foi desenhado para centralizar
o comportamento comum de escrita e a busca genérica por Id.

A busca por Id fica compilada e sem tracking por padrão,
favorecendo reaproveitamento e leveza.

Quando houver necessidade de entidade rastreada,
essa responsabilidade sobe para o repositório concreto da entidade.
------------------------------------------------------------
*/