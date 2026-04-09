using FluentValidation;

namespace MeuSistema.Application.Customer.Queries.GetByIdCustomer;

public class GetByIdCustomerQueryValidator : AbstractValidator<GetByIdCustomerQuery>
{
    public GetByIdCustomerQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("O ID do cliente deve ser um GUID válido.");
    }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe GetByIdCustomerQueryValidator
→ Define regras de validação para a consulta GetByIdCustomerQuery.

✅ Uso de FluentValidation
→ Biblioteca utilizada para criar validações de forma fluida, organizada e desacoplada.

✅ Regra de validação do ID
→ Garante que o campo ID:
   - Não seja vazio (NotEmpty)
   - Seja um GUID válido (Guid.TryParse)

✅ Objetivo
→ Evitar que requisições inválidas cheguem à camada de aplicação,
   garantindo integridade e prevenindo erros em tempo de execução.
*/