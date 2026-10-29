namespace ScreenSound.Shared.Modelos.Response;

public record MusicaResponse(int Id, string Nome, int ArtistaId, string NomeArtista, int? AnoLancamento, string? FotoPerfil);