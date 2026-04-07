using MediatR;

namespace MeuSistema.SharedKernel.Primitives;

public abstract class BaseEvent : INotification
{
    public Guid AggregatedId { get; protected init; }
    public string MessageType { get; protected init; }
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;
}