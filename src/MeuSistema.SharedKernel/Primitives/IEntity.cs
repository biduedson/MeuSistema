namespace MeuSistema.SharedKernel.Primitives;

public interface IEntity;

public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
{
    TKey Id { get; }
}

// namespace  → localização do arquivo no projeto
// IEntity    → crachá — identifica que a classe é uma entidade do sistema
// IEntity<TKey> → entidade com Id flexível, você escolhe o tipo: Guid, int, string...
// out TKey   → Id só entrega valor, nunca recebe
// : IEntity  → quem implementar IEntity<TKey> também é um IEntity
// where      → TKey precisa saber se comparar com outro TKey
//              impede comparar Ids de tipos diferentes por engano
// TKey Id { get; } → entrega o Id — não permite alterar de fora