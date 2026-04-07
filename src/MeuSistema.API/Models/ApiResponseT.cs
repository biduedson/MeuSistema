using System.Text.Json.Serialization;

namespace MeuSistema.API.Models;

public sealed class ApiResponse<TResult> : ApiResponse
{
    [JsonConstructor]
    public ApiResponse(
        TResult result,
        bool success,
        string successMessage,
        int statusCode,
        IEnumerable<ApiErrorResponse> errors
    ) : base(success, successMessage, statusCode, errors)
    {
        Result = result;
    }

    public ApiResponse() { }

    public TResult Result { get; private init; }

    public static ApiResponse<TResult> Ok(TResult result) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result };

    public static ApiResponse<TResult> Ok(TResult result, string successMessage) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };

    public static ApiResponse<TResult> Created(TResult result) =>
        new() { Success = true, StatusCode = StatusCodes.Status201Created, Result = result };
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe representa uma resposta padrão da API com retorno de dados.
//
// 1) ApiResponse<TResult>
// → É uma versão genérica da classe ApiResponse.
// → Permite retornar, além do status e mensagens, um dado tipado no campo Result.
//
// 2) TResult
// → Representa o tipo do dado retornado.
// → Exemplo:
//    - ApiResponse<string>
//    - ApiResponse<UsuarioDto>
//    - ApiResponse<List<ProdutoDto>>
//
// 3) Herança de ApiResponse
// → Essa classe herda da classe base ApiResponse.
// → Assim, ela reutiliza os campos comuns:
//    - Success
//    - SuccessMessage
//    - StatusCode
//    - Errors
//
// 4) [JsonConstructor]
// → Indica ao System.Text.Json qual construtor usar na desserialização.
// → Importante quando a classe possui construtor customizado.
//
// 5) Result
// → Propriedade que armazena o dado retornado pela API.
// → É o payload da resposta em caso de sucesso.
//
// 6) Método Ok(TResult result)
// → Cria uma resposta HTTP 200 (OK) com dados.
// → Usado quando a operação foi bem-sucedida e há conteúdo para retornar.
//
// 7) Método Ok(TResult result, string successMessage)
// → Igual ao anterior, mas permite incluir uma mensagem de sucesso.
//
// 8) Método Created(TResult result)
// → Cria uma resposta HTTP 201 (Created) com dados.
// → Usado normalmente quando um novo recurso foi criado com sucesso.
//
// 9) Benefício principal
// → Padroniza respostas de sucesso com payload.
// → Facilita o consumo da API pelo front-end e mantém consistência no contrato.
//
// 10) Exemplo prático de uso
// → ApiResponse<UsuarioDto>.Ok(usuario)
// → ApiResponse<Guid>.Created(id)
// → ApiResponse<List<ProdutoDto>>.Ok(lista, "Produtos carregados com sucesso")
// ------------------------------------------------------