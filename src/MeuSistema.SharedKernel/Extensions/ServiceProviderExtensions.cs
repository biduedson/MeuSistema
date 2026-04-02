using MeuSistema.SharedKernel.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MeuSistema.SharedKernel.Extensions;

public static class ServiceProviderExtensions
{
    public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class, IAppOptions =>
        serviceProvider
            .GetService<IOptions<TOptions>>()?.Value!;
}

/*
📌 Explicação detalhada

1. using MeuSistema.SharedKernel.Primitives;
   - Importa tipos definidos no seu domínio, incluindo a interface IAppOptions que marca classes de opções de configuração.

2. using Microsoft.Extensions.DependencyInjection;
   - Permite trabalhar com o container de injeção de dependência (DI) do .NET.

3. using Microsoft.Extensions.Options;
   - Importa o suporte ao Options Pattern, que fornece acesso fortemente tipado a configurações via IOptions<T>.

4. namespace MeuSistema.SharedKernel.Extensions;
   - Define o namespace. Colocar no SharedKernel faz sentido porque é uma extensão genérica que pode ser usada em várias camadas.

5. public static class ServiceProviderExtensions
   - Cria uma classe estática de extensões. Isso adiciona métodos ao tipo IServiceProvider sem precisar instanciá-lo.

6. public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider)
   - Método de extensão para IServiceProvider. Ele retorna uma instância de opções de configuração do tipo TOptions.
   - O where TOptions : class, IAppOptions garante que só classes de opções que implementam IAppOptions possam ser usadas.

7. serviceProvider.GetService<IOptions<TOptions>>()?.Value!;
   - Pede ao container de DI uma instância de IOptions<TOptions>.
   - ?.Value acessa o objeto de configuração real.
   - O operador ! (null-forgiving) diz ao compilador para confiar que não será nulo.
   - Resultado: retorna o objeto de opções configurado e pronto para uso.

📌 Usabilidade no contexto
Esse método simplifica o acesso às configurações tipadas:
- Em vez de injetar IOptions<T> em cada classe, você pode pedir diretamente TOptions ao IServiceProvider.
- Facilita cenários onde você precisa resolver opções dinamicamente em tempo de execução.
- Útil em arquiteturas DDD/CQRS para centralizar como opções são obtidas, mantendo o código mais limpo e consistente.

👉 Em resumo: essa extensão é um atalho para recuperar opções configuradas via Options Pattern diretamente do IServiceProvider, tornando o acesso às configurações mais simples e padronizado.
*/
