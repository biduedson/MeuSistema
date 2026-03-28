

namespace MeuSistema.Domain.Shared.Exceptions;

public sealed class ValidationException(string message) : DomainException(message)
{
}
