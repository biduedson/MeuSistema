using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Extensions;

internal static class ModelBuilderExtensions
{
    internal static void RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList();

        foreach (var foreignKey in foreignKeys)
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
    }
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

Esse código define um **método de extensão** para o `ModelBuilder` do Entity Framework Core. 
O objetivo é **remover a convenção padrão de deleção em cascata** em relacionamentos entre entidades.

👉 Linha por linha:

1. `var foreignKeys = modelBuilder.Model.GetEntityTypes()...`
   - `modelBuilder.Model.GetEntityTypes()` → pega todas as entidades (tabelas) configuradas no modelo.
   - `.SelectMany(entity => entity.GetForeignKeys())` → para cada entidade, obtém todas as chaves estrangeiras (FKs), ou seja, os relacionamentos com outras tabelas.
   - `.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)` → filtra apenas as FKs que:
       - `!fk.IsOwnership` → não são de "ownership".  
         🔹 *Ownership* em EF Core significa "posse": quando uma entidade depende totalmente da outra, como um **Value Object** que só existe dentro da entidade pai.  
         Exemplo: um `Endereco` que só existe dentro de `Cliente`. Se o cliente for apagado, o endereço também deve ser.  
       - `fk.DeleteBehavior == DeleteBehavior.Cascade` → relacionamentos configurados para deleção em cascata (se apagar o pai, os filhos são apagados automaticamente).
   - `.ToList()` → transforma o resultado em uma lista para poder iterar.

2. `foreach (var foreignKey in foreignKeys)`
   - Percorre cada chave estrangeira encontrada.

3. `foreignKey.DeleteBehavior = DeleteBehavior.Restrict;`
   - Altera o comportamento da deleção.  
   - Em vez de "Cascade" (apagar filhos automaticamente quando o pai é apagado), passa para "Restrict".  
   - *Restrict* significa: o banco **não permite apagar o pai se ainda houver filhos relacionados**.  
   - Isso força o desenvolvedor a tratar manualmente a exclusão dos filhos, evitando perda de dados acidental.

---

✅ Em resumo:
Esse método percorre todos os relacionamentos do modelo e **remove a deleção em cascata**, substituindo por **restrição**.  
Assim, se você tentar apagar um registro pai que ainda tem filhos, o banco vai impedir.  
Isso dá mais segurança e controle, evitando que dados sejam apagados automaticamente sem intenção.
*/
