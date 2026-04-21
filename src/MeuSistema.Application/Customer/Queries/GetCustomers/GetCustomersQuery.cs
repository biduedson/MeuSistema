using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Customer.Queries.QueriesModel;
using MeuSistema.Application.Customer.Responses;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<Result<IReadOnlyList<CustomerQueryModel>>>;

// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------

/*
✅ Classe GetCustomersQuery → Define uma consulta para recuperar todos os clientes armazenados no sistema.
✅ Herança de IRequest<Result<IEnumerable<CustomerQueryModel>>> → Utiliza MediatR para facilitar a comunicação entre camadas.
✅ Uso de Ardalis.Result → Encapsula a resposta da consulta, permitindo indicar sucesso ou erro de forma estruturada.
✅ Arquitetura baseada em CQRS → Mantém separação entre leitura (query) e escrita (command), garantindo escalabilidade e organização.
✅ Essa abordagem melhora a eficiência da recuperação de dados e permite fácil manutenção da lógica de negócios.
// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------

/*
✅ GetCustomersQuery
→ Representa uma Query responsável por solicitar
   a lista de clientes cadastrados no sistema.

✅ IRequest<Result<IReadOnlyList<CustomerQueryModel>>>
→ Define que essa query será processada por um handler via MediatR
   e retornará um Result contendo uma coleção somente leitura
   de CustomerQueryModel.

✅ Uso de CustomerQueryModel
→ Indica que a consulta retorna modelos de leitura (DTOs),
   próprios para exposição externa,
   sem depender diretamente da entidade de domínio Customer.

✅ Uso de IReadOnlyList<CustomerQueryModel>
→ Deixa explícito que o retorno é uma coleção somente leitura,
   adequada para consultas onde os dados serão apenas consumidos.

✅ Uso de Ardalis.Result
→ Padroniza o retorno da operação,
   permitindo representar de forma consistente cenários como sucesso,
   falha ou outros status definidos pela aplicação.

✅ Contexto CQRS
→ Essa classe pertence ao lado de leitura (Query),
   sendo usada apenas para recuperar dados,
   sem alterar o estado da aplicação.
*/