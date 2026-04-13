

using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Domain.ValueObjects;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Customer.Handlers;

public class UpdateCustomerCommandHandler(
    IValidator<UpdateCustomerCommand> validator,
    ICustomerRepository repository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken    
        )
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors());

        var customer = await repository.GetByIdAsync(request.Id);

        if(customer == null)
            return Result.NotFound($"Nenhum cliente encontrado com o Id: {request.Id}");

        var newEmail =  Email.Create(request.Email);

        if(await repository.ExistsByEmailAsync(newEmail, customer.Id))
            return Result.Error("Já existe um cliente cadastrado com esse email.");

        customer.ChangeEmail(newEmail.Address);

        repository.Update(customer);

        await unitOfWork.SaveChangesAsync();

        return Result.SuccessWithMessage("Email do cliente atualizado com sucesso.");
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. Declaração da classe  
   - `UpdateCustomerCommandHandler` é o **Handler** responsável por processar o comando `UpdateCustomerCommand`.  
   - Ele implementa `IRequestHandler<UpdateCustomerCommand, Result>` do MediatR, ou seja, recebe um comando e retorna um `Result`.  
   - Recebe via injeção de dependência:  
     - `IValidator<UpdateCustomerCommand>` → validação com FluentValidation.  
     - `ICustomerRepository` → acesso ao repositório de clientes.  
     - `IUnitOfWork` → garante persistência transacional.

2. Validação inicial  
   - `validator.Validate(request)` → aplica as regras do FluentValidation.  
   - Se inválido, retorna `Result.Invalid(...)` com os erros.  
   - Isso evita consultas desnecessárias ao banco com dados obviamente incorretos.

3. Busca do cliente  
   - `repository.GetByIdAsync(request.Id)` → tenta localizar o cliente pelo Id.  
   - Se não encontrado, retorna `Result.NotFound(...)`.  
   - Garante que só clientes existentes sejam atualizados.

4. Criação do Value Object `Email`  
   - `Email.Create(request.Email)` → transforma a string em um objeto `Email`.  
   - Se o formato for inválido, lança `ValidationException`.  
   - Isso reforça a integridade do domínio: só e‑mails válidos entram no agregado.

5. Verificação de duplicidade  
   - `repository.ExistsByEmailAsync(newEmail, customer.Id)` → checa se já existe outro cliente com o mesmo e‑mail.  
   - Se sim, retorna `Result.Error(...)`.  
   - Evita duplicidade e garante unicidade do e‑mail.

6. Atualização do agregado  
   - `customer.ChangeEmail(newEmail.Address)` → o agregado aplica a mudança.  
   - O domínio é quem toma a decisão final, mantendo consistência interna.

7. Persistência  
   - `repository.Update(customer)` → marca o cliente para atualização.  
   - `unitOfWork.SaveChangesAsync()` → confirma a transação no banco.  
   - Garante atomicidade e consistência.

8. Resultado final  
   - `Result.SuccessWithMessage(...)` → retorna sucesso com mensagem amigável.  
   - Padroniza a resposta da aplicação.

---

✅ Em resumo:  
Esse Handler aplica **defesa em profundidade**:  
- Primeiro o **Validator** barra dados inválidos.  
- Depois o **Value Object Email** garante integridade no domínio.  
- O **Agregado Customer** aplica a regra de negócio.  
- O **Repositório + UnitOfWork** persistem a mudança com segurança.  

O que parece redundância é, na verdade, robustez: impede que consultas ao banco sejam feitas com dados inválidos e garante que o domínio nunca aceite estados incorretos.
*/
