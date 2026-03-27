using System;
using System.Text.RegularExpressions;

namespace MeuSistema.Domain.ValueObjects;

public sealed record Email
{
    private Email(string address) =>
        Address = address.ToLowerInvariant().Trim();

    public Email () { }
    public string Address { get; }

    public static Email  Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("O email não pode ser vazio.");

        return  new Email(emailAddress);
    }

    public override string ToString() => Address;
}