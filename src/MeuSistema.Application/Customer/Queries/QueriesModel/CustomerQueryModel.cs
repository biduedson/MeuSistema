

using MeuSistema.Application.Abstractions;

namespace MeuSistema.Application.Customer.Queries.QueriesModel;

public class CustomerQueryModel : IQueryModel<Guid>
{
    public CustomerQueryModel(
        Guid id,
        string firstName,
        string lastName,
        string gender,
        string email,
        DateTime dateOfBirth
        )
    {
        Id= id;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;

    }

    private CustomerQueryModel() { }

    public Guid Id { get; private init ; }    
    public string FirstName { get; private init; } 
    public string LastName { get; private init; }
    public string Gender { get; private init; }
    public string Email { get; private init; }
    public DateTime DateOfBirth { get; private init; }


}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DETALHADA 🔹
// -----------------------------------------

/*
✅ Classe CustomerQueryModel → Representa o modelo de dados retornado pelas consultas (queries) relacionadas a clientes.
✅ Implementação de IQueryModel<Guid> → Define um contrato genérico para modelos de consulta, garantindo consistência e reutilização.
✅ Construtor público → Permite inicializar o objeto com todos os atributos necessários de forma explícita.
✅ Construtor privado → Necessário para compatibilidade com EF Core, que exige um construtor sem parâmetros para materialização.
✅ Propriedades com init-only → Garantem imutabilidade após a inicialização, reforçando segurança e previsibilidade dos dados.
✅ Arquitetura CQRS → Separa claramente os modelos de leitura (query models) dos modelos de escrita (entities), facilitando manutenção e escalabilidade.
*/
