using Ardalis.Result;
using MediatR;
using MeuSistema.Application.Customer.Responses;

namespace MeuSistema.Application.Customer.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<Result<GetCustomersResponse>>;

// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------

/*
✅ Classe GetCustomersQuery → Define uma consulta para recuperar todos os clientes armazenados no sistema.
✅ Herança de IRequest<Result<IEnumerable<CustomerQueryModel>>> → Utiliza MediatR para facilitar a comunicação entre camadas.
✅ Uso de Ardalis.Result → Encapsula a resposta da consulta, permitindo indicar sucesso ou erro de forma estruturada.
✅ Arquitetura baseada em CQRS → Mantém separação entre leitura (query) e escrita (command), garantindo escalabilidade e organização.
✅ Essa abordagem melhora a eficiência da recuperação de dados e permite fácil manutenção da lógica de negócios.
*/
