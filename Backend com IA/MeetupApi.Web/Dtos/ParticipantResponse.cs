namespace MeetupApi.Web.Dtos;

public record ParticipantResponse(
    long Id,
    string Nome,
    string Email,
    DateTime? CriadoEm);
