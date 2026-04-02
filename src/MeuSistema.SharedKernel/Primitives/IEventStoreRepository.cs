namespace MeuSistema.SharedKernel.Primitives;

public interface IEventStoreRepository : IDisposable
{
    Task StoreAsync(IEnumerable<EventStore> eventStores);
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. `namespace MeuSistema.Domain.Shared.Primitives;`  
   - Define o agrupamento lógico onde a interface está.  
   - "Shared.Primitives" indica que são tipos fundamentais usados em várias partes do domínio.

2. `public interface IEventStoreRepository : IDisposable`  
   - Declara uma interface pública chamada `IEventStoreRepository`.  
   - Interfaces são contratos: descrevem métodos que classes concretas devem implementar.  
   - `: IDisposable` → obriga qualquer implementação a liberar recursos corretamente (ex.: conexões de banco).

3. `Task StoreAsync(IEnumerable<EventStore> eventStores);`  
   - Método assíncrono para armazenar uma coleção de eventos (`EventStore`).  
   - `Task` → indica execução assíncrona, não bloqueia a aplicação.  
   - `IEnumerable<EventStore>` → sequência de eventos que devem ser persistidos.

---

✅ Em resumo:  
Essa interface define o contrato para um repositório de eventos.  
Ela garante que qualquer implementação saiba **armazenar eventos** de forma assíncrona e também **liberar recursos** com `Dispose()`.
*/
