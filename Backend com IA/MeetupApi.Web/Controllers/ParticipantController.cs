using MeetupApi.Web.Dtos;
using MeetupApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApi.Web.Controllers;

[ApiController]
[Route("api/participantes")]
public class ParticipantController : ControllerBase
{
    private readonly ParticipantService _participantService;

    public ParticipantController(ParticipantService participantService)
    {
        _participantService = participantService;
    }

    [HttpPost]
    public ActionResult<ParticipantResponse> Cadastrar([FromBody] ParticipantRequest request)
    {
        ParticipantResponse response = _participantService.Cadastrar(request);
        return Created($"/api/participantes/{response.Id}", response);
    }

    [HttpPut("{id:long}")]
    public ActionResult<ParticipantResponse> Atualizar(long id, [FromBody] ParticipantRequest request)
    {
        return Ok(_participantService.Atualizar(id, request));
    }

    [HttpDelete("{id:long}")]
    public IActionResult Deletar(long id)
    {
        _participantService.Deletar(id);
        return NoContent();
    }
}
