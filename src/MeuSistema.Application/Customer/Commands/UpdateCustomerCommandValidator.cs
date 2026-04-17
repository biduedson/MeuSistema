

using FluentValidation;

namespace MeuSistema.Application.Customer.Commands;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
               .WithMessage("O Id do cliente é obrigatório.");

        RuleFor(command => command.Email)
            .NotEmpty()
               .WithMessage("O email do cliente é obrigatório.")
            .EmailAddress()
               .WithMessage("O email do cliente deve ser um endereço de email válido.")
            .MaximumLength(254)
               .WithMessage("O email do cliente deve ter no máximo 254 caracteres.");
    }
}
/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. Declaração da classe  
   - `UpdateCustomerCommandValidator` é um validador específico para o comando `UpdateCustomerCommand`.  
   - Ele herda de `AbstractValidator<T>` da biblioteca FluentValidation, permitindo definir regras de validação de forma fluente e expressiva.  
   - O construtor define todas as regras que serão aplicadas quando o comando for validado.

2. Validação do campo `Id`  
   - `NotEmpty()` → garante que o Id não seja vazio (Guid.Empty).  
   - `Must(id => Guid.TryParse(id.ToString(), out _))` → garante que o valor seja um GUID válido.  
   - Mensagens personalizadas são fornecidas para cada falha:  
     - "O Id do cliente é obrigatório."  
     - "O ID do cliente deve ser um GUID válido."

3. Validação do campo `Email`  
   - `NotEmpty()` → o e-mail não pode ser nulo ou vazio.  
   - `EmailAddress()` → valida se o formato é realmente de um endereço de e-mail (usa regex interna).  
   - `MaximumLength(200)` → impede que o e-mail ultrapasse 200 caracteres.  
   - Mensagens personalizadas para cada regra:  
     - "O email do cliente é obrigatório."  
     - "O email do cliente deve ser um endereço de email válido."  
     - "O email do cliente deve ter no máximo 200 caracteres."

4. Comportamento em execução  
   - Quando um `UpdateCustomerCommand` é enviado pelo MediatR, o FluentValidation roda automaticamente essas regras.  
   - Se o comando tiver valores inválidos (ex.: `Email = "ggggggg"`), o validador retorna erros com as mensagens configuradas.  
   - Isso impede que o Handler execute a lógica de atualização com dados incorretos.

---

✅ Em resumo:  
Esse validador garante que os dados do comando `UpdateCustomerCommand` sejam consistentes antes de chegar ao Handler.  
- O `Id` precisa ser um GUID válido e não vazio.  
- O `Email` precisa ser preenchido, ter formato válido e no máximo 200 caracteres.  
Assim, a aplicação mantém integridade e evita erros de negócio logo na entrada.
*/
