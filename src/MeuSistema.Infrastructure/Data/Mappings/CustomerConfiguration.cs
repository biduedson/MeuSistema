using MeuSistema.Domain.Entities.CustumerAggregate;
using MeuSistema.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuSistema.Infrastructure.Data.Mappings;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .ConfigureBaseEntity();

        builder
            .Property(customer => customer.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(customer => customer.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(customer => customer.Gender)
            .IsRequired()
            .HasMaxLength(6)
            .HasConversion<string>();

        builder.OwnsOne(customer => customer.Email, ownedNav =>
        {
            ownedNav
                .Property(email => email.Address)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnName(nameof(Customer.Email));

            ownedNav
                .HasIndex(email => email.Address)
                .IsUnique();
        });

        builder
            .Property(customer => customer.DateOfBirth)
            .IsRequired()
            .HasColumnType("Date");
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Essa classe `CustomerConfiguration` implementa `IEntityTypeConfiguration<Customer>`, que é usada pelo EF Core para configurar
como a entidade `Customer` será mapeada para o banco de dados. Assim, todas as regras ficam centralizadas aqui.

👉 Linha por linha:

1. `builder.ConfigureBaseEntity();`
   - Aplica a configuração padrão definida em `EntityTypeBuilderExtensions.ConfigureBaseEntity`.  
   - Isso garante que:
     - O campo `Id` seja a chave primária.  
     - O `Id` seja obrigatório e não gerado automaticamente pelo banco.  
     - A propriedade `DomainEvents` seja ignorada (não persistida no banco).  

2. `builder.Property(customer => customer.FirstName)...`
   - Configura a propriedade `FirstName` (primeiro nome).  
   - `.IsRequired()` → obrigatório, não pode ser nulo.  
   - `.HasMaxLength(100)` → limite de 100 caracteres.  

3. `builder.Property(customer => customer.LastName)...`
   - Configura a propriedade `LastName` (sobrenome).  
   - Também obrigatório e com limite de 100 caracteres.  

4. `builder.Property(customer => customer.Gender)...`
   - Configura a propriedade `Gender` (gênero).  
   - `.IsRequired()` → obrigatório.  
   - `.HasMaxLength(6)` → limite de 6 caracteres (ex: "Male", "Female").  
   - `.HasConversion<string>()` → garante que o valor seja armazenado como texto no banco.  

5. `builder.OwnsOne(customer => customer.Email, ownedNav => { ... })`
   - Configura a propriedade `Email` como um *owned type* (tipo de propriedade que pertence à entidade).  
   - Isso significa que o `Email` não é uma tabela separada, mas sim parte da tabela `Customer`.  
   - Dentro da configuração:
     - `.Property(email => email.Address)...` → define que o campo `Address` é obrigatório, com limite de 254 caracteres, e será armazenado na coluna `Email`.  
     - `.HasIndex(email => email.Address).IsUnique();` → cria um índice único para o campo `Address`, garantindo que não existam dois clientes com o mesmo email.  

6. `builder.Property(customer => customer.DateOfBirth)...`
   - Configura a propriedade `DateOfBirth` (data de nascimento).  
   - `.IsRequired()` → obrigatório.  
   - `.HasColumnType("Date")` → armazena apenas a parte de data (sem hora) no banco.  

---

✅ Em resumo:
Essa configuração garante que a entidade `Customer` seja mapeada corretamente no banco:
- `Id` como chave primária, obrigatório e não gerado automaticamente.  
- `FirstName` e `LastName` obrigatórios, até 100 caracteres.  
- `Gender` obrigatório, até 6 caracteres, armazenado como texto.  
- `Email` tratado como propriedade pertencente ao cliente, com índice único para evitar duplicidade.  
- `DateOfBirth` obrigatório, armazenado como tipo `Date`.  

Assim, o EF Core sabe exatamente como criar a tabela `Customer` e aplicar todas as restrições e regras de negócio.
*/
