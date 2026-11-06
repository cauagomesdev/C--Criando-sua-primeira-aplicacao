using MeetupApi.Web.Dtos;
using MeetupApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApi.Web.Controllers;

[ApiController]
[Route("api/eventos/{eventoId:long}/inscricoes")]
public class RegistrationController : ControllerBase
{
    private readonly RegistrationService _registrationService;

    public RegistrationController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost]
    public ActionResult<RegistrationResponse> Inscrever(long eventoId, [FromBody] RegistrationRequest request)
    {
        RegistrationResponse response = _registrationService.Inscrever(eventoId, request);
        return Created($"/api/eventos/{eventoId}/inscricoes/{response.Id}", response);
    }

    [HttpGet]
    public ActionResult<List<RegistrationResponse>> ListarInscricoes(long eventoId)
    {
        return Ok(_registrationService.ListarInscricoes(eventoId));
    }

    [HttpPatch("{inscricaoId:long}/cancelar")]
    public ActionResult<RegistrationResponse> CancelarInscricao(long eventoId, long inscricaoId)
    {
        return Ok(_registrationService.CancelarInscricao(eventoId, inscricaoId));
    }
}
