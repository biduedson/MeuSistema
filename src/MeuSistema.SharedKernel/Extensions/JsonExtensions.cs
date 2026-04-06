using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MeuSistema.SharedKernel.Extensions;

public static class JsonExtensions
{
    private static readonly Lazy<JsonSerializerOptions> LazyOptions =
        new(() => new JsonSerializerOptions().Configure(), isThreadSafe: true);

    public static T FromJson<T>(this string value) =>
        value != null ? JsonSerializer.Deserialize<T>(value, LazyOptions.Value) : default;

    public static string ToJson<T>(this T value) =>
       !value.IsDefault() ? JsonSerializer.Serialize(value, LazyOptions.Value) : default;

    public static JsonSerializerOptions Configure(this JsonSerializerOptions jsonSettings)
    {
        jsonSettings.WriteIndented = false;
        jsonSettings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSettings.ReadCommentHandling = JsonCommentHandling.Skip;
        jsonSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSettings.TypeInfoResolver = new PrivateConstructorContractResolver();
        jsonSettings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return jsonSettings;
    }
}

internal sealed class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object
            && jsonTypeInfo.CreateObject is null
            && jsonTypeInfo.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
        {
            jsonTypeInfo.CreateObject = () => Activator.CreateInstance(jsonTypeInfo.Type, true)!;
        }

        return jsonTypeInfo;
    }
}

/*
📌 Explicação linha a linha

- using System.Reflection → necessário para inspecionar construtores via reflection.
- using System.Text.Json / Serialization / Metadata → importa o motor JSON nativo e recursos avançados de serialização.
- namespace MeuSistema.SharedKernel.Extensions → define que essa utilidade pertence ao SharedKernel, podendo ser usada em qualquer camada.
- public static class JsonExtensions → classe estática que concentra métodos auxiliares para trabalhar com JSON.
- LazyOptions → cria uma instância preguiçosa de JsonSerializerOptions, inicializada só quando usada (performance + thread safety).
- FromJson<T> → extensão para string, converte JSON em objeto do tipo T usando as opções configuradas.
- ToJson<T> → extensão para qualquer objeto, converte em string JSON aplicando as mesmas opções.
- Configure(JsonSerializerOptions) → método de extensão que ajusta as opções:
   • WriteIndented = false → JSON compacto.
   • DefaultIgnoreCondition = WhenWritingNull → ignora propriedades nulas.
   • ReadCommentHandling = Skip → ignora comentários.
   • PropertyNamingPolicy = CamelCase → propriedades em camelCase.
   • TypeInfoResolver = PrivateConstructorContractResolver → permite instanciar classes com construtores privados.
   • Converters.Add(JsonStringEnumConverter) → enums como string em camelCase.
- PrivateConstructorContractResolver → resolver customizado que habilita deserializar classes sem construtor público.
- GetTypeInfo override → se o tipo é objeto, não tem construtor público e CreateObject é nulo, define uma função para criar instância via Activator.CreateInstance.

✅ Em resumo: esse código cria um helper robusto para serialização JSON, centralizando configurações e permitindo deserializar entidades com construtores privados — algo comum em DDD. Facilita conversão de objetos ↔ JSON de forma consistente e escalável.
*/
