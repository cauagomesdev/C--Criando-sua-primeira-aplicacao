namespace MeetupApi.Web.Dtos;

public record RegistrationResponse(
    long Id,
    long EventoId,
    string NomeParticipante,
    string EmailParticipante,
    DateTime DataInscricao,
    string Status);
