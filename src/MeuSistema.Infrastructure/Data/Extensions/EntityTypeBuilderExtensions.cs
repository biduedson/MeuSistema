
using MeuSistema.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuSistema.Infrastructure.Data.Extensions;

internal static class EntityTypeBuilderExtensions
{
    internal static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder
            .HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Ignore(entity => entity.DomainEvents);
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Esse código define um **método de extensão** para o `EntityTypeBuilder` do EF Core.  
O objetivo é padronizar a configuração de entidades que herdam de `BaseEntity`.

👉 Linha por linha:

1. `internal static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)`
   - Cria um método de extensão chamado `ConfigureBaseEntity`.  
   - Ele só pode ser usado em entidades (`TEntity`) que herdam de `BaseEntity`.  
   - *EntityTypeBuilder* = objeto usado pelo EF Core para configurar como uma entidade será mapeada para o banco.

2. `builder.HasKey(entity => entity.Id);`
   - Define que a propriedade `Id` é a **chave primária** da entidade.  
   - *HasKey* = método que indica qual campo será usado como identificador único no banco.

3. `builder.Property(entity => entity.Id).IsRequired().ValueGeneratedNever();`
   - Configura a propriedade `Id`:  
     - `.IsRequired()` → obrigatório, não pode ser nulo.  
     - `.ValueGeneratedNever()` → o banco **não gera automaticamente** o valor do `Id`.  
       🔹 Isso significa que o `Id` deve ser atribuído manualmente pelo código (ex: GUID ou outro identificador).

4. `builder.Ignore(entity => entity.DomainEvents);`
   - Diz ao EF Core para **ignorar** a propriedade `DomainEvents`.  
   - *DomainEvents* = lista de eventos de domínio que a entidade dispara (conceito do DDD).  
   - Esses eventos não devem ser persistidos no banco, só usados dentro da lógica de domínio.  
   - *Ignore* = exclui essa propriedade do mapeamento para tabelas.

---

✅ Em resumo:
Esse método garante que todas as entidades base (`BaseEntity`) sejam configuradas de forma consistente:
- `Id` como chave primária, obrigatório e não gerado pelo banco.  
- `DomainEvents` ignorado (não vai para o banco).  
Assim, você não precisa repetir essa configuração em cada entidade — basta chamar `builder.ConfigureBaseEntity()` dentro do `OnModelCreating`.
*/
