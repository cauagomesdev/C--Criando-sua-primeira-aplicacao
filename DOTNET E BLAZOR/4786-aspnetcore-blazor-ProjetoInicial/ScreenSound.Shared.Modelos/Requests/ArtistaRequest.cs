using System.ComponentModel.DataAnnotations;

namespace ScreenSound.Shared.Modelos.Requests;
public record ArtistaRequest([Required] string Nome, [Required] string Bio, string? FotoPerfil);

