
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using MeuSistema.Application.Customer.Commands;
using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Customer.Handlers;

public class DeleteCustomerCommandHandler(
    IValidator<DeleteCustomerCommand> validator,
    ICustomerRepository repository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid) return  Result.Invalid(validationResult.AsErrors());

        var customer = await repository.GetByIdAsync(request.Id);

        if(customer == null) return Result.NotFound($"Nenhum customer encontrado com o id: { request.Id}");


        customer.Delete();

        repository.Remove(customer);
        
        await unitOfWork.SaveChangesAsync();
        
        return Result.SuccessWithMessage("Customer deletado com sucesso");
    }
}
// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe DeleteCustomerCommandHandler → Processa o comando para remoção de um cliente. 
✅ Uso de FluentValidation → Garante que os dados da requisição sejam válidos antes da execução. 
✅ Implementação de IUnitOfWork → Gerencia transações para persistência segura das operações. 
✅ Verificação da existência do cliente → Impede erros ao tentar excluir registros inexistentes. 
✅ Geração automática do evento "CustomerDeletedEvent" → Permite rastrear operações realizadas sobre clientes. 
✅ Essa abordagem promove segurança e consistência na exclusão de clientes dentro do sistema. 
*/