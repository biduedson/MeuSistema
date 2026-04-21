

using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Customer.Queries.QueriesModel;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetByIdCustomerQuery(Guid id) : IRequest<Result<CustomerQueryModel>> 
{
    public Guid Id { get;} = id;
}
// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ GetByIdCustomerQuery
→ Representa uma Query (consulta) responsável por solicitar
   os dados de um cliente a partir do seu ID.

✅ IRequest<Result<CustomerQueryModel>>
→ Define que essa query será processada por um handler via MediatR
   e retornará um Result contendo um CustomerQueryModel.

✅ Uso de CustomerQueryModel
→ Indica que o retorno da query é um modelo de leitura (DTO),
   não a entidade de domínio Customer, mantendo a separação do CQRS.

✅ Propriedade Id (Guid)
→ Identificador único do cliente a ser consultado.
   É passado no momento da criação da query e permanece imutável.

✅ Uso de Ardalis.Result
→ Padroniza o retorno da operação,
   permitindo representar cenários como:
   - sucesso (Success)
   - não encontrado (NotFound)
   - inválido (Invalid)

✅ Contexto CQRS
→ Essa classe pertence ao lado de leitura (Query),
   não altera estado da aplicação,
   apenas solicita dados para consulta.
*/