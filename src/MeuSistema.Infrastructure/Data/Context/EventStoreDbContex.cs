using MeuSistema.Domain.Shared.Primitives;
using MeuSistema.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Context;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions)
    : BaseDbContext<EventStoreDbContext>(dbOptions)
{
    public DbSet<EventStore> EventStores => Set<EventStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Essa classe `EventStoreDbContext` representa o contexto de banco de dados específico para armazenar os eventos de domínio.

1. `public class EventStoreDbContext(...) : BaseDbContext<EventStoreDbContext>(dbOptions)`  
   - Define o contexto do EF Core para a tabela de eventos.  
   - Herda de `BaseDbContext`, recebendo as configurações globais.  
   - O construtor recebe as opções de configuração (`DbContextOptions`).

2. `public DbSet<EventStore> EventStores => Set<EventStore>();`  
   - Expõe o conjunto de eventos (`EventStore`) como uma tabela no banco.  
   - Permite consultar e salvar eventos diretamente via EF Core.

3. `protected override void OnModelCreating(ModelBuilder modelBuilder)`  
   - Método chamado na criação do modelo.  
   - `base.OnModelCreating(modelBuilder)` aplica as convenções herdadas.  
   - `modelBuilder.ApplyConfiguration(new EventStoreConfiguration());` aplica a configuração detalhada da entidade `EventStore` (definição de chave, colunas obrigatórias, comentários, etc.).

✅ Em resumo:  
Esse `DbContext` é responsável por persistir os eventos de domínio na tabela `EventStore`, garantindo que cada evento seja armazenado com suas informações (Id, tipo, dados em JSON e data de ocorrência).
*/
