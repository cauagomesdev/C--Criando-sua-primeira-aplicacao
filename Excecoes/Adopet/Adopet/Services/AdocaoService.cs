using Adopet.Dtos;
using Adopet.Exceptions;
using Adopet.Models;
using Adopet.Models.Enums;
using Adopet.Repositories;

namespace Adopet.Services;

public class AdocaoService
{
    private readonly PetRepository _petRepository;
    private readonly TutorRepository _tutorRepository;
    private readonly AdocaoRepository _adocaoRepository;

    public AdocaoService(
        PetRepository petRepository,
        TutorRepository tutorRepository,
        AdocaoRepository adocaoRepository
    )
    {
        _petRepository = petRepository;
        _tutorRepository = tutorRepository;
        _adocaoRepository = adocaoRepository;
    }

    public List<AdocaoDto> ListarTodos()
    {
        return _adocaoRepository.GetAll().Select(adocao => new AdocaoDto(adocao)).ToList();
    }

    public AdocaoDto? Listar(long id)
    {
        var adocao = _adocaoRepository.GetById(id);
        return adocao != null ? new AdocaoDto(adocao) : null;
    }

    public void Solicitar(SolicitacaoDeAdocaoDto dto)
    {
        var pet = _petRepository.GetById(dto.IdPet);
        var tutor = _tutorRepository.GetById(dto.IdTutor);

        if (pet == null || tutor == null)
        {
            throw new NullReferenceException();
        }
        if (pet.Adotado)
        {
            throw new PetAdotadoException("Pet já foi adotado!", new ApplicationException());
        }
        if (_adocaoRepository.ExistsByPetIdAndStatus(pet.Id, StatusAdocao.AGUARDANDO_AVALIACAO))
        {
            throw new PetEmProcessoDeAdocaoException("Pet já está em processo de adoção!");
        }
        var numeroDeAdocoesDoTutor = _adocaoRepository.CountByTutorIdAndStatus(
            tutor.Id,
            StatusAdocao.APROVADO
        );
        if (numeroDeAdocoesDoTutor >= 2)
        {
            throw new TutorComLimiteAtingidoException(
                "Tutor já atingiu o limite de adoções aprovadas!"
            );
        }
        _adocaoRepository.Add(new Adocao(tutor, pet, dto.Motivo));
    }

    public void Aprovar(AprovarAdocaoDto dto)
    {
        var adocao = _adocaoRepository.GetById(dto.IdAdocao);
        adocao.MarcarComoAprovada();
        adocao.Pet.MarcarComoAdotado();
        _adocaoRepository.SaveChanges();
    }

    public void Reprovar(ReprovarAdocaoDto dto)
    {
        var adocao = _adocaoRepository.GetById(dto.IdAdocao);
        adocao.MarcarComoReprovada(dto.Justificativa);
        _adocaoRepository.SaveChanges();
    }
}
