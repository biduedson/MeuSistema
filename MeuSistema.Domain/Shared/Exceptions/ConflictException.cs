

namespace MeuSistema.Domain.Shared.Exceptions;

public sealed class ConflictException(string message)
    : DomainException(message){}
