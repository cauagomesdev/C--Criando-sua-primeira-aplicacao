using System.ComponentModel.DataAnnotations;

namespace Adopet.Api.Dto;
public record CadastroTutorDto
{
    [Required]
    public string Nome { get; init; }

    [Required]
    public string Email { get; init; }
}
