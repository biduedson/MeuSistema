
using MeuSistema.Domain.Exceptions;
using MeuSistema.SharedKernel;

namespace MeuSistema.Domain.ValueObjects;

public sealed record Email
{
    private Email(string address) =>
        Address = address.ToLowerInvariant().Trim();

    public Email () { }
    public string Address { get; }

    public static Email Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ValidationException("O email é obrigatório.");

        if (!RegexPatterns.EmailIsValid.IsMatch(emailAddress))
            throw new ValidationException("O email é inválido.");

        return new Email(emailAddress);
    }

    public override string ToString() => Address;
}