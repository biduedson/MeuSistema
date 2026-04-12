using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.Application.Customer.Responses;
using MeuSistema.Domain.Entities.CustumerAggregate;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomersQuery, Result<GetCustomersResponse>>
{
    public async Task<Result<GetCustomersResponse>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await repository.GetAllAsync();

        var customerQueryModels = customers.Select(c => new CustomerQueryModel(
            c.Id,
            c.FirstName,
            c.LastName,
            c.Gender.ToString(),
            c.Email.ToString(),
            c.DateOfBirth
        ));

        return Result.Success(new GetCustomersResponse(customerQueryModels));
    }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------
/*
✅ GetCustomersQueryHandler → Handler responsável por processar a query "GetCustomersQuery".
✅ IRequestHandler<GetCustomersQuery, Result<GetCustomersResponse>> → Define que esse handler responde a GetCustomersQuery e retorna um Result<GetCustomersResponse>.
✅ ICustomerRepository → Injeta o repositório, que abstrai o acesso ao banco (DDD). O handler não conhece EF Core diretamente.
✅ repository.GetAllAsync() → Busca todas as entidades Customer. O EF Core materializa os objetos dentro da implementação do repositório.
✅ Conversão para CustomerQueryModel → O handler projeta manualmente cada entidade Customer em um DTO de leitura (QueryModel). 
   - Aqui não há materialização pelo ORM, apenas conversão.
   - O uso de record em CustomerQueryModel garante imutabilidade e concisão.
✅ Result.Success → Encapsula o retorno em um objeto Result, padronizando sucesso/falha e facilitando o tratamento na camada superior.
*/
