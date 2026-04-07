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

        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {

            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            logger.LogInformation("----- Iniciando transação: '{TransactionId}'", transaction.TransactionId);

            try
            {
                var (domainEvents, eventStores) = BeforeSaveChanges();

                var rowsAffected = await context.SaveChangesAsync();

                logger.LogInformation("----- Commit da transação: '{TransactionId}'", transaction.TransactionId);

                await transaction.CommitAsync();

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
        });
       
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



// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe implementa o padrão Unit of Work.
// O objetivo é centralizar a confirmação das alterações no banco,
// garantindo consistência transacional e controle dos eventos de domínio.
//
// 1) O que é a estratégia usada aqui
// → A estratégia usada é a Execution Strategy do EF Core.
// → Ela é obtida com:
//    context.Database.CreateExecutionStrategy()
// → No PostgreSQL com EnableRetryOnFailure, essa estratégia costuma ser
//   a NpgsqlRetryingExecutionStrategy.
//
// 2) Para que serve essa estratégia
// → Serve para reexecutar automaticamente operações que falham por erros transitórios.
// → Exemplos:
//    - falha momentânea de conexão
//    - timeout temporário
//    - indisponibilidade breve do banco
//
// 3) Por que ela foi necessária aqui
// → Porque o DbContext foi configurado com retry automático no provider do PostgreSQL.
// → Quando retry automático está habilitado, o EF Core não aceita transações
//   iniciadas manualmente fora da execution strategy.
// → Por isso, toda a operação transacional precisa ficar dentro de:
//    strategy.ExecuteAsync(...)
//
// 4) Relação entre a strategy e a transação
// → A execution strategy precisa controlar o bloco inteiro que pode ser repetido.
// → Isso inclui:
//    - abrir a transação
//    - salvar no banco
//    - executar ações complementares
//    - confirmar ou desfazer a transação
// → Em outras palavras:
//    retry e transação precisam estar no mesmo bloco controlado pela strategy.
//
// 5) SaveChangesAsync()
// → É o método principal da UnitOfWork.
// → Ele executa toda a operação de persistência com segurança transacional.
//
// 6) CreateExecutionStrategy()
// → Cria a estratégia de execução do provider.
// → Essa estratégia encapsula o comportamento de retry configurado no DbContext.
//
// 7) strategy.ExecuteAsync(...)
// → Executa todo o bloco como uma unidade reexecutável.
// → Se ocorrer uma falha transitória suportada, o EF Core pode tentar novamente.
//
// 8) BeginTransactionAsync(IsolationLevel.ReadCommitted)
// → Abre uma transação explícita no banco.
// → O nível ReadCommitted garante que apenas dados já confirmados sejam lidos.
// → Isso ajuda a manter consistência sem ser tão restritivo quanto níveis mais altos.
//
// 9) BeforeSaveChanges()
// → Executado antes de salvar no banco.
// → Ele faz 3 coisas:
//    - encontra entidades com DomainEvents
//    - extrai esses eventos
//    - cria objetos EventStore para persistência posterior
// → Depois limpa os DomainEvents das entidades para evitar duplicidade.
//
// 10) ChangeTracker
// → O ChangeTracker do EF Core acompanha entidades alteradas no contexto.
// → Aqui ele é usado para localizar entidades que geraram eventos de domínio.
//
// 11) domainEvents
// → São os eventos de domínio levantados pelas entidades.
// → Exemplo:
//    - CustomerCreatedEvent
//    - OrderPaidEvent
//
// 12) eventStores
// → São representações persistíveis dos eventos.
// → Normalmente usados para auditoria, histórico ou event store.
//
// 13) context.SaveChangesAsync()
// → Persiste as alterações das entidades no banco relacional.
// → Esse é o ponto onde os dados principais são gravados.
//
// 14) AfterSaveChangesAsync(...)
// → Executado depois do SaveChanges.
// → Faz duas coisas:
//    - publica eventos com MediatR
//    - armazena os eventos no repositório de event store
//
// 15) mediator.Publish(...)
// → Publica os eventos de domínio para handlers interessados.
// → Isso permite desacoplamento entre a gravação principal e reações secundárias.
//
// 16) eventStoreRepository.StoreAsync(...)
// → Persiste os eventos transformados em EventStore.
// → Isso cria um histórico estruturado dos eventos ocorridos.
//
// 17) transaction.CommitAsync()
// → Confirma a transação no banco.
// → Sem esse comando, a transação não é efetivamente concluída.
//
// 18) catch / RollbackAsync()
// → Se ocorrer qualquer exceção, a transação é desfeita.
// → Isso evita gravações parciais e mantém consistência.
//
// 19) Logs
// → O logger registra:
//    - início da transação
//    - sucesso
//    - falhas
// → Isso ajuda em auditoria, observabilidade e diagnóstico.
//
// 20) IReadOnlyList
// → Foi usado para impedir alteração das coleções após sua criação.
// → Isso protege os dados da operação e reforça imutabilidade.
//
// 21) IDisposable
// → A classe libera corretamente os recursos usados.
// → Isso inclui o DbContext e o EventStoreRepository.
//
// 22) Benefício final
// → Essa implementação garante:
//    - persistência transacional
//    - compatibilidade com retry automático do EF Core
//    - coleta, publicação e armazenamento de eventos
//    - rollback em falha
//    - logs e descarte correto de recursos
// ------------------------------------------------------