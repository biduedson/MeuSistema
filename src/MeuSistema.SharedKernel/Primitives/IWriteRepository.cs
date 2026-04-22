
namespace MeuSistema.SharedKernel.Primitives;

public interface IWriteRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<TEntity?> GetByIdAsync(TKey id);
}
/*
------------------------------------------------------------
📌 EXPLICAÇÃO CONCEITUAL (ATUALIZADA - WRITE SIDE / CQRS)

✅ IWriteRepository<TEntity, TKey>
→ Interface genérica que representa o **lado de escrita (Command)** da aplicação.
→ Define operações responsáveis por **modificar o estado do sistema**.

------------------------------------------------------------

📌 Tipos genéricos:

- TEntity
  → Representa a entidade do domínio (ex.: Customer, Order).
  → Deve implementar IEntity<TKey>, garantindo que possui uma chave.

- TKey
  → Representa o tipo da chave da entidade (ex.: Guid, int, string).

------------------------------------------------------------

📌 Restrições (constraints):

- where TEntity : IEntity<TKey>
  → Garante que toda entidade tenha um identificador (Id).
  → Permite trabalhar com a entidade de forma segura e consistente.

- where TKey : IEquatable<TKey>
  → Garante que a chave pode ser comparada corretamente.
  → Necessário para operações como busca por Id.

------------------------------------------------------------

📌 IDisposable

→ Indica que a implementação do repositório pode liberar recursos,
  como conexões com banco de dados.

→ No contexto atual, o ciclo de vida do DbContext pode ser controlado
  manualmente ou pelo container de DI.

------------------------------------------------------------

📌 Métodos:

- Add(TEntity entity)
  → Adiciona uma nova entidade ao contexto.

- Update(TEntity entity)
  → Marca a entidade como modificada para persistência.

- Remove(TEntity entity)
  → Remove a entidade do contexto.

- GetByIdAsync(TKey id)
  → Busca uma entidade pelo identificador.
  → Retorna TEntity? (nullable), pois a entidade pode não existir.
  → No write side, a entidade normalmente é retornada **com tracking**,
    permitindo alteração e posterior persistência.

------------------------------------------------------------

📌 Contexto CQRS:

→ Essa interface pertence ao **lado de escrita (Command)**.

✔ Responsável por:
- Inserir dados
- Atualizar dados
- Remover dados
- Recuperar entidade para alteração

❌ NÃO é responsável por:
- Listagens (GetAll)
- Paginação
- Filtros complexos
- Projeções (DTO / QueryModel)

Essas responsabilidades pertencem ao **lado de leitura (Query)**,
representado por IReadOnlyRepository.

------------------------------------------------------------

📌 Separação de responsabilidades:

Write (Command):
→ IWriteRepository
→ trabalha com entidades de domínio (TEntity)
→ usa tracking (EF Core)

Read (Query):
→ IReadOnlyRepository
→ trabalha com QueryModels (DTOs)
→ usa AsNoTracking()

------------------------------------------------------------

📌 Sobre o "in TKey" (contravariância):

→ O modificador "in" indica que TKey é usado apenas como entrada.

→ No contrato:
   Task<TEntity?> GetByIdAsync(TKey id);

→ TKey:
   ✔ é consumido como parâmetro
   ❌ não é retornado

→ Isso permite maior flexibilidade de tipos em cenários genéricos.

------------------------------------------------------------

🎯 RESUMO:

→ IWriteRepository define operações de escrita no padrão DDD + CQRS.
→ Trabalha com entidades de domínio e controle de estado.
→ Não mistura responsabilidades de leitura.
→ Mantém a aplicação desacoplada da tecnologia de persistência.

------------------------------------------------------------
*/