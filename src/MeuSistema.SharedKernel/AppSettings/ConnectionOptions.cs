

using System.ComponentModel.DataAnnotations;
using MeuSistema.SharedKernel.Primitives;

namespace MeuSistema.SharedKernel.AppSettings;

public sealed class ConnectionOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => "ConnectionStrings";

    [Required]
    public string PostGreeSqlConnection { get; init; } 
}
