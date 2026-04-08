

using Ardalis.Result;
using MediatR;

namespace MeuSistema.Application.Customer.Commands;

public class DeleteCustomerCommand(Guid id) :IRequest<Result>
{
    public Guid Id { get; } = id;
}



// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe DeleteCustomerCommand → Representa um comando para exclusão de um cliente na aplicação. 
✅ Propriedade Id → Define o identificador único (GUID) do cliente a ser excluído. 
✅ Implementação de IRequest<Result> → Indica que o comando retornará um resultado padronizado de sucesso ou erro. 
✅ Uso de Ardalis.Result → Facilita a padronização da resposta, permitindo diferentes tipos de retorno como sucesso, erro ou inválido. 
✅ Essa estrutura garante que a exclusão de clientes seja feita de maneira organizada e previsível dentro da aplicação. 
*/
