using System.ComponentModel.DataAnnotations;

namespace Adopet.Dtos;

public record AprovarAdocaoDto
{
    [Required]
    public long IdAdocao { get; init; }
}
