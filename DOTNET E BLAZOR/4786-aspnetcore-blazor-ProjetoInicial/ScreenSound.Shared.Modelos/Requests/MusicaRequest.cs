using System.ComponentModel.DataAnnotations;

namespace ScreenSound.Shared.Modelos.Requests;

public record MusicaRequest([Required] string Nome, [Required] int ArtistaId, int AnoLancamento, int GeneroId);

