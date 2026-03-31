using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeuSistema.Domain.Shared.Primitives;

public interface IRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
}

/*
------------------------------------------------------------
📌 Explicação conceitual:

- IRepository<TEntity, TKey> → Interface genérica para repositórios.
  - TEntity: representa a entidade do domínio (ex.: Cliente, Pedido).
  - TKey: representa o tipo da chave primária (ex.: int, Guid).

- IDisposable → obriga quem implementar a interface a liberar recursos
  (como conexões de banco).

- Restrições:
  - where TEntity : IEntity<TKey> → só aceita entidades que implementem
    a interface IEntity<TKey>, garantindo que tenham uma chave.
  - where TKey : IEquatable<TKey> → a chave deve ser comparável
    (int, Guid, string).

- Métodos:
  - Add → adiciona uma nova entidade.
  - Update → atualiza uma entidade existente.
  - Remove → remove uma entidade.
  - GetByIdAsync → busca entidade pelo ID de forma assíncrona.

🎯 Conceito: 
Esse contrato define operações básicas de persistência (CRUD) sem se acoplar
a uma tecnologia específica. Assim, a camada de aplicação depende apenas da
interface, e a implementação concreta (ex.: EF Core, MongoDB, memória) pode
variar sem impactar o domínio. É o padrão Repository dentro do DDD.

🟦 Sobre o "in TKey":
- O modificador "in" indica **contravariância**.
- Isso significa que o tipo genérico TKey só é usado como **entrada** 
  nos métodos da interface, nunca como saída.
- No código, repare que TKey aparece apenas como parâmetro em:
      Task<TEntity> GetByIdAsync(TKey id);
- Ou seja, o repositório só consome a chave (recebe como parâmetro),
  não retorna nem produz um TKey.
- Por isso foi marcado com "in": para deixar claro que TKey é usado
  exclusivamente como **parâmetro de entrada**.
------------------------------------------------------------
*/
