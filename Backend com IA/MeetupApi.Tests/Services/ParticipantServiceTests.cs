using FluentAssertions;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;
using MeetupApi.Web.Models;
using MeetupApi.Web.Repositories;
using MeetupApi.Web.Services;
using Moq;

namespace MeetupApi.Tests.Services;

public class ParticipantServiceTests
{
    private readonly Mock<IParticipantRepository> _participantRepository;
    private readonly ParticipantService _participantService;

    public ParticipantServiceTests()
    {
        _participantRepository = new Mock<IParticipantRepository>();
        _participantService = new ParticipantService(_participantRepository.Object);
    }

    // -------------------------------------------------------------------------
    // CADASTRO — SUCESSO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveCadastrarParticipanteComSucesso()
    {
        // Arrange
        ParticipantRequest request = CriarParticipanteRequest();
        Participant participanteSalvo = CriarParticipante();

        _participantRepository.Setup(r => r.FindByEmail("ana@email.com")).Returns((Participant?)null);
        _participantRepository.Setup(r => r.Save(It.IsAny<Participant>())).Returns(participanteSalvo);

        // Act
        ParticipantResponse response = _participantService.Cadastrar(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(1L);
        response.Nome.Should().Be("Ana Silva");
        response.Email.Should().Be("ana@email.com");
        _participantRepository.Verify(r => r.Save(It.IsAny<Participant>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // CADASTRO — VALIDAÇÕES
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveLancarExcecaoQuandoEmailDuplicado()
    {
        // Arrange
        ParticipantRequest request = CriarParticipanteRequest();
        Participant participanteExistente = CriarParticipante();

        _participantRepository.Setup(r => r.FindByEmail("ana@email.com")).Returns(participanteExistente);

        // Act
        Action act = () => _participantService.Cadastrar(request);

        // Assert
        act.Should().Throw<ConflictException>();
        _participantRepository.Verify(r => r.Save(It.IsAny<Participant>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // ATUALIZAÇÃO — SUCESSO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveAtualizarParticipanteComSucesso()
    {
        // Arrange
        Participant participanteExistente = CriarParticipante();
        var requestAtualizado = new ParticipantRequest("Ana Silva Atualizada", "ana.nova@email.com");
        var participanteAtualizado = new Participant("Ana Silva Atualizada", "ana.nova@email.com");
        SetId(participanteAtualizado, 1L);

        _participantRepository.Setup(r => r.FindById(1L)).Returns(participanteExistente);
        _participantRepository.Setup(r => r.Save(It.IsAny<Participant>())).Returns(participanteAtualizado);

        // Act
        ParticipantResponse response = _participantService.Atualizar(1L, requestAtualizado);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(1L);
        response.Nome.Should().Be("Ana Silva Atualizada");
        response.Email.Should().Be("ana.nova@email.com");
        _participantRepository.Verify(r => r.Save(It.IsAny<Participant>()), Times.Once);
    }

    // -------------------------------------------------------------------------
    // ATUALIZAÇÃO — VALIDAÇÕES
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveLancarExcecaoQuandoAtualizarParticipanteInexistente()
    {
        // Arrange
        _participantRepository.Setup(r => r.FindById(99L)).Returns((Participant?)null);

        // Act
        Action act = () => _participantService.Atualizar(99L, CriarParticipanteRequest());

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
        _participantRepository.Verify(r => r.Save(It.IsAny<Participant>()), Times.Never);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoAtualizarComEmailDeOutroParticipante()
    {
        // Arrange
        Participant participante = CriarParticipante();
        var outroParticipante = new Participant("Bruno Lima", "bruno@email.com");
        SetId(outroParticipante, 2L);

        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _participantRepository.Setup(r => r.FindByEmail("bruno@email.com")).Returns(outroParticipante);

        // Act
        Action act = () => _participantService.Atualizar(1L, new ParticipantRequest("Ana Silva", "bruno@email.com"));

        // Assert
        act.Should().Throw<ConflictException>();
        _participantRepository.Verify(r => r.Save(It.IsAny<Participant>()), Times.Never);
    }

    [Fact]
    public void DeveAtualizarParticipanteMantendoOProprioEmail()
    {
        // Arrange
        Participant participante = CriarParticipante();

        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);
        _participantRepository.Setup(r => r.FindByEmail("ana@email.com")).Returns(participante);
        _participantRepository.Setup(r => r.Save(participante)).Returns(participante);

        // Act
        ParticipantResponse response = _participantService.Atualizar(1L, new ParticipantRequest("Ana Souza", "ana@email.com"));

        // Assert
        response.Nome.Should().Be("Ana Souza");
        _participantRepository.Verify(r => r.Save(participante), Times.Once);
    }

    // -------------------------------------------------------------------------
    // DELEÇÃO — SUCESSO
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveDeletarParticipanteComSucesso()
    {
        // Arrange
        Participant participante = CriarParticipante();

        _participantRepository.Setup(r => r.FindById(1L)).Returns(participante);

        // Act
        Action act = () => _participantService.Deletar(1L);

        // Assert
        act.Should().NotThrow();
        _participantRepository.Verify(r => r.Delete(participante), Times.Once);
    }

    // -------------------------------------------------------------------------
    // DELEÇÃO — VALIDAÇÕES
    // -------------------------------------------------------------------------

    [Fact]
    public void DeveLancarExcecaoQuandoDeletarParticipanteInexistente()
    {
        // Arrange
        _participantRepository.Setup(r => r.FindById(99L)).Returns((Participant?)null);

        // Act
        Action act = () => _participantService.Deletar(99L);

        // Assert
        act.Should().Throw<ResourceNotFoundException>();
        _participantRepository.Verify(r => r.Delete(It.IsAny<Participant>()), Times.Never);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Participant CriarParticipante()
    {
        var participante = new Participant("Ana Silva", "ana@email.com");
        SetId(participante, 1L);
        return participante;
    }

    private static ParticipantRequest CriarParticipanteRequest() =>
        new("Ana Silva", "ana@email.com");

    private static void SetId(Participant participante, long id) =>
        typeof(Participant).GetProperty(nameof(Participant.Id))!.SetValue(participante, id);
}
