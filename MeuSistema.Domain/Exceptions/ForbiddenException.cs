namespace MeuSistema.Domain.Exceptions;

public sealed class ForbiddenException : DomainException
 {
    public ForbiddenException(string message)
        : base(message) { }

    public ForbiddenException()
        :base("Forbidden") { }
}
