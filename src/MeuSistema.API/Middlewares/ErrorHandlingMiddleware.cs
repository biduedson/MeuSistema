using System;
using System.Net.Mime;
using System.Threading.Tasks;
using MeuSistema.API.Models;
using MeuSistema.SharedKernel.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeuSistema.API.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IHostEnvironment environment
)
{
    private const string ErrorMessage = "Ocorreu um erro interno ao processar sua solicitação";

    private static readonly string ApiResponseJson = ApiResponse.InternalServerError(ErrorMessage).ToJson();

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Uma exceção inesperada foi lançada: {Message}", ex.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (environment.IsDevelopment())
            {
                httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
                await httpContext.Response.WriteAsync($"Ocorreu um erro interno: {ex}");
            }
            else
            {
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                await httpContext.Response.WriteAsync(ApiResponseJson);
            }
        }
    }
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Esse middleware é responsável por capturar exceções não tratadas da aplicação.
//
// 1) Objetivo principal
// → Interceptar erros inesperados que acontecem durante o processamento da requisição.
// → Evita que a API quebre sem controle e garante uma resposta padrão ao cliente.
//
// 2) RequestDelegate next
// → Representa o próximo middleware da pipeline.
// → O método "await next(httpContext)" permite que a requisição continue seu fluxo normal.
//
// 3) ILogger<ErrorHandlingMiddleware>
// → Usado para registrar logs de erro.
// → Isso ajuda a monitorar falhas e investigar problemas na aplicação.
//
// 4) IHostEnvironment environment
// → Permite identificar o ambiente atual da aplicação.
// → Exemplo: Development, Staging ou Production.
//
// 5) ErrorMessage
// → Mensagem padrão de erro interno que será enviada ao cliente em produção.
//
// 6) ApiResponseJson
// → Resposta de erro já padronizada em formato JSON.
// → Usa o modelo ApiResponse para manter consistência na API.
//
// 7) Método Invoke(HttpContext httpContext)
// → Método principal do middleware.
// → É executado automaticamente para cada requisição HTTP.
//
// 8) Bloco try
// → Tenta executar normalmente o restante da pipeline da aplicação.
//
// 9) Bloco catch(Exception ex)
// → Captura qualquer exceção não tratada que ocorrer durante a requisição.
//
// 10) logger.LogError(...)
// → Registra o erro no sistema de logs.
// → Inclui a exceção e a mensagem detalhada para facilitar análise.
//
// 11) StatusCode = 500
// → Define a resposta HTTP como erro interno do servidor.
//
// 12) environment.IsDevelopment()
// → Se a aplicação estiver em ambiente de desenvolvimento,
//   retorna detalhes completos do erro para facilitar debug.
//
// 13) Resposta em Development
// → Retorna texto puro com detalhes da exceção.
// → Útil para testes e depuração local.
//
// 14) Resposta em Production
// → Retorna uma resposta JSON padronizada e segura.
// → Evita expor detalhes internos do sistema ao cliente.
//
// 15) Benefício final
// → Centraliza o tratamento de erros da API.
// → Melhora segurança, manutenção, padronização e experiência do consumidor da API.
// ------------------------------------------------------