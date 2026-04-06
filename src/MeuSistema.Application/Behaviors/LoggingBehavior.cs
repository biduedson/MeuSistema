using System.Diagnostics;
using MediatR;
using MeuSistema.SharedKernel.Extensions;
using Microsoft.Extensions.Logging;

namespace MeuSistema.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var commandName = request.GetGenericTypeName();

            logger.LogInformation("----- Iniciando processamento do comando '{CommandName}'", commandName);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next(cancellationToken);

            timer.Stop();

            var timeTaken = timer.Elapsed.TotalSeconds;

            logger.LogInformation("----- Comando '{CommandName}' processado em {TimeTaken} segundos", commandName, timeTaken);

            return response;
        }
    }
}

/*
📌 O que essa classe faz:
- Essa classe é um **Pipeline Behavior** do MediatR.
- Ela intercepta cada requisição (command/query) antes e depois do handler real.
- O objetivo é adicionar **logging** e medir o tempo de execução de cada comando.
- Funciona como um "middleware interno" do MediatR.

📌 Por que foi criada:
- Para centralizar lógica de logging sem precisar repetir em cada handler.
- Para monitorar performance dos comandos e consultas.
- Para facilitar rastreamento e diagnóstico de problemas.
- Para seguir o princípio de separação de responsabilidades (handlers focam na lógica de negócio, behaviors cuidam de cross-cutting concerns).

📌 Explicação linha a linha:
1. public class LoggingBehavior<TRequest, TResponse>(ILogger<...> logger)
   → Classe genérica com dois tipos (TRequest e TResponse).
   → Usa primary constructor (C# 12) para injetar automaticamente o logger.

2. : IPipelineBehavior<TRequest, TResponse>
   → Implementa a interface do MediatR que permite interceptar requisições.
   → Funciona como middleware interno.

3. where TRequest : IRequest<TResponse>
   → Restrição genérica: garante que TRequest seja um request válido do MediatR que retorna TResponse.

4. Handle(...)
   → Método obrigatório da interface.
   → Recebe o request, o delegate para chamar o próximo handler e o token de cancelamento.

5. request.GetGenericTypeName()
   → Obtém o nome do tipo do comando para logar de forma legível.

6. logger.LogInformation(...) (início)
   → Loga que o processamento do comando começou.

7. Stopwatch
   → Cria um cronômetro para medir tempo de execução.
   → Start() inicia a contagem.

8. await next(cancellationToken)
   → Chama o próximo handler no pipeline (o handler real da requisição).
   → Aguarda a resposta.

9. timer.Stop() e timer.Elapsed.TotalSeconds
   → Para o cronômetro e calcula o tempo decorrido em segundos.

10. logger.LogInformation(...) (fim)
    → Loga que o comando terminou e quanto tempo levou.

11. return response
    → Retorna a resposta do handler para continuar o fluxo normal.
*/
