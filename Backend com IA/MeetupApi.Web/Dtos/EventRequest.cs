using System.ComponentModel.DataAnnotations;

namespace MeetupApi.Web.Dtos;

public record EventRequest(
    [Required(AllowEmptyStrings = false)] string Titulo,
    string? Descricao,
    [Required] DateTime? DataHora,
    string? Local,
    [Required][Range(1, int.MaxValue)] int? LimiteVagas);
