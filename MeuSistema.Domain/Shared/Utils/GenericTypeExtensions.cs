namespace MeuSistema.Domain.Shared.Utils;

public static class GenericTypeExtensions
{
    public static bool IsDefault<T>(this T value) =>
        Equals(value, default(T));

    public static string GenericTypeName(this object @object)
    {
        var type = @object.GetType();

        if (!type.IsGenericType)
            return type.Name;

        var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());

        return $"{type.Name[..type.Name.IndexOf('`')]}<{genericTypes}>";
    }
}

/*
------------------------------------------------------------
📌 Explicação didática do código

- Essa classe é estática e serve para criar **métodos de extensão**.

1) IsDefault<T>
   - Verifica se o valor é o padrão do tipo T.
   - Exemplos:
     int x = 0; x.IsDefault() → true
     string s = null; s.IsDefault() → true
     bool b = false; b.IsDefault() → true

2) GenericTypeName
   - Retorna o nome legível do tipo de um objeto.
   - Passo a passo:
     a) Pega o tipo real com GetType().
     b) Se não for genérico → retorna direto o nome (ex.: Int32, String, Pessoa, Int32[]).
     c) Se for genérico → pega os tipos concretos usados (ex.: Int32, String).
     d) Remove o sufixo interno feio (List`1, Dictionary`2) usando IndexOf('`') e [..].
     e) Monta o nome bonito com os tipos entre < >.

- Exemplos de saída:
   42.GenericTypeName() → "Int32"
   "Edson".GenericTypeName() → "String"
   new int[]{1,2,3}.GenericTypeName() → "Int32[]"
   new List<int>().GenericTypeName() → "List<Int32>"
   new Dictionary<string,int>().GenericTypeName() → "Dictionary<String,Int32>"

👉 Assim, você consegue imprimir nomes de tipos de forma clara e legível.
------------------------------------------------------------
*/
