using Adopet.Models;
using Adopet.Models.Enums;

namespace Adopet.Dtos;

public record PetDto(long Id, string Nome, int Idade, TipoPet Tipo, bool Adotado, string Imagem)
{
    public PetDto(Pet pet) : this(pet.Id, pet.Nome, pet.Idade, pet.Tipo, pet.Adotado, pet.Imagem)
    {
    }
}
