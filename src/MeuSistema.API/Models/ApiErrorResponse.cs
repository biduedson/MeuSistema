using System.Text.Json.Serialization;

namespace MeuSistema.API.Models;

[method: JsonConstructor]
public sealed class ApiErrorResponse(string message)
{
    public string Message { get; } = message;

    public override string ToString() => Message;
}

// ------------------------------------------------------
// 🔹 EXPLICAÇÃO RESUMIDA E CLARA 🔹
// ------------------------------------------------------
// Essa classe representa uma resposta padrão de erro da API.
//
// 1) ApiErrorResponse
// → Modelo simples usado para retornar mensagens de erro ao cliente.
// → Exemplo de retorno em JSON:
//    { "message": "Recurso não encontrado." }
//
// 2) string message
// → Parâmetro recebido no construtor primário da classe.
// → É o texto que será exposto na propriedade Message.
//
// 3) public string Message { get; }
// → Propriedade somente leitura.
// → Garante que a mensagem de erro seja definida na criação do objeto
//   e não possa ser alterada depois.
//
// 4) [method: JsonConstructor]
// → Indica ao System.Text.Json qual construtor deve ser usado
//   na serialização/desserialização do objeto.
// → Útil principalmente quando a classe usa construtor primário.
//
// 5) sealed
// → Impede que essa classe seja herdada.
// → Isso ajuda a manter o modelo fechado e simples para resposta da API.
//
// 6) ToString()
// → Retorna diretamente a mensagem de erro.
// → Facilita logs, debug e exibição rápida do conteúdo do objeto.
//
// 7) Objetivo final
// → Padronizar respostas de erro da API
// → Evitar retornar strings soltas ou formatos inconsistentes
// → Deixar a comunicação da API mais organizada e previsível
// ------------------------------------------------------