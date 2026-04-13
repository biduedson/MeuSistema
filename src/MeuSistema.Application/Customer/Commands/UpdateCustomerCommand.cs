
using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using MediatR;

namespace MeuSistema.Application.Customer.Commands;

public class UpdateCustomerCommand : IRequest<Result>
{
    [Required]
    public Guid Id { get;set; }

    [Required]
    [MaxLength(200)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

}
// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe UpdateCustomerCommand → Representa um comando para atualização de informações de um cliente. 
✅ Propriedade Id → Define o identificador único do cliente a ser atualizado. 
✅ Propriedade Email → Define o novo e-mail do cliente, garantindo que siga regras de validação. 
✅ Uso de Data Annotations → Garante que os dados fornecidos estejam em um formato válido antes do processamento. 
✅ Implementação de IRequest<Result> → Indica que o comando retornará um resultado padronizado de sucesso ou erro. 
✅ Essa estrutura melhora a integridade dos dados ao garantir que apenas clientes válidos sejam atualizados corretamente. 
*/
