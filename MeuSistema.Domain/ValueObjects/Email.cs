
using MeuSistema.Domain.Shared;
using MeuSistema.Domain.Shared.Primitives;

namespace MeuSistema.Domain.ValueObjects;

public sealed record Email
{
    private Email(string address) =>
        Address = address.ToLowerInvariant().Trim();

    public Email () { }
    public string Address { get; }

    public static DomainResult<Email> Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return DomainResult<Email>.Failure("O email é obrigatório.");

        return !RegexPatterns.EmailIsValid.IsMatch(emailAddress)
               ?DomainResult<Email>.Failure("O email é inválido.")  
               :DomainResult<Email>.Success(new Email(emailAddress));
    }

    public override string ToString() => Address;
}