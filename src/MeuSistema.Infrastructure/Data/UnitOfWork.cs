using System.Data;
using MediatR;
using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.SharedKernel.Extensions;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeuSistema.Infrastructure.Data;

internal sealed class UnitOfWork(
    AppDbContext context,
    IEventStoreRepository eventStoreRepository,
    IMediator mediator,
    ILogger<UnitOfWork> logger
) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        logger.LogInformation("----- Iniciando transação: '{TransactionId}'", transaction.TransactionId);

        try
        {
            var (domainEvents, eventStores) = BeforeSaveChanges();

            var rowsAffected = await context.SaveChangesAsync();

            logger.LogInformation("----- Commit da transação: '{TransactionId}'", transaction.TransactionId);

            await AfterSaveChangesAsync(domainEvents, eventStores);

            logger.LogInformation(
                "----- Transação confirmada com sucesso: '{TransactionId}', Linhas afetadas: {RowsAffected}",
                transaction.TransactionId,
                rowsAffected);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu uma exceção inesperada ao confirmar a transação: '{TransactionId}', mensagem: {Message}",
                transaction.TransactionId,
                ex.Message);

            await transaction.RollbackAsync();
            throw;
        }
    }

    private (IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore> eventStores) BeforeSaveChanges()
    {
        var domainEntities = context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        var eventStores = domainEvents
            .ConvertAll(@event => new EventStore(@event.AggregatedId, @event.GetGenericTypeName(), @event.ToJson()));

        domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

        return (domainEvents.AsReadOnly(), eventStores.AsReadOnly());
    }

    private async Task AfterSaveChangesAsync(
        IReadOnlyList<BaseEvent> domainEvents,
        IReadOnlyList<EventStore> eventStores)
    {
        if (domainEvents.Count > 0)
            await Task.WhenAll(domainEvents.Select(@event => mediator.Publish(@event)));

        if (domainEvents.Count > 0)
            await eventStoreRepository.StoreAsync(eventStores);
    }

    #region IDisposable

    private bool _disposed;

    ~UnitOfWork() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            context.Dispose();
            eventStoreRepository.Dispose();
        }

        _disposed = true;
    }

    #endregion
}

/*
📌 Explicação geral:

- **UnitOfWork**: implementa o padrão "Unidade de Trabalho". Ele garante que salvar dados e disparar eventos aconteça dentro de uma transação consistente.

- **SaveChangesAsync**:
  - Abre uma transação (`BeginTransactionAsync`).
  - Coleta eventos de domínio antes de salvar (`BeforeSaveChanges`).
  - Salva alterações no banco (`context.SaveChangesAsync`).
  - Publica e persiste eventos depois (`AfterSaveChangesAsync`).
  - Se tudo der certo → commit. Se der erro → rollback.

- **BeforeSaveChanges**:
  - Usa o `ChangeTracker` do EF Core para encontrar entidades que geraram eventos (`DomainEvents`).
  - Extrai todos os eventos dessas entidades.
  - Converte em `EventStore` (objeto pronto para persistência, com ID agregado, tipo e JSON).
  - Limpa os eventos das entidades para não duplicar.
  - Retorna uma **tupla** `(domainEvents, eventStores)`.

- **AfterSaveChangesAsync**:
  - Recebe duas listas:
    - `IReadOnlyList<BaseEvent> domainEvents`: lista de eventos de domínio.  
      → `IReadOnlyList` foi usado para garantir **imutabilidade** (não pode adicionar/remover eventos depois).
    - `IReadOnlyList<EventStore> eventStores`: lista de eventos já preparados para serem gravados no banco.
  - Publica os eventos via `mediator.Publish` (dispara para handlers interessados).
  - Persiste os eventos no `eventStoreRepository` (histórico/auditoria).

- **IDisposable**:
  - Implementa o padrão Dispose para liberar recursos (DbContext e EventStoreRepository).
  - Usa `_disposed` para evitar liberar duas vezes.
  - `GC.SuppressFinalize` evita que o coletor de lixo chame o destrutor novamente.

📌 Em resumo:
Esse `UnitOfWork` garante que:
1. Alterações no banco sejam feitas de forma transacional.
2. Eventos de domínio sejam coletados antes, publicados e persistidos depois.
3. Recursos sejam liberados corretamente.
4. Tudo seja registrado em log para rastreamento.
*/
