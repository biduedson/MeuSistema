using System.Text.Json.Serialization;

namespace MeuSistema.Domain.Entities.CustumerAggregate;


[JsonConverter(typeof(JsonStringEnumConverter<EGender>))]
public enum EGender
{
    None = 0,   
    Male = 1,   
    Female = 2, 
}

/*
------------------------------------------------------------
📌 Explicação simples:

- Esse código define um enum chamado EGender, que representa gênero:
  None (nenhum), Male (masculino), Female (feminino).

- A marcação [JsonConverter(typeof(JsonStringEnumConverter<EGender>))]
  serve para que, ao serializar em JSON, os valores sejam convertidos
  para texto legível ("Male", "Female") em vez de números (1, 2).

🎯 Exemplo prático:
- Sem o converter: { "gender": 1 }
- Com o converter: { "gender": "Male" }

👉 Em resumo:
O enum tipa o gênero no domínio e o atributo garante que, em JSON,
ele apareça como string legível, não como número.
------------------------------------------------------------
*/
