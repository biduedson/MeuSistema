using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Context;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> dbOptions)
    : BaseDbContext<CustomerDbContext>(dbOptions)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Essa classe `CustomerDbContext` representa o **contexto de banco de dados** específico para a entidade `Customer`.  
Ela herda de `BaseDbContext<CustomerDbContext>`, o que significa que já recebe todas as configurações globais definidas no contexto base.

👉 Linha por linha:

1. `public class CustomerDbContext(DbContextOptions<CustomerDbContext> dbOptions) : BaseDbContext<CustomerDbContext>(dbOptions)`
   - Define a classe `CustomerDbContext`.  
   - O construtor recebe `DbContextOptions<CustomerDbContext>`, que contém as configurações do banco (string de conexão, provider, etc.).  
   - Passa essas opções para o `BaseDbContext`, que aplica convenções globais (collation, rastreamento, etc.).  
   - Assim, o `CustomerDbContext` já nasce configurado com as regras comuns.

2. `public DbSet<Customer> Customers => Set<Customer>();`
   - Define um **DbSet** para a entidade `Customer`.  
   - *DbSet* = coleção que representa uma tabela no banco.  
   - Aqui, `Customers` será a tabela de clientes.  
   - O EF Core usa esse DbSet para consultas (`LINQ`) e operações de escrita (`Add`, `Update`, `Remove`).

3. `protected override void OnModelCreating(ModelBuilder modelBuilder)`
   - Método chamado quando o EF Core está construindo o modelo do banco.  
   - `base.OnModelCreating(modelBuilder)` → aplica as configurações herdadas do `BaseDbContext` (collation, remoção de deleção em cascata, etc.).  
   - `modelBuilder.ApplyConfiguration(new CustomerConfiguration());` → aplica a configuração específica da entidade `Customer`.  
     - Essa configuração está na classe `CustomerConfiguration`, onde são definidas regras como tamanho máximo de campos, obrigatoriedade, índices únicos, etc.

---

✅ Em resumo:
- `CustomerDbContext` é o contexto que representa a tabela de clientes no banco.  
- Ele herda configurações globais do `BaseDbContext`.  
- Expõe o `DbSet<Customer>` para manipular clientes.  
- Aplica regras específicas da entidade `Customer` via `CustomerConfiguration`.  
Assim, o EF Core sabe exatamente como criar e manipular a tabela `Customers` com todas as restrições e convenções definidas.
*/
