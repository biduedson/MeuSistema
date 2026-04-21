using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using MeuSistema.Application.Abstractions;
using MeuSistema.Application.Customer.Queries.QueriesModel;


namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetByIdCustomerQueryHandler(
    IValidator<GetByIdCustomerQuery> validator,
    ICustomerReadOnlyRepository readOnlyRepository
) : IRequestHandler<GetByIdCustomerQuery, Result<CustomerQueryModel>>
{
    public async Task<Result<CustomerQueryModel>> Handle(
        GetByIdCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors());

        var customerQueryModel = await readOnlyRepository.GetByIdAsync(request.Id);

        if (customerQueryModel == null)
            return Result.NotFound($"Nenhum cliente encontrado com o id {request.Id}");

        return Result.Success(customerQueryModel);
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
// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ GetByIdCustomerQueryHandler
→ Handler responsável por processar a query de busca de cliente por ID.

✅ IRequestHandler<GetByIdCustomerQuery, Result<CustomerQueryModel>>
→ Define que esse handler recebe uma GetByIdCustomerQuery
   e retorna um Result contendo um CustomerQueryModel.

✅ IValidator<GetByIdCustomerQuery>
→ Utiliza FluentValidation para validar a requisição antes de executar a lógica.
   Garante que os dados de entrada estejam corretos e evita chamadas desnecessárias ao banco.

✅ Validação (validator.ValidateAsync)
→ Executa a validação da query.
   Caso inválida, retorna Result.Invalid com os erros mapeados via Ardalis.Result.

✅ ICustomerReadOnlyRepository
→ Repositório de leitura (lado Query do CQRS).
   Responsável por buscar os dados já projetados para leitura,
   sem expor a entidade de domínio diretamente.

✅ readOnlyRepository.GetByIdAsync(request.Id)
→ Executa a consulta no banco de dados e retorna um CustomerQueryModel.
   A projeção e uso de AsNoTracking ficam encapsulados na implementação do repositório.

✅ Verificação de inexistência
→ Caso nenhum cliente seja encontrado, retorna Result.NotFound,
   padronizando a resposta para a camada superior (ex: API).

✅ CustomerQueryModel
→ Modelo de leitura (DTO) utilizado para retorno da query.
   Contém apenas os dados necessários para consumo externo,
   sem expor regras ou estrutura interna do domínio.

✅ Result.Success(customerQueryModel)
→ Encapsula o resultado de sucesso,
   mantendo um padrão consistente de resposta na aplicação.

✅ Contexto arquitetural
→ Esse handler pertence ao lado de leitura do CQRS.
   Ele não altera estado, não utiliza tracking para escrita
   e trabalha exclusivamente com dados projetados para consulta.
*/