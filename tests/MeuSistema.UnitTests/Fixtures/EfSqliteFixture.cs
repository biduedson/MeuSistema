

using MeuSistema.Infrastructure.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.UnitTests.Fixtures;

public class EfSqliteFixture : IAsyncLifetime, IDisposable
{
    private const string ConnectionString = "Data Source=:memory:";
    private readonly SqliteConnection _connection;

    

    public EfSqliteFixture()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        var builder = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection);
        Context = new AppDbContext(builder.Options); 

    }

    public AppDbContext Context { get; }

    #region IAsyncLifeTime

    public async Task InitializeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();     
    }

    public  Task DisposeAsync() => Task.CompletedTask;

    #endregion

    #region IDisposable

    private bool _disposed;

    ~EfSqliteFixture() => Dispose(false);

    public  void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if(disposing)
        {
            _connection.Dispose();
            Context?.Dispose();
        }

        _disposed = true;
    }
    #endregion
}
// -----------------------------------------------------------------------------
// Explicação da lógica do EfSqliteFixture
//
// Este fixture existe para permitir testes com EF Core usando um banco SQLite
// em memória. A ideia é ter um banco real para teste, mas temporário, rápido
// e descartável.
//
// Fluxo lógico da classe:
// 1. A classe cria uma conexão SQLite apontando para ":memory:".
// 2. Essa conexão é aberta imediatamente no construtor.
// 3. O AppDbContext é configurado para usar exatamente essa conexão aberta.
// 4. Antes dos testes, o banco é apagado e recriado.
// 5. Durante os testes, o Context é reutilizado para operações reais.
// 6. Ao final, a conexão e o contexto são descartados corretamente.
//
// Por que a conexão é guardada em campo?
// - Porque no SQLite em memória o banco não vive sozinho.
// - Ele só existe enquanto a conexão estiver aberta.
// - Então não basta criar o DbContext: é necessário manter a conexão viva.
// - Se essa conexão for fechada ou descartada, o banco some da memória.
//
// Tradução literal da ideia principal:
// - "SqliteConnection" = conexão com o SQLite.
// - "Open()" = abrir a conexão.
// - "UseSqlite(_connection)" = usar SQLite com essa conexão específica.
// - "AppDbContext(builder.Options)" = criar o contexto com essas configurações.
//
// Por que não deixar isso solto e temporário?
// - Porque o GC (Garbage Collector) cuida apenas da memória gerenciada.
// - Ele não foi feito para controlar no tempo certo recursos externos como
//   conexão com banco.
// - Se esse recurso não for controlado manualmente, o momento da liberação
//   fica imprevisível.
// - Em teste isso é ruim, porque o banco em memória depende diretamente da
//   conexão continuar existindo.
//
// Papel do IDisposable nessa classe:
// - "IDisposable" existe para liberar recursos de forma determinística.
// - "Dispose()" = descarte manual e controlado.
// - Aqui isso é usado para encerrar corretamente:
//     * a conexão SQLite
//     * o AppDbContext
// - Isso evita deixar recurso aberto e evita depender do GC para algo que deve
//   ser encerrado de forma explícita.
//
// Lógica do Dispose:
// - "Dispose()" chama "Dispose(true)".
// - O "true" significa: estou descartando manualmente, então posso liberar
//   recursos gerenciados com segurança.
// - Depois chama "GC.SuppressFinalize(this)".
// - Isso avisa ao Garbage Collector que o finalizador não precisa mais rodar,
//   porque a limpeza já foi feita manualmente.
//
// Papel do finalizador:
// - "~EfSqliteFixture()" é o finalizador.
// - Ele chama "Dispose(false)".
// - O "false" significa: a limpeza está acontecendo pelo GC, não manualmente.
// - Isso funciona como fallback de segurança caso Dispose() não seja chamado.
//
// Por que existe o campo "_disposed"?
// - Para impedir descarte duplo.
// - Sem isso, a classe poderia tentar liberar a mesma conexão/contexto mais de
//   uma vez.
// - Então "_disposed" funciona como trava de proteção.
//
// Papel do IAsyncLifetime:
// - Essa interface é do xUnit e permite preparar e encerrar o fixture no ciclo
//   de vida dos testes.
// - "InitializeAsync()" roda antes do uso.
// - "DisposeAsync()" existe para cumprir o contrato assíncrono da interface.
//
// Lógica do InitializeAsync:
// - "EnsureDeletedAsync()" = garante que qualquer estrutura anterior seja apagada.
// - "EnsureCreatedAsync()" = garante que o schema seja criado de novo.
// - Isso faz os testes começarem com estado limpo e previsível.
//
// Em resumo:
// - O banco em memória depende da conexão aberta.
// - O EF Core depende de um DbContext configurado com essa conexão.
// - O teste depende de um ambiente limpo antes de começar.
// - O IDisposable existe para garantir que a liberação aconteça no momento
//   certo, sem depender do comportamento imprevisível do Garbage Collector.
// - O finalizador existe apenas como rede de segurança.
// -----------------------------------------------------------------------------