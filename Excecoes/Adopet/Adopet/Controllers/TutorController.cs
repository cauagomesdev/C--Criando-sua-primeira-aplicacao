using Adopet.Api.Dto;
using Adopet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Adopet.Controllers;

[ApiController]
[Route("[controller]")]
public class TutorController : ControllerBase
{
    private readonly TutorService _service;

    public TutorController(TutorService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult BuscarTodos()
    {
        List<TutorDto> tutores = _service.ListarTodos();
        return Ok(tutores);
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastroTutorDto dados)
    {
        _service.Cadastrar(dados);
        return Ok();
    }
}
