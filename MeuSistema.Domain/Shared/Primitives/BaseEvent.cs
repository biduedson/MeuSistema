
namespace MeuSistema.Domain.Shared.Primitives;

public abstract class BaseEvent
{
    public Guid AggregatedId { get; protected init; }
    public string MessageType { get; protected init; }
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;
}