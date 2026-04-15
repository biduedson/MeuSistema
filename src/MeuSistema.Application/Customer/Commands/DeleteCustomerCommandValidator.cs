

using FluentValidation;

namespace MeuSistema.Application.Customer.Commands;

public class DeleteCustomerCommandValidator :AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("O ID do cliente deve ser um GUID válido.");
    }
}


// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe DeleteCustomerCommandValidator → Define regras de validação para o comando DeleteCustomerCommand. 
✅ Uso de FluentValidation → Permite uma validação fluida para garantir integridade dos dados de entrada. 
✅ Regra de validação do ID → Garante que o identificador do cliente não seja um valor vazio antes da exclusão. 
✅ Essa abordagem evita erros e garante que apenas registros válidos sejam processados. 
*/
