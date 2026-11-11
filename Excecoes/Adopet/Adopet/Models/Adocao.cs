using System.ComponentModel.DataAnnotations;
using Adopet.Models.Enums;

namespace Adopet.Models;

public class Adocao
{
    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    public long TutorId { get; set; }
    public virtual Tutor Tutor { get; set; }
    [Required]
    public long PetId { get; set; }
    public virtual Pet Pet { get; set; }
    public string Motivo { get; set; }
    public StatusAdocao Status { get; set; }
    public string? Justificativa { get; set; }

    public Adocao()
    {
    }

    public Adocao(Tutor tutor, Pet pet, string motivo)
    {
        Tutor = tutor;
        Pet = pet;
        Motivo = motivo;
        Status = StatusAdocao.AGUARDANDO_AVALIACAO;
    }

    public void MarcarComoAprovada()
    {
        Status = StatusAdocao.APROVADO;
    }

    public void MarcarComoReprovada(string justificativa)
    {
        Status = StatusAdocao.REPROVADO;
        Justificativa = justificativa;
    }
}
