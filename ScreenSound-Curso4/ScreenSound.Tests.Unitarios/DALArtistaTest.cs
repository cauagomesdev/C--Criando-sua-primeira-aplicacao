using FluentAssertions;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.Tests.Unitarios;

public class DALArtistaTest
{
    [Fact]
    public void Adicionar_DeveSalvarArtistaNoBanco()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Artista>(contexto);
            // Act
            dal.Adicionar(new Artista("Djavan", "Cantor e compositor alagoano"));
        }

        // Assert (contexto novo para garantir que foi salvo de verdade)
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        var artistas = contextoVerificacao.Artistas.ToList();
        artistas.Should().ContainSingle();
        artistas[0].Nome.Should().Be("Djavan");
        artistas[0].Bio.Should().Be("Cantor e compositor alagoano");
        artistas[0].Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Listar_DeveRetornarTodosOsArtistas()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Artista>(contexto);
        dal.Adicionar(new Artista("Djavan", "Bio 1"));
        dal.Adicionar(new Artista("Gal Costa", "Bio 2"));

        // Act
        var artistas = dal.Listar();

        // Assert
        artistas.Select(a => a.Nome).Should().BeEquivalentTo(["Djavan", "Gal Costa"]);
    }

    [Fact]
    public void Listar_ComBancoVazio_DeveRetornarListaVazia()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Artista>(contexto);

        // Act
        var artistas = dal.Listar();

        // Assert
        artistas.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Atualizar_DeveAlterarDadosDoArtista()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        int id;
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Artista>(contexto);
            var artista = new Artista("Djavan", "Bio antiga");
            dal.Adicionar(artista);
            id = artista.Id;

            // Act
            artista.Bio = "Bio nova";
            dal.Atualizar(artista);
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        contextoVerificacao.Artistas.Find(id)!.Bio.Should().Be("Bio nova");
    }

    [Fact]
    public void Deletar_DeveRemoverArtistaDoBanco()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Artista>(contexto);
        var artista = new Artista("Djavan", "Bio");
        dal.Adicionar(artista);
        dal.Adicionar(new Artista("Gal Costa", "Bio"));

        // Act
        dal.Deletar(artista);

        // Assert
        dal.Listar().Should().ContainSingle(a => a.Nome == "Gal Costa");
    }

    [Fact]
    public void RecuperarPor_ComCondicaoExistente_DeveRetornarArtista()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Artista>(contexto);
        dal.Adicionar(new Artista("Djavan", "Bio"));
        dal.Adicionar(new Artista("Gal Costa", "Bio"));

        // Act
        var artista = dal.RecuperarPor(a => a.Nome == "Gal Costa");

        // Assert
        artista.Should().NotBeNull();
        artista!.Nome.Should().Be("Gal Costa");
    }

    [Fact]
    public void RecuperarPor_ComCondicaoInexistente_DeveRetornarNull()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Artista>(contexto);
        dal.Adicionar(new Artista("Djavan", "Bio"));

        // Act
        var artista = dal.RecuperarPor(a => a.Nome == "Não existe");

        // Assert
        artista.Should().BeNull();
    }
}
