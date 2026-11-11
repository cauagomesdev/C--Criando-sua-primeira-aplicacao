using Adopet.Models;
using Adopet.Models.Enums;

namespace Adopet.Dtos;

public record AdocaoDto(long Id, long Tutor, long Pet, string Motivo, StatusAdocao Status, string Justificativa)
{
    public AdocaoDto(Adocao adocao) : this(adocao.Id, adocao.Tutor.Id, adocao.Pet.Id, adocao.Motivo, adocao.Status, adocao.Justificativa)
    {
    }
}
