using Adopet.Models;

namespace Adopet.Api.Dto;

public record TutorDto(long Id, string Nome, string Email)
{
    public TutorDto(Tutor tutor) : this(tutor.Id, tutor.Nome, tutor.Email)
    {
    }
}
