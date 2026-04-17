namespace MeuSistema.SharedKernel.Primitives
{
    public abstract class BaseEntity : IEntity<Guid>
    {
        private readonly List<BaseEvent> _domainEvents = [];
        protected BaseEntity() => Id = Guid.NewGuid();
        protected BaseEntity(Guid id) => Id = id;

        public IEnumerable<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
        public Guid Id { get; private init; }

        protected void AddDomainEvent(BaseEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() =>
            _domainEvents.Clear();
    }
}

// -----------------------------------------------------------------------------
// EXPLICAÇÃO:
//
// 1. Interface IEntity<out TKey>:
//    - O "out" significa covariância: o tipo genérico TKey só pode ser usado como saída.
//    - Por isso, a interface define apenas "TKey Id { get; }" (somente leitura).
//    - Não é permitido usar TKey como entrada (ex.: set ou parâmetros).
//
// 2. Propriedade Id com "private init":
//    - "init" permite atribuir valor apenas na criação do objeto.
//    - Com "private init", só a própria classe pode definir o valor.
//    - Resultado: o Id é criado no construtor e depois só pode ser lido, nunca alterado externamente.
//    - Isso é compatível com o "out": o Id é saída, não entrada.
//
// 3. Diferença entre public e protected:
//    - "public" → acessível por qualquer código externo. Ex.: ClearDomainEvents() pode ser chamado fora.
//    - "protected" → acessível apenas pela própria classe e suas derivadas. Ex.: AddDomainEvent() só pode
//      ser usado internamente ou por classes filhas, protegendo a lógica de negócio.
//
// 4. Resumindo:
//    - O "out" garante que o Id é só leitura.
//    - O "private init" garante que o Id é imutável após a criação.
//    - "public" dá acesso externo controlado (limpar eventos).
//    - "protected" restringe acesso a herdeiros (adicionar eventos).
//    - Juntos, esses conceitos asseguram que toda entidade tem um identificador imutável e controla
//      seus próprios eventos de domínio de forma segura e encapsulada.
// -----------------------------------------------------------------------------
