using Adopet.Api.Dto;
using Adopet.Models;
using Adopet.Repositories;

namespace Adopet.Services;

public class TutorService
{
    private readonly TutorRepository _repository;

    public TutorService(TutorRepository repository)
    {
        _repository = repository;
    }

    public List<TutorDto> ListarTodos()
    {
        return _repository.GetAll().Select(tutor => new TutorDto(tutor)).ToList();
    }

    public void Cadastrar(CadastroTutorDto dados)
    {
        _repository.Add(new Tutor(dados));
    }
}
