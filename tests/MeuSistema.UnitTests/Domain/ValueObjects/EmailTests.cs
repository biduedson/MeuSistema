using FluentAssertions;
using MeuSistema.Domain.Exceptions;
using MeuSistema.Domain.ValueObjects;

namespace MeuSistema.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("ma@hostname.com")]
    [InlineData("ma@hostname.comcom")]
    [InlineData("MA@hostname.coMCom")]
    [InlineData("MA@HOSTNAME.COM")]
    [InlineData("m.a@hostname.co")]
    [InlineData("m_a1a@hostname.com")]
    [InlineData("ma-a@hostname.com")]
    [InlineData("ma-a@hostname.com.edu")]
    [InlineData("ma-a.aa@hostname.com.edu")]
    [InlineData("ma.h.saraf.onemore@hostname.com.edu")]
    [InlineData("ma12@hostname.com")]
    [InlineData("12@hostname.com")]
    public void Should_CreateEmail_WhenValidAddress(string emailAddress)
    {
        var act = Email.Create(emailAddress);

        act.Should().NotBeNull();
        act.Address.Should().Be(emailAddress.ToLowerInvariant().Trim());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_ThrowValidationException_WhenEmailIsEmpty(string emailAddress)
    {
        Action act = () => Email.Create(emailAddress);

        act.Should().Throw<ValidationException>()
             .WithMessage("O email é obrigatório.");
    }

    [Theory]
    [InlineData("invalidd-email")]
    [InlineData("user@")]
    [InlineData("user@com")]
    public void Should_ThrowValidationException_WhenEmailIsInvalid(string emailAddress)
    {
        Action act = () => Email.Create(emailAddress);
        act.Should().Throw<ValidationException>()
             .WithMessage("O email é inválido.");
    }
}

/*
===========================================================
📌 Explicação do código:

- Esse arquivo contém testes unitários para o Value Object "Email".
- O padrão usado é AAA (Arrange, Act, Assert):
  - Arrange → preparar os dados de entrada (emailAddress).
  - Act → executar a ação que queremos testar (Email.Create).
  - Assert → verificar o resultado (se criou corretamente ou lançou exceção).

- [Theory]:
  Indica que o teste é parametrizado. O mesmo método será executado várias vezes
  com diferentes valores de entrada.

- [InlineData]:
  Define os valores concretos que serão passados como parâmetro para cada execução
  do teste. Cada linha é uma execução separada.

- FluentAssertions:
  Biblioteca que permite escrever asserts de forma fluente e legível, como
  "act.Should().NotBeNull()" ou "act.Should().Throw<ValidationException>()".

- Testes implementados:
  1. Should_CreateEmail_WhenValidAddress → garante que emails válidos criam objetos Email corretamente.
  2. Should_ThrowValidationException_WhenEmailIsEmpty → garante que emails vazios lançam exceção "O email é obrigatório".
  3. Should_ThrowValidationException_WhenEmailIsInvalid → garante que emails com formato inválido lançam exceção "O email é inválido".
===========================================================
*/
