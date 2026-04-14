using System;
using FluentValidation;

namespace MeuSistema.Application.Customer.Commands;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
                .WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(100)
                .WithMessage("O nome do cliente deve ter no máximo 100 caracteres.");

        RuleFor(command => command.LastName)
            .NotEmpty()
                .WithMessage("O sobrenome do cliente é obrigatório.")
            .MaximumLength(100)
                .WithMessage("O sobrenome do cliente deve ter no máximo 100 caracteres.");

        RuleFor(command => command.Email)
            .NotEmpty()
                .WithMessage("O email do cliente é obrigatório.")
            .EmailAddress()
                .WithMessage("O email do cliente deve ser um endereço de email válido.")
            .MaximumLength(200)
                .WithMessage("O email do cliente deve ter no máximo 200 caracteres.");

        RuleFor(command => command.BirthDate)
            .NotEmpty()
                .WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.Now)
                .WithMessage("A data de nascimento deve estar no passado.");
    }
}

// ------------------------------------------------------
// 🔹 Explicação marota do código 🔹
// ------------------------------------------------------
// - Cada RuleFor define validações para uma propriedade do comando.
// - NotEmpty() cobre null, string vazia e DateTime.MinValue.
// - MaximumLength() limita o tamanho das strings.
// - EmailAddress() valida formato de email.
// - LessThan(DateTime.Now) garante que a data não seja futura.
// - BirthDate como DateTime: se não informado, vira MinValue → cai no NotEmpty.
// - Resultado: qualquer campo inválido gera mensagem clara com o nome da propriedade.
// ------------------------------------------------------
