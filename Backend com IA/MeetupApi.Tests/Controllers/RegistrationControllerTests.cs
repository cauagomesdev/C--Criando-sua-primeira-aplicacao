using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupApi.Tests.Controllers;

public class RegistrationControllerTests : IDisposable
{
    private static readonly JsonSerializerOptions Json = JsonSerializerOptions.Web;

    private readonly MeetupWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public RegistrationControllerTests()
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
    // POST /api/eventos/{eventoId}/inscricoes
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveCriarInscricaoComSucesso()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/eventos/{evento.Id}/inscricoes", new RegistrationRequest(participante.Id), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        RegistrationResponse? body = await response.Content.ReadFromJsonAsync<RegistrationResponse>(Json);
        body!.Status.Should().Be("ATIVA");
        body.EmailParticipante.Should().Be("ana@email.com");
    }

    [Fact]
    public async Task DeveRetornar404QuandoEventoNaoExiste()
    {
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/eventos/9999/inscricoes", new RegistrationRequest(1L), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar404QuandoParticipanteNaoExiste()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/eventos/{evento.Id}/inscricoes", new RegistrationRequest(9999L), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar409QuandoEventoLotado()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(1, DateTime.Now.AddDays(2)));
        Participant participanteA = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        SalvarInscricao(new Registration(evento, participanteA, DateTime.Now, RegistrationStatus.ATIVA));
        Participant participanteB = SalvarParticipante(new Participant("Bob Santos", "bob@email.com"));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/eventos/{evento.Id}/inscricoes", new RegistrationRequest(participanteB.Id), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar409QuandoInscricaoDuplicada()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        SalvarInscricao(new Registration(evento, participante, DateTime.Now, RegistrationStatus.ATIVA));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/eventos/{evento.Id}/inscricoes", new RegistrationRequest(participante.Id), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar422QuandoEventoNoPassado()
    {
        // Arrange — evento salvo direto pelo repositório para contornar a validação do request
        Event eventoPassado = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(-1)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/eventos/{eventoPassado.Id}/inscricoes", new RegistrationRequest(participante.Id), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar422QuandoBodyInvalido()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        var content = new StringContent("{\"participanteId\": null}", Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response =
            await _client.PostAsync($"/api/eventos/{evento.Id}/inscricoes", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // GET /api/eventos/{eventoId}/inscricoes
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveListarInscricoesDoEvento()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        SalvarInscricao(new Registration(evento, participante, DateTime.Now, RegistrationStatus.ATIVA));

        // Act
        List<RegistrationResponse>? responses = await _client.GetFromJsonAsync<List<RegistrationResponse>>(
            $"/api/eventos/{evento.Id}/inscricoes", Json);

        // Assert
        responses.Should().HaveCount(1);
        responses![0].EmailParticipante.Should().Be("ana@email.com");
        responses[0].Status.Should().Be("ATIVA");
    }

    // -------------------------------------------------------------------------
    // PATCH /api/eventos/{eventoId}/inscricoes/{inscricaoId}/cancelar
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveCancelarInscricaoComSucesso()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        Registration inscricao = SalvarInscricao(
            new Registration(evento, participante, DateTime.Now, RegistrationStatus.ATIVA));

        // Act
        HttpResponseMessage response = await _client.PatchAsync(
            $"/api/eventos/{evento.Id}/inscricoes/{inscricao.Id}/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        RegistrationResponse? body = await response.Content.ReadFromJsonAsync<RegistrationResponse>(Json);
        body!.Status.Should().Be("CANCELADA");
    }

    [Fact]
    public async Task DeveRetornar422QuandoCancelamentoTardio()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddHours(12)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        Registration inscricao = SalvarInscricao(
            new Registration(evento, participante, DateTime.Now, RegistrationStatus.ATIVA));

        // Act
        HttpResponseMessage response = await _client.PatchAsync(
            $"/api/eventos/{evento.Id}/inscricoes/{inscricao.Id}/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().Be("Cancelamento permitido somente até 24h antes do evento");
    }

    [Fact]
    public async Task DeveRetornar404QuandoInscricaoNaoExiste()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));

        // Act
        HttpResponseMessage response = await _client.PatchAsync(
            $"/api/eventos/{evento.Id}/inscricoes/9999/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar422QuandoInscricaoJaCancelada()
    {
        // Arrange
        Event evento = SalvarEvento(CriarEvento(100, DateTime.Now.AddDays(2)));
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));
        Registration inscricao = SalvarInscricao(
            new Registration(evento, participante, DateTime.Now, RegistrationStatus.CANCELADA));

        // Act
        HttpResponseMessage response = await _client.PatchAsync(
            $"/api/eventos/{evento.Id}/inscricoes/{inscricao.Id}/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
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

    private Participant SalvarParticipante(Participant participante)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
        return repository.Save(participante);
    }

    private Registration SalvarInscricao(Registration inscricao)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MeetupApi.Web.Data.MeetupDbContext>();
        // Attach marca o evento/participante já persistidos como Unchanged e a inscrição (Id = 0) como Added
        context.Attach(inscricao);
        context.SaveChanges();
        return inscricao;
    }

    private static Event CriarEvento(int capacidade, DateTime dataHora) =>
        new(".NET Summit 2026", "Conferência de .NET", dataHora, "São Paulo", capacidade);
}
