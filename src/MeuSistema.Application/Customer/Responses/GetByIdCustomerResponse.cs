using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Customer.Responses;

public class GetByIdCustomerResponse(
    Guid id,
    string firstName,
    string lastName,
    EGender gender,
    string email,
    DateTime dateOfBirth
) : IResponse
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public EGender Gender { get; } = gender;
    public string Email { get; } = email;
    public DateTime DateOfBirth { get; } = dateOfBirth;
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Classe GetByIdCustomerResponse
→ Representa o modelo de resposta retornado pela query de busca de cliente por ID.

✅ Implementação de IResponse
→ Indica que esse objeto faz parte do contrato padrão de respostas da aplicação,
   podendo ser utilizado em pipelines, logging ou padronização de retornos.

✅ Construtor primário
→ Recebe todos os dados necessários para montar a resposta no momento da criação,
   garantindo que o objeto seja inicializado de forma completa.

✅ Propriedades somente leitura (get)
→ Torna o objeto imutável após sua criação, evitando alterações indevidas nos dados.

✅ Uso de EGender
→ Representa o gênero do cliente através de um enum do domínio,
   mantendo consistência e tipagem forte.

✅ Objetivo
→ Retornar apenas os dados necessários do cliente para consumo externo,
   sem expor informações sensíveis como senha ou dados internos da entidade.

✅ Contexto no CQRS
→ Esse response é utilizado por uma Query (leitura),
   sendo responsável apenas por transportar dados,
   sem conter regras de negócio ou comportamento.
*/