using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.Tests.Unitarios;

public class DALGeneroTest
{
    [Fact]
    public void Adicionar_DeveSalvarGeneroNoBanco()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Genero>(contexto);
            // Act
            dal.Adicionar(new Genero { Nome = "MPB", Descricao = "Música Popular Brasileira" });
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        var generos = contextoVerificacao.Generos.ToList();
        generos.Should().ContainSingle();
        generos[0].Nome.Should().Be("MPB");
        generos[0].Descricao.Should().Be("Música Popular Brasileira");
    }

    [Fact]
    public void Adicionar_ComMusicas_DeveSalvarRelacionamentoMuitosParaMuitos()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var genero = new Genero
            {
                Nome = "MPB",
                Musicas = [new Musica("Oceano"), new Musica("Flor de Lis")],
            };

            // Act
            new DAL<Genero>(contexto).Adicionar(genero);
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        var generoSalvo = contextoVerificacao.Generos.Include(g => g.Musicas).Single();
        generoSalvo.Musicas.Select(m => m.Nome).Should().BeEquivalentTo(["Oceano", "Flor de Lis"]);
    }

    [Fact]
    public void Listar_DeveRetornarTodosOsGeneros()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Genero>(contexto);
        dal.Adicionar(new Genero { Nome = "MPB" });
        dal.Adicionar(new Genero { Nome = "Rock" });

        // Act
        var generos = dal.Listar();

        // Assert
        generos.Select(g => g.Nome).Should().BeEquivalentTo(["MPB", "Rock"]);
    }

    [Fact]
    public void Atualizar_DeveAlterarDadosDoGenero()
    {
        // Arrange
        var opcoes = ContextoEmMemoria.CriarOpcoes();
        int id;
        using (var contexto = ContextoEmMemoria.CriarContexto(opcoes))
        {
            var dal = new DAL<Genero>(contexto);
            var genero = new Genero { Nome = "MPB", Descricao = "Antiga" };
            dal.Adicionar(genero);
            id = genero.Id;

            // Act
            genero.Descricao = "Nova";
            dal.Atualizar(genero);
        }

        // Assert
        using var contextoVerificacao = ContextoEmMemoria.CriarContexto(opcoes);
        contextoVerificacao.Generos.Find(id)!.Descricao.Should().Be("Nova");
    }

    [Fact]
    public void Deletar_DeveRemoverGeneroDoBanco()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Genero>(contexto);
        var genero = new Genero { Nome = "MPB" };
        dal.Adicionar(genero);
        dal.Adicionar(new Genero { Nome = "Rock" });

        // Act
        dal.Deletar(genero);

        // Assert
        dal.Listar().Should().ContainSingle(g => g.Nome == "Rock");
    }

    [Fact]
    public void RecuperarPor_ComCondicaoExistente_DeveRetornarGenero()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Genero>(contexto);
        dal.Adicionar(new Genero { Nome = "MPB" });
        dal.Adicionar(new Genero { Nome = "Rock" });

        // Act
        var genero = dal.RecuperarPor(g => g.Nome == "Rock");

        // Assert
        genero.Should().NotBeNull();
        genero!.Nome.Should().Be("Rock");
    }

    [Fact]
    public void RecuperarPor_ComCondicaoInexistente_DeveRetornarNull()
    {
        // Arrange
        using var contexto = ContextoEmMemoria.CriarContexto(ContextoEmMemoria.CriarOpcoes());
        var dal = new DAL<Genero>(contexto);
        dal.Adicionar(new Genero { Nome = "MPB" });

        // Act
        var genero = dal.RecuperarPor(g => g.Nome == "Jazz");

        // Assert
        genero.Should().BeNull();
    }
}
