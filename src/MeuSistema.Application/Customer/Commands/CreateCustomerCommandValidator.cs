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
            .MaximumLength(100);

        
        RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100);

        
        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();
    }
}
