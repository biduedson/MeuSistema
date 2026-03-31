using MeuSistema.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MeuSistema.Infrastructure.Data.Context;

public abstract class BaseDbContext<TContext>(DbContextOptions<TContext> dbOptions) : DbContext(dbOptions)
    where TContext : DbContext
{
    private const string Collation = "SQL_Latin1_General_CP1_CI_AS";

    public override ChangeTracker ChangeTracker
    {
        get
        {
            base.ChangeTracker.LazyLoadingEnabled = false;
            base.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
            base.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
            return base.ChangeTracker;
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .AreUnicode(false)
            .HaveMaxLength(254);
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
             .UseCollation(Collation)
             .RemoveCascadeDeleteConvention();

        base.OnModelCreating(modelBuilder);
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Essa classe `BaseDbContext` é uma abstração que centraliza configurações comuns para todos os contextos de banco de dados
do Entity Framework Core. Assim, qualquer contexto que herde dela já terá essas regras aplicadas.

👉 Linha por linha:

1. `private const string Collation = "SQL_Latin1_General_CP1_CI_AS";`
   - Define a *collation* padrão do banco.  
   - *Collation* = regra de ordenação e comparação de textos.  
   - `"CI"` = *Case Insensitive* (ignora maiúsculas/minúsculas).  
   - `"AS"` = *Accent Sensitive* (considera acentos).  
   - Isso garante consistência ao comparar strings no banco.

2. `public override ChangeTracker ChangeTracker { get { ... } }`
   - Sobrescreve a propriedade `ChangeTracker` do EF Core.  
   - *ChangeTracker* = mecanismo que rastreia o estado das entidades (nova, modificada, removida, etc.).  
   - Dentro do `get`:
     - `LazyLoadingEnabled = false` → desativa *Lazy Loading*.  
       🔹 *Lazy Loading* = carregamento automático de dados relacionados quando acessados.  
       Aqui está desativado para evitar consultas inesperadas.  
     - `CascadeDeleteTiming = OnSaveChanges` → deleções em cascata só ocorrem quando `SaveChanges()` é chamado.  
       🔹 *Cascade Delete* = apagar filhos automaticamente quando o pai é apagado.  
     - `DeleteOrphansTiming = OnSaveChanges` → entidades órfãs (sem referência ao pai) só são removidas no `SaveChanges()`.  
       🔹 *Orphans* = registros filhos que perderam o vínculo com o pai.  
     - `return base.ChangeTracker` → retorna o objeto já configurado.

3. `protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)`
   - Define convenções globais para propriedades do tipo `string`.  
   - `.AreUnicode(false)` → strings não serão armazenadas como Unicode (economiza espaço).  
   - `.HaveMaxLength(254)` → todas as strings terão tamanho máximo de 254 caracteres.  
   - Isso evita inconsistências e define um padrão para todas as entidades.

4. `protected override void OnModelCreating(ModelBuilder modelBuilder)`
   - Configurações aplicadas na criação do modelo do banco.  
   - `.UseCollation(Collation)` → aplica a collation definida anteriormente.  
   - `.RemoveCascadeDeleteConvention()` → remove a convenção padrão de deleção em cascata.  
     🔹 Isso significa que o EF não vai apagar filhos automaticamente; será necessário tratar manualmente.  
   - `base.OnModelCreating(modelBuilder)` → chama a implementação da classe pai para manter outras configurações padrão.

---

✅ Em resumo:
Esse `BaseDbContext` garante que todos os contextos derivados tenham:
- Comparação de strings consistente (collation).  
- Rastreamento de mudanças configurado (sem lazy loading, deleções só no SaveChanges).  
- Convenções globais para strings (não Unicode, máximo 254).  
- Remoção da deleção em cascata automática, dando mais controle ao desenvolvedor.  
Assim, ele centraliza regras e evita duplicação de configuração em cada `DbContext`.
*/
