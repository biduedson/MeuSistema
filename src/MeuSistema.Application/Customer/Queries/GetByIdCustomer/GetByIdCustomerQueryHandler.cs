using System.Diagnostics;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using MeuSistema.Application.Customer.Responses;
using MeuSistema.Domain.Entities.CustumerAggregate;


namespace MeuSistema.Application.Customer.Queries.GetByIdCustomer;

public class GetByIdCustomerQueryHandler(
    IValidator<GetByIdCustomerQuery> validator,
    ICustomerRepository repository
) : IRequestHandler<GetByIdCustomerQuery, Result<GetByIdCustomerResponse>>
{
    public async Task<Result<GetByIdCustomerResponse>> Handle(
        GetByIdCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors());

        var customer = await repository.GetByIdAsync(request.Id);

        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado com o id {request.Id}");

        return Result.Success(new GetByIdCustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Gender,
            customer.Email.ToString(),
            customer.DateOfBirth
        ));
    }

}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe GetByIdCustomerQueryHandler → Processa a query responsável por buscar um cliente pelo ID. 
✅ Uso de FluentValidation → Garante que os dados da requisição sejam válidos antes de executar a consulta. 
✅ Uso de Ardalis.Result → Padroniza as respostas da aplicação, permitindo retorno de sucesso, erro ou não encontrado. 
✅ Consulta ao repositório (ICustomerRepository) → Recupera o cliente a partir do banco de dados utilizando a camada de domínio. 
✅ Verificação de cliente inexistente → Retorna Result.NotFound caso nenhum cliente seja encontrado com o ID informado. 
✅ Mapeamento para GetByIdCustomerResponse → Converte a entidade de domínio em um objeto seguro para exposição externa. 
✅ Proteção de dados sensíveis → Evita retorno de informações internas como senha ou propriedades do domínio. 
✅ Essa estrutura garante uma leitura organizada, segura e alinhada com os princípios de CQRS e DDD. 
*/