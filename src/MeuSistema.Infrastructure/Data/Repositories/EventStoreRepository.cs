using MeuSistema.Infrastructure.Data.Context;
using MeuSistema.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Infrastructure.Data.Repositories;

internal sealed class EventStoreRepository(EventStoreDbContext dbContext) : IEventStoreRepository
{
    public async Task StoreAsync(IEnumerable<EventStore> eventStores)
    {
        await dbContext.EventStores.AddRangeAsync(eventStores);
        await dbContext.SaveChangesAsync();
    }

    #region IDisposable

    private bool _disposed;
    ~EventStoreRepository() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            dbContext.Dispose();

        _disposed = true;
    }

    #endregion
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. `internal sealed class EventStoreRepository(...) : IEventStoreRepository`  
   - Classe concreta e selada (não pode ser herdada).  
   - Implementa `IEventStoreRepository`, contrato para armazenar eventos.  
   - Recebe `EventStoreDbContext` no construtor primário.

2. `StoreAsync(IEnumerable<EventStore> eventStores)`  
   - Método assíncrono que adiciona uma coleção de eventos ao banco (`AddRangeAsync`).  
   - `SaveChangesAsync()` persiste as alterações.  
   - Usado em arquiteturas de Event Sourcing para registrar eventos.

3. `IDisposable`  
   - `_disposed` → flag para evitar descarte duplicado.  
   - `~EventStoreRepository() => Dispose(false);`  
     - O `~` define o **destrutor** (finalizer).  
     - Chamado automaticamente pelo **Garbage Collector (GC)** quando o objeto é destruído.  
     - Aqui chama `Dispose(false)`, liberando apenas recursos **não gerenciados**.  
   - `Dispose()` → método público para liberar recursos manualmente.  
     - Chama `Dispose(true)` e `GC.SuppressFinalize(this)` para evitar que o GC rode o destrutor.  
   - `Dispose(bool disposing)` → se `true`, libera recursos gerenciados (ex.: `dbContext.Dispose()`); se `false`, apenas não gerenciados.

---

✅ Em resumo:  
O `EventStoreRepository` é responsável por salvar eventos no banco e garantir que o `DbContext` seja corretamente descartado.  
O destrutor (`~EventStoreRepository`) é uma proteção extra: se você esquecer de chamar `Dispose()`, o GC ainda vai liberar os recursos não gerenciados.  
Assim, ele combina **persistência de eventos** com **gestão segura de recursos**.
*/
