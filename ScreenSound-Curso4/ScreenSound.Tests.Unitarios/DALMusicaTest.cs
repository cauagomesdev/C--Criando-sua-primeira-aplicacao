using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.Tests.Unitarios;

public class DALMusicaTest
{
    [Fact]
    public void Adicionar_DeveSalvarMusicaNoBanco()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Musica>(contexto);
            // Act
            dal.Adicionar(new Musica("Oceano") { AnoLancamento = 1989 });
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        var musicas = contextoVerificacao.Musicas.ToList();
        musicas.Should().ContainSingle();
        musicas[0].Nome.Should().Be("Oceano");
        musicas[0].AnoLancamento.Should().Be(1989);
    }

    [Fact]
    public void Adicionar_ComArtista_DeveSalvarRelacionamento()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        int artistaId;
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var artista = new Artista("Djavan", "Bio");
            new DAL<Artista>(contexto).Adicionar(artista);
            artistaId = artista.Id;

            // Act
            new DAL<Musica>(contexto).Adicionar(new Musica("Oceano") { Artista = artista });
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        var musica = contextoVerificacao.Musicas.Include(m => m.Artista).Single();
        musica.ArtistaId.Should().Be(artistaId);
        musica.Artista!.Nome.Should().Be("Djavan");
    }

    [Fact]
    public void Listar_DeveRetornarTodasAsMusicas()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Musica>(contexto);
        dal.Adicionar(new Musica("Oceano"));
        dal.Adicionar(new Musica("Flor de Lis"));

        // Act
        var musicas = dal.Listar();

        // Assert
        musicas.Select(m => m.Nome).Should().BeEquivalentTo(["Oceano", "Flor de Lis"]);
    }

    [Fact]
    public void Atualizar_DeveAlterarDadosDaMusica()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        int id;
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Musica>(contexto);
            var musica = new Musica("Oceano") { AnoLancamento = 1900 };
            dal.Adicionar(musica);
            id = musica.Id;

            // Act
            musica.AnoLancamento = 1989;
            dal.Atualizar(musica);
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        contextoVerificacao.Musicas.Find(id)!.AnoLancamento.Should().Be(1989);
    }

    [Fact]
    public void Deletar_DeveRemoverMusicaDoBanco()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Musica>(contexto);
        var musica = new Musica("Oceano");
        dal.Adicionar(musica);

        // Act
        dal.Deletar(musica);

        // Assert
        dal.Listar().Should().BeEmpty();
    }

    [Fact]
    public void RecuperarPor_ComCondicaoExistente_DeveRetornarMusica()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Musica>(contexto);
        var flor = new Musica("Flor de Lis");
        dal.Adicionar(new Musica("Oceano"));
        dal.Adicionar(flor);

        // Act
        var musica = dal.RecuperarPor(m => m.Id == flor.Id);

        // Assert
        musica.Should().NotBeNull();
        musica!.Nome.Should().Be("Flor de Lis");
    }

    [Fact]
    public void RecuperarPor_ComCondicaoInexistente_DeveRetornarNull()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Musica>(contexto);
        dal.Adicionar(new Musica("Oceano"));

        // Act
        var musica = dal.RecuperarPor(m => m.Id == 999);

        // Assert
        musica.Should().BeNull();
    }
}
