namespace MeuSistema.SharedKernel.Primitives;

/// <summary>
/// Representa a interface para opções de configuração da aplicação.
/// </summary>
public interface IAppOptions
{
    /// <summary>
    /// O caminho da seção de configuração no arquivo de configuração da aplicação.
    /// Essa propriedade estática define onde os valores de configuração podem ser encontrados.
    /// </summary>
    static abstract string ConfigSectionPath { get; }
}

// -----------------------------------------
// 🔹 EXPLICAÇÃO DO CÓDIGO 🔹
// -----------------------------------------
/*
✅ Interface IAppOptions → Define um contrato para classes que representam opções de configuração da aplicação. 
✅ Propriedade estática ConfigSectionPath → Indica onde as configurações relacionadas à opção podem ser encontradas no arquivo de configuração. 
✅ Essa interface é utilizada para garantir que diferentes tipos de configuração sigam um padrão e possam ser validados corretamente. 
*/
