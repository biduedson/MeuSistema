

namespace MeuSistema.Domain.Shared.Exceptions
{
   public sealed class UnauthorizedException: DomainException
    {
        public UnauthorizedException(string message)
            : base(message) { }

        public UnauthorizedException()
            : base("Unauthorized") { }
    }
}
