namespace ScreenSound.Shared.Modelos.Requests;

public record ArtistaRequestEdit(int Id, string Nome, string Bio, string? FotoPerfil)
    : ArtistaRequest(Nome, Bio, FotoPerfil);
