using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Abstractions;
using MeuSistema.Application.Customer.Queries.QueriesModel;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetCustomersQueryHandler(ICustomerReadOnlyRepository readOnlyRepository)
    : IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerQueryModel>>>
{
    public async Task<Result<IReadOnlyList<CustomerQueryModel>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await readOnlyRepository.GetAllAsync();
   
        return Result.Success(customers);
    }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------
/*
✅ GetCustomersQueryHandler
→ Handler responsável por processar a query GetCustomersQuery.

✅ IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerQueryModel>>>
→ Define que esse handler recebe uma GetCustomersQuery
   e retorna um Result contendo uma lista somente leitura de CustomerQueryModel.

✅ ICustomerReadOnlyRepository
→ Injeta o repositório de leitura da aplicação.
   Esse contrato pertence ao lado de Query no CQRS
   e abstrai a consulta dos dados sem expor detalhes da infraestrutura.

✅ readOnlyRepository.GetAllAsync()
→ Executa a busca de todos os clientes no fluxo de leitura.
   A responsabilidade de consultar o banco e projetar os dados
   fica concentrada na implementação do repositório de leitura.

✅ CustomerQueryModel
→ Representa o modelo de leitura retornado pela query.
   Ele existe para transportar apenas os dados necessários para consulta,
   sem expor diretamente a entidade de domínio Customer.

✅ IReadOnlyList<CustomerQueryModel>
→ Indica que o retorno é uma coleção somente leitura,
   deixando claro que o objetivo da query é apenas consultar dados,
   e não modificá-los.

✅ Result.Success(customers)
→ Encapsula a lista retornada em um Result de sucesso,
   padronizando a resposta da aplicação e facilitando
   o tratamento do resultado nas camadas superiores.

✅ Contexto arquitetural
→ Esse handler pertence ao lado de leitura do CQRS.
   Ele não altera estado da aplicação, não usa tracking para escrita
   e trabalha apenas com dados projetados para consulta.
*/