using MeuSistema.API.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace MeuSistema.API.Extensions;

internal static class MiddlewareExtensions
{
    public static void UseErrorHanDling(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}
