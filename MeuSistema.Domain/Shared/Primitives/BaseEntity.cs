using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeuSistema.Domain.Shared.Primitives;

public abstract class BaseEntity : IEntity<Guid>
{
    protected BaseEntity() => Id = Guid.NewGuid();

    protected BaseEntity(Guid id) => Id = id;

    public Guid Id { get; private init; }
}

