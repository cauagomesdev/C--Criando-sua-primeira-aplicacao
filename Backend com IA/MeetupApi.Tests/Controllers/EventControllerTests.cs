using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupApi.Tests.Controllers;

public class EventControllerTests : IDisposable
{
    private static readonly JsonSerializerOptions Json = JsonSerializerOptions.Web;

    private readonly MeetupWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EventControllerTests()
    {
        _factory = new MeetupWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // -------------------------------------------------------------------------
    // POST /api/eventos
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveCriarEventoComSucesso()
    {
        // Arrange
        EventRequest request = CriarEventoRequest();

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/eventos", request, Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        EventResponse? body = await response.Content.ReadFromJsonAsync<EventResponse>(Json);
        body!.Id.Should().BePositive();
        body.Titulo.Should().Be(".NET Summit 2026");
        body.LimiteVagas.Should().Be(100);
    }

    [Fact]
    public async Task DeveRetornar422QuandoDataNoPassado()
    {
        // Arrange
        var request = new EventRequest(
            ".NET Summit 2026", "Conferência de .NET",
            DateTime.Now.AddDays(-1), "São Paulo", 100);

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/eventos", request, Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar422QuandoBodyInvalido()
    {
        // Arrange
        var request = new EventRequest(
            "", "Descrição", DateTime.Now.AddMonths(1), "São Paulo", 100);

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/eventos", request, Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // GET /api/eventos
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveListarTodosEventos()
    {
        // Arrange
        SalvarEvento(CriarEvento());

        // Act
        List<EventResponse>? responses =
            await _client.GetFromJsonAsync<List<EventResponse>>("/api/eventos", Json);

        // Assert
        responses.Should().HaveCount(1);
        responses![0].Titulo.Should().Be(".NET Summit 2026");
    }

    [Fact]
    public async Task DeveRetornarListaVaziaQuandoNaoHaEventos()
    {
        // Act
        List<EventResponse>? responses =
            await _client.GetFromJsonAsync<List<EventResponse>>("/api/eventos", Json);

        // Assert
        responses.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // GET /api/eventos/proximos
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveListarProximosEventos()
    {
        // Arrange
        SalvarEvento(CriarEvento());
        SalvarEvento(new Event("Passado", "desc", DateTime.Now.AddDays(-1), "SP", 10));

        // Act
        List<EventResponse>? responses =
            await _client.GetFromJsonAsync<List<EventResponse>>("/api/eventos/proximos", Json);

        // Assert
        responses.Should().HaveCount(1);
        responses![0].Titulo.Should().Be(".NET Summit 2026");
    }

    // -------------------------------------------------------------------------
    // GET /api/eventos/{id}
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveBuscarEventoPorIdComSucesso()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento());

        // Act
        HttpResponseMessage response = await _client.GetAsync($"/api/eventos/{evento.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        EventResponse? body = await response.Content.ReadFromJsonAsync<EventResponse>(Json);
        body!.Id.Should().Be(evento.Id);
        body.Titulo.Should().Be(".NET Summit 2026");
    }

    [Fact]
    public async Task DeveRetornar404QuandoEventoNaoExiste()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/eventos/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // PUT /api/eventos/{id}
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveAtualizarEventoComSucesso()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento());
        var requestAtualizado = new EventRequest(
            ".NET Summit 2027", "Conferência atualizada",
            DateTime.Now.AddMonths(2), "Rio de Janeiro", 200);

        // Act
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/api/eventos/{evento.Id}", requestAtualizado, Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        EventResponse? body = await response.Content.ReadFromJsonAsync<EventResponse>(Json);
        body!.Titulo.Should().Be(".NET Summit 2027");
        body.Local.Should().Be("Rio de Janeiro");
        body.LimiteVagas.Should().Be(200);
    }

    [Fact]
    public async Task DeveRetornar404AoAtualizarEventoInexistente()
    {
        // Act
        HttpResponseMessage response =
            await _client.PutAsJsonAsync("/api/eventos/9999", CriarEventoRequest(), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // DELETE /api/eventos/{id}
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveDeletarEventoComSucesso()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento());

        // Act
        HttpResponseMessage response = await _client.DeleteAsync($"/api/eventos/{evento.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeveRetornar404AoDeletarEventoInexistente()
    {
        // Act
        HttpResponseMessage response = await _client.DeleteAsync("/api/eventos/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private Event SalvarEvento(Event evento)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
        return repository.Save(evento);
    }

    private static Event CriarEvento() =>
        new(".NET Summit 2026", "Conferência de .NET", DateTime.Now.AddMonths(1), "São Paulo", 100);

    private static EventRequest CriarEventoRequest() =>
        new(".NET Summit 2026", "Conferência de .NET", DateTime.Now.AddMonths(1), "São Paulo", 100);
}
