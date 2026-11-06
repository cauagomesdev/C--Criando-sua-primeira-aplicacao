using MeetupApi.Web.Dtos;
using MeetupApi.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApi.Web.Controllers;

[ApiController]
[Route("api/eventos")]
public class EventController : ControllerBase
{
    private readonly EventService _eventService;

    public EventController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    public ActionResult<EventResponse> CriarEvento([FromBody] EventRequest request)
    {
        EventResponse response = _eventService.CriarEvento(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
    }

    [HttpGet]
    public ActionResult<List<EventResponse>> ListarEventos()
    {
        return Ok(_eventService.ListarEventos());
    }

    [HttpGet("{id:long}")]
    public ActionResult<EventResponse> BuscarPorId(long id)
    {
        return Ok(_eventService.BuscarPorId(id));
    }

    [HttpGet("busca")]
    public ActionResult<List<EventResponse>> BuscarEventosPorLocal([FromQuery] string local)
    {
        return Ok(_eventService.BuscarEventosPorLocal(local));
    }

    [HttpGet("proximos")]
    public ActionResult<List<EventResponse>> ListarProximosEventos()
    {
        return Ok(_eventService.ListarProximosEventos());
    }

    [HttpPut("{id:long}")]
    public ActionResult<EventResponse> AtualizarEvento(long id, [FromBody] EventRequest request)
    {
        return Ok(_eventService.AtualizarEvento(id, request));
    }

    [HttpDelete("{id:long}")]
    public IActionResult DeletarEvento(long id)
    {
        _eventService.DeletarEvento(id);
        return NoContent();
    }
}
