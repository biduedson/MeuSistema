

using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Customer.Responses;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetByIdCustomerQuery(Guid id) : IRequest<Result<GetByIdCustomerResponse>> 
{
    public Guid Id { get;} = id;
}
// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe GetByIdCustomerQuery → Representa uma consulta para obter um cliente pelo ID na aplicação. 
✅ Propriedade Id → Define o identificador único (GUID) do cliente a ser consultado. 
✅ Implementação de IRequest<Result> → Indica que o comando retornará um resultado padronizado de sucesso ou erro. 
✅ Uso de Ardalis.Result → Facilita a padronização da resposta, permitindo diferentes tipos de retorno como sucesso, erro ou inválido. 
✅ Essa estrutura garante que a exclusão de clientes seja feita de maneira organizada e previsível dentro da aplicação. 
*/