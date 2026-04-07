using System.Linq;
using Ardalis.Result;
using MeuSistema.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeuSistema.API.Extensions;

internal static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.Ok(result.SuccessMessage))
            : result.ToHttpNonSuccessResult();

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsCreated())
        {
            return new CreatedResult(result.Location, ApiResponse<T>.Created(result.Value));
        }
        else if (result.IsOk())
        {
            return new OkObjectResult(ApiResponse<T>.Ok(result.Value, result.SuccessMessage));
        }
        else
        {
            return result.ToHttpNonSuccessResult();
        }
    }

    private static IActionResult ToHttpNonSuccessResult(this IResult result)
    {
        var errors = result.Errors.Select(error => new ApiErrorResponse(error)).ToList();

        switch (result.Status)
        {
            case ResultStatus.Invalid:
                var validationErrors = result.ValidationErrors
                    .Select(validation => new ApiErrorResponse(validation.ErrorMessage));
                return new BadRequestObjectResult(ApiResponse.BadRequest(validationErrors));

            case ResultStatus.NotFound:
                return new NotFoundObjectResult(ApiResponse.NotFound(errors));

            case ResultStatus.Forbidden:
                return new ForbidResult();

            case ResultStatus.Unauthorized:
                return new UnauthorizedObjectResult(ApiResponse.Unauthorized(errors));

            default:
                return new BadRequestObjectResult(ApiResponse.BadRequest(errors));
        }
    }
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe contém métodos de extensão para converter objetos do Ardalis.Result
// em respostas HTTP padronizadas da API.
//
// 1) Objetivo principal
// → Transformar resultados da camada de aplicação/domínio em IActionResult.
// → Isso evita repetir lógica de retorno HTTP em cada controller/endpoint.
//
// 2) ToActionResult(this Result result)
// → Converte um Result sem payload (sem dados).
// → Se a operação for sucesso, retorna HTTP 200 (OK).
// → Se houver falha, redireciona para o método que trata erros.
//
// 3) ToActionResult<T>(this Result<T> result)
// → Converte um Result genérico com dados.
// → Se o resultado for Created, retorna HTTP 201.
// → Se for sucesso comum, retorna HTTP 200.
// → Se falhar, converte automaticamente para resposta de erro HTTP.
//
// 4) Result<T>
// → Representa um resultado com payload.
// → Exemplo:
//    - Result<Guid>
//    - Result<CustomerDto>
//    - Result<List<ProductDto>>
//
// 5) ToHttpNonSuccessResult(this IResult result)
// → Método privado responsável por traduzir falhas em respostas HTTP adequadas.
// → Ele interpreta o status do Ardalis.Result e devolve o código correto da API.
//
// 6) Mapeamento dos erros
// → ResultStatus.Invalid      → 400 Bad Request
// → ResultStatus.NotFound     → 404 Not Found
// → ResultStatus.Forbidden    → 403 Forbidden
// → ResultStatus.Unauthorized → 401 Unauthorized
// → Outros casos              → 400 Bad Request
//
// 7) Conversão de erros
// → As mensagens de erro do Ardalis.Result são transformadas em ApiErrorResponse.
// → Isso mantém o padrão de resposta da API consistente.
//
// 8) Benefício principal
// → Centraliza a conversão de Result para IActionResult.
// → Deixa controllers/endpoints mais limpos.
// → Facilita manutenção, padronização e escalabilidade da API.
//
// 9) Exemplo de uso
// → return result.ToActionResult();
// → return customerResult.ToActionResult();
// ------------------------------------------------------