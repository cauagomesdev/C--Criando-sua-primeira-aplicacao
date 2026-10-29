namespace ScreenSound.Shared.Modelos.Requests;

public record MusicaRequestEdit(int Id, string Nome, int ArtistaId, int AnoLancamento, int GeneroId)
    : MusicaRequest(Nome, ArtistaId, AnoLancamento, GeneroId);