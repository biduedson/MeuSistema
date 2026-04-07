using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace MeuSistema.API.Models;

public class ApiResponse
{
    [JsonConstructor]
    public ApiResponse(bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
    {
        Success = success;
        SuccessMessage = successMessage;
        StatusCode = statusCode;
        Errors = errors;
    }

    public ApiResponse() { }

    public bool Success { get; protected init; }
    public string SuccessMessage { get; protected init; }
    public int StatusCode { get; protected init; }
    public IEnumerable<ApiErrorResponse> Errors { get; private init; }

    public static ApiResponse Ok() =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK };

    public static ApiResponse Ok(string successMessage) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    public static ApiResponse BadRequest() =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse BadRequest(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateErrors(errorMessage) };
    public static ApiResponse BadRequest(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    public static ApiResponse Unauthorized() =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse Unauthorized(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    public static ApiResponse Forbidden() =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse Forbidden(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateErrors(errorMessage) };

    public static ApiResponse NotFound() =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse NotFound(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateErrors(errorMessage) };

    public static ApiResponse NotFound(IEnumerable<ApiErrorResponse> errors) =>
      new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    public static ApiResponse InternalServerError(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateErrors(errorMessage) };

    private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
        [new ApiErrorResponse(errorMessage)];

    public override string ToString() =>
        $"Success: {Success} | StatusCode: {StatusCode} | HasErrors: {Errors.Any()}";
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe representa um modelo padrão de resposta da API.
//
// 1) Objetivo da classe
// → Padronizar os retornos da API em um único formato.
// → Evita retornar respostas soltas e inconsistentes em cada endpoint.
//
// 2) Estrutura principal da resposta
// → Success        = indica se a operação foi bem-sucedida
// → SuccessMessage = mensagem de sucesso opcional
// → StatusCode     = código HTTP da resposta
// → Errors         = coleção de erros retornados pela API
//
// 3) [JsonConstructor]
// → Indica ao System.Text.Json qual construtor usar ao desserializar o objeto.
// → Útil quando a classe possui mais de um construtor.
//
// 4) Construtor vazio
// → Permite criar o objeto com inicialização por propriedades usando "new() { ... }".
// → Isso é usado nos métodos estáticos da própria classe.
//
// 5) protected init / private init
// → Permitem que os valores sejam definidos apenas na criação do objeto.
// → Isso ajuda a manter a resposta imutável após ser montada.
//
// 6) Métodos estáticos (Factory Methods)
// → A classe fornece métodos prontos para criar respostas padronizadas:
//    - Ok()                  → 200
//    - BadRequest()          → 400
//    - Unauthorized()        → 401
//    - Forbidden()           → 403
//    - NotFound()            → 404
//    - InternalServerError() → 500
//
// 7) Vantagem desses métodos
// → Evitam repetição de código nos controllers/endpoints.
// → Em vez de montar a resposta manualmente toda vez,
//   basta chamar o método correspondente.
//
// 8) CreateErrors(string errorMessage)
// → Método auxiliar que transforma uma string simples
//   em uma lista de ApiErrorResponse.
// → Isso garante que o campo Errors sempre siga o mesmo padrão.
//
// 9) ToString()
// → Retorna um resumo textual do objeto.
// → Útil para logs, debug e inspeção rápida da resposta.
//
// 10) Benefício final
// → Centraliza e padroniza o contrato de resposta da API.
// → Facilita manutenção, documentação e consumo pelo front-end.
// ------------------------------------------------------