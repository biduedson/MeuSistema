
using MeuSistema.Domain.Exceptions;
using MeuSistema.SharedKernel;

namespace MeuSistema.Domain.ValueObjects;

public sealed record Email
{
    private Email(string address) =>
        Address = address.ToLowerInvariant().Trim();

    public Email () { }
    public string Address { get; }

    public static Email Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ValidationException("O email é obrigatório.");

        if (!RegexPatterns.EmailIsValid.IsMatch(emailAddress))
            throw new ValidationException("O email é inválido.");

        return new Email(emailAddress);
    }

    public override string ToString() => Address;
}

/*
🔹 EXPLICAÇÃO DETALHADA 🔹

1. Declaração da classe  
   - `Email` é um **Value Object** imutável e selado (`sealed record`).  
   - Representa o conceito de e-mail dentro do domínio, encapsulando regras de validação e consistência.  
   - Como é um `record`, possui igualdade estrutural (dois objetos com o mesmo endereço são considerados iguais).

2. Construtor privado  
   - `private Email(string address)` → só pode ser chamado internamente.  
   - Normaliza o e-mail: converte para minúsculas (`ToLowerInvariant`) e remove espaços extras (`Trim`).  
   - Isso garante consistência e evita problemas de comparação.

3. Construtor público vazio  
   - `public Email()` → existe apenas para compatibilidade com frameworks (ex.: EF Core).  
   - Não deve ser usado diretamente para criar instâncias válidas.

4. Propriedade `Address`  
   - Armazena o endereço de e-mail já normalizado.  
   - É somente leitura (`get;`), reforçando a imutabilidade do objeto.

5. Método de fábrica `Create`  
   - Responsável por criar instâncias válidas de `Email`.  
   - Aplica regras de negócio:  
     - Se o e-mail for nulo ou vazio → lança `ValidationException("O email é obrigatório.")`.  
     - Se não corresponder ao padrão regex → lança `ValidationException("O email é inválido.")`.  
   - Retorna uma instância de `Email` já validada e consistente.  
   - Esse padrão garante que **nenhum objeto inválido** seja criado no domínio.

6. `ToString()`  
   - Retorna o valor do `Address`.  
   - Útil para exibir ou logar o e-mail diretamente.

---

✅ Em resumo:  
Esse `ValueObject` garante que **todo e-mail dentro do domínio seja válido e consistente**.  
- O domínio nunca aceita um e-mail inválido, pois a criação só é possível via `Email.Create`.  
- A normalização (minúsculas + trim) evita duplicidade e inconsistência.  
- Exceções são lançadas imediatamente se houver erro, protegendo a integridade do agregado `Customer`.  

Assim, o domínio não fica “burro”: ele mesmo toma a decisão final sobre o que é válido ou não.
*/
