
using MeuSistema.Domain.Shared.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuSistema.Infrastructure.Data.Mappings;

internal class EventStoreConfiguration : IEntityTypeConfiguration<EventStore>
{
    public void Configure(EntityTypeBuilder<EventStore> builder)
    {
        builder
            .HasKey(eventStore => eventStore.Id);

        builder
            .Property(eventStore => eventStore.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(eventStore => eventStore.AggregatedId)
            .IsRequired();

        builder
            .Property(eventStore => eventStore.MessageType)
            .IsRequired();

        builder
            .Property(eventStore => eventStore.Data)
            .IsRequired()
            .HasComment("Evento serializado em JSON");

        builder
            .Property(eventStore => eventStore.OccurredOn)
            .IsRequired()
            .HasColumnName("CreatedAt");
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Essa configuração define como a entidade `EventStore` será mapeada no banco de dados:

1. `HasKey(eventStore => eventStore.Id)`  
   - Define o campo `Id` como chave primária da tabela.

2. `Property(eventStore => eventStore.Id)...`  
   - O `Id` é obrigatório e não é gerado automaticamente pelo banco (`ValueGeneratedNever`).

3. `Property(eventStore => eventStore.AggregatedId)...`  
   - Guarda o identificador da entidade agregada relacionada ao evento (ex: o Id do `Customer`).

4. `Property(eventStore => eventStore.MessageType)...`  
   - Armazena o tipo do evento (ex: `CustomerCreatedEvent`, `CustomerDeletedEvent`).

5. `Property(eventStore => eventStore.Data)...`  
   - Contém os dados do evento serializados em JSON.  
   - O comentário ajuda a documentar a coluna no banco.

6. `Property(eventStore => eventStore.OccurredOn)...`  
   - Registra a data/hora em que o evento ocorreu.  
   - No banco, essa coluna será chamada `CreatedAt`.

✅ Em resumo:  
Essa configuração garante que a tabela `EventStore` seja usada como **armazenamento de eventos de domínio**, com todos os campos obrigatórios e bem definidos para auditoria e rastreabilidade.
*/
