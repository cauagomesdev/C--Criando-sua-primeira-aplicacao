using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupApi.Tests.Controllers;

public class ParticipantControllerTests : IDisposable
{
    private static readonly JsonSerializerOptions Json = JsonSerializerOptions.Web;

    private readonly MeetupWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ParticipantControllerTests()
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
    // POST /api/participantes
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveCadastrarParticipanteComSucesso()
    {
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/participantes", new ParticipantRequest("Ana Silva", "ana@email.com"), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        ParticipantResponse? body = await response.Content.ReadFromJsonAsync<ParticipantResponse>(Json);
        body!.Id.Should().BePositive();
        body.Nome.Should().Be("Ana Silva");
        body.Email.Should().Be("ana@email.com");
    }

    [Fact]
    public async Task DeveRetornar409QuandoEmailDuplicado()
    {
        // Arrange
        SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/participantes", new ParticipantRequest("Ana Silva", "ana@email.com"), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeveRetornar422QuandoBodyInvalido()
    {
        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/participantes", new ParticipantRequest("Ana Silva", "email-invalido"), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // PUT /api/participantes/{id}
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveAtualizarParticipanteComSucesso()
    {
        // Arrange
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"/api/participantes/{participante.Id}",
            new ParticipantRequest("Ana Silva Atualizada", "ana.nova@email.com"), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ParticipantResponse? body = await response.Content.ReadFromJsonAsync<ParticipantResponse>(Json);
        body!.Id.Should().Be(participante.Id);
        body.Nome.Should().Be("Ana Silva Atualizada");
        body.Email.Should().Be("ana.nova@email.com");
    }

    [Fact]
    public async Task DeveRetornar404AoAtualizarParticipanteInexistente()
    {
        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync(
            "/api/participantes/9999", new ParticipantRequest("Ana Silva", "ana@email.com"), Json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // DELETE /api/participantes/{id}
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DeveDeletarParticipanteComSucesso()
    {
        // Arrange
        Participant participante = SalvarParticipante(new Participant("Ana Silva", "ana@email.com"));

        // Act
        HttpResponseMessage response = await _client.DeleteAsync($"/api/participantes/{participante.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeveRetornar404AoDeletarParticipanteInexistente()
    {
        // Act
        HttpResponseMessage response = await _client.DeleteAsync("/api/participantes/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ErrorResponse? body = await response.Content.ReadFromJsonAsync<ErrorResponse>(Json);
        body!.Mensagem.Should().NotBeNullOrEmpty();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private Participant SalvarParticipante(Participant participante)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
        return repository.Save(participante);
    }
}
