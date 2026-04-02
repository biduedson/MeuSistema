using System.Reflection;

namespace MeuSistema.SharedKernel.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAllTypeOf<TInterface>(this Assembly assembly)
    {
        var isAssignableToInterface = typeof(TInterface).IsAssignableFrom;
        return [.. assembly
              .GetTypes()
              .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && isAssignableToInterface(type))];
    }
}

/*
📌 Explicação detalhada

1. using System.Reflection;
   - Importa o namespace que permite usar reflection, ou seja, inspecionar assemblies e tipos em tempo de execução.

2. namespace MeuSistema.SharedKernel.Extensions;
   - Define o espaço de nomes. Colocar no SharedKernel faz sentido porque é uma utilidade genérica que pode ser usada em várias camadas.

3. public static class AssemblyExtensions
   - Cria uma classe estática de extensões. Isso permite adicionar métodos ao tipo Assembly sem precisar instanciá-lo.

4. public static IEnumerable<Type> GetAllTypeOf<TInterface>(this Assembly assembly)
   - Método de extensão para Assembly. Ele retorna todos os tipos (Type) dentro de um assembly que implementam ou herdam de uma interface ou classe base (TInterface).

5. var isAssignableToInterface = typeof(TInterface).IsAssignableFrom;
   - Cria uma função que verifica se um tipo pode ser atribuído a TInterface. É o critério usado para saber se uma classe implementa ou herda da interface/base.

6. assembly.GetTypes()
   - Obtém todos os tipos definidos dentro do assembly (classes, interfaces, enums, etc.).

7. .Where(...)
   - Aplica filtros para garantir que só classes concretas sejam retornadas:
     • type.IsClass → pega apenas classes.
     • !type.IsAbstract → exclui classes abstratas.
     • !type.IsInterface → exclui interfaces.
     • isAssignableToInterface(type) → garante que a classe implementa ou herda de TInterface.

8. return [.. ...]
   - Retorna a lista final de tipos encontrados como IEnumerable<Type>.

📌 Usabilidade no contexto
Esse método é útil em arquiteturas como DDD/CQRS porque permite:
- Registrar automaticamente implementações no container de DI (ex.: todos os repositórios ou handlers).
- Escalabilidade: se novas classes forem adicionadas, elas já são descobertas sem precisar mexer no código de configuração.
- Extensibilidade: facilita criar sistemas de plugins ou módulos que seguem uma interface comum.

👉 Em resumo: ele automatiza a descoberta de classes concretas que implementam uma interface/base, reduzindo código repetitivo e tornando a aplicação mais dinâmica e escalável.
*/
