namespace MeetupApi.Web.Dtos;

public record EventResponse(
    long Id,
    string Titulo,
    string? Descricao,
    DateTime DataHora,
    string? Local,
    int LimiteVagas,
    int VagasRestantes);
