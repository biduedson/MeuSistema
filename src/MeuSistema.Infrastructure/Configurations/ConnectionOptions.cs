

using System.ComponentModel.DataAnnotations;

namespace MeuSistema.Infrastructure.Configurations;

public sealed class ConnectionOptions
{
    public const string ConfigSectionPath = "ConnectionStrings";

    [Required]
    public string DefaultConnection { get; init; } 
}
