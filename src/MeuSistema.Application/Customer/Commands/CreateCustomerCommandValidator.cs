using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
