using System.ComponentModel.DataAnnotations;

namespace MeetupApi.Web.Dtos;

public record RegistrationRequest(
    [Required] long? ParticipanteId);
