using System.Text.RegularExpressions;

namespace MeuSistema.Domain.Shared;

public static partial class RegexPatterns
{
    public static readonly Regex EmailIsValid = EmailRegexPatternAttr();

    [GeneratedRegex(@"^([0-9a-zA-Z]([+\-_.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$",
       RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegexPatternAttr();
}

// using System.Text.RegularExpressions
// → importa a lib de Regex do .NET

// static partial class RegexPatterns
// → static   = não precisa instanciar, acessa direto pela classe
// → partial  = permite o .NET gerar código automático em tempo de compilação
// → agrupa todos os padrões Regex do sistema em um só lugar

// EmailIsValid
// → variável pública que guarda o Regex pronto para uso
// → qualquer lugar do sistema chama: RegexPatterns.EmailIsValid

// EmailRegexPatternAttr()
// → método privado que o [GeneratedRegex] usa para gerar o Regex

// [GeneratedRegex(...)]
// → atributo do .NET 7+ que gera o Regex em tempo de compilação
// → mais rápido que criar Regex em tempo de execução
// → o padrão valida se o email tem formato correto: nome@dominio.com

// RegexOptions.IgnoreCase        → ignora maiúsculas e minúsculas
// RegexOptions.Compiled          → compila o Regex para máxima performance
// RegexOptions.CultureInvariant  → funciona igual em qualquer idioma/região