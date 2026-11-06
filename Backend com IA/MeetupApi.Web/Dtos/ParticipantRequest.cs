using System.ComponentModel.DataAnnotations;

namespace MeetupApi.Web.Dtos;

public record ParticipantRequest(
    [Required(AllowEmptyStrings = false)] string Nome,
    [Required][EmailAddress] string Email);
