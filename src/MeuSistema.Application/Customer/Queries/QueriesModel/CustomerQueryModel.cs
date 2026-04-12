using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.Application.Customer.Queries.QueriesModel;

public record CustomerQueryModel(
        Guid Id,
        string FirstName,
        string LastName,
        string Gender,
        string Email,
        DateTime DateOfBirth
        ) : IQueryModel<Guid>;




// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------

/*
✅ CustomerQueryModel → Representa o modelo de dados retornado pelas consultas (queries) relacionadas a clientes.
✅ Implementação de IQueryModel<Guid> → Define um contrato genérico para modelos de consulta, garantindo consistência e reutilização.
✅ Sendo um record → Por baixo dos panos é uma class, mas com:
   - Construtor primário gerado automaticamente.
   - Propriedades init-only (imutabilidade após inicialização).
   - Comparação por valor (== compara conteúdo, não referência).
   - Método Deconstruct gerado automaticamente.
✅ Uso em CQRS → Ideal para QueryModels e DTOs, pois são objetos de dados simples, previsíveis e comparáveis.
✅ Arquitetura → Separa claramente os modelos de leitura (query models) dos modelos de escrita (entities), facilitando manutenção e escalabilidade.
⚠️ Observação → Se o ORM (EF Core/Mongo) precisar materializar diretamente esse modelo, é mais seguro usar class. 
   No seu caso atual, como a conversão é feita manualmente no handler, record funciona perfeitamente.
*/
