using MeuSistema.API.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace MeuSistema.API.Extensions;

internal static class MiddlewareExtensions
{
    public static void UseErrorHanDling(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}



// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe define uma extensão para registrar o middleware
// de tratamento de erros na pipeline da aplicação.
//
// 1) MiddlewareExtensions → Classe estática que contém métodos de extensão.
// 2) UseErrorHanDling → Método de extensão para IApplicationBuilder.
//    - Permite adicionar o ErrorHandlingMiddleware de forma simples.
//    - Exemplo de uso no Program.cs:
//        app.UseErrorHanDling();
// 3) builder.UseMiddleware<ErrorHandlingMiddleware>() →
//    - Insere o middleware na pipeline.
//    - Garante que todas as requisições passem pelo tratamento de erros.
// 4) Benefício → Centraliza o registro do middleware,
//    deixando o Program.cs mais limpo e organizado.
// ------------------------------------------------------
