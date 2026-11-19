using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Modelos;

namespace ScreenSound.Tests.Integracao;

/// <summary>
/// Testa os endpoints /artistas fazendo requisições HTTP de verdade
/// contra a API rodando em memória.
/// </summary>
public class EndPointArtistasTest : IClassFixture<ScreenSoundApiFactory>
{
    private readonly ScreenSoundApiFactory factory;
    private readonly HttpClient client;
    private readonly int pessoaId;

    public EndPointArtistasTest(ScreenSoundApiFactory factory)
    {
        // O xUnit cria uma instância desta classe por teste,
        // então cada teste começa com o banco limpo.
        this.factory = factory;
        pessoaId = factory.ResetarBanco();
        client = factory.CriarClienteAutenticado();
    }

    private Artista CadastrarArtista(string nome, string bio = "Bio")
    {
        return factory.UsarContexto(contexto =>
        {
            var artista = new Artista(nome, bio);
            contexto.Artistas.Add(artista);
            contexto.SaveChanges();
            return artista;
        });
    }

    private void CadastrarAvaliacao(int artistaId, int pessoa, int nota)
    {
        factory.UsarContexto(contexto =>
        {
            contexto.AvaliacaoArtistas.Add(new AvaliacaoArtista { ArtistaId = artistaId, PessoaId = pessoa, Nota = nota });
            return contexto.SaveChanges();
        });
    }

    // ---------- Autorização ----------

    [Theory]
    [InlineData("GET", "/artistas")]
    [InlineData("GET", "/artistas/Djavan")]
    [InlineData("DELETE", "/artistas/1")]
    [InlineData("GET", "/artistas/1/avaliacao")]
    public async Task Endpoints_SemAutenticacao_DevemRetornar401(string metodo, string url)
    {
        // Arrange
        var clienteAnonimo = factory.CreateClient();

        // Act
        var response = await clienteAnonimo.SendAsync(new HttpRequestMessage(new HttpMethod(metodo), url));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ---------- GET /artistas ----------

    [Fact]
    public async Task GetArtistas_Deve_Retornar_Ok_Mesmo_Sem_Dados()
    {
        // Act
        var response = await client.GetAsync("/artistas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var artistas = await response.Content.ReadFromJsonAsync<List<ArtistaResponse>>();
        artistas.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task GetArtistas_ComDados_DeveRetornarTodosOsArtistas()
    {
        // Arrange
        CadastrarArtista("Djavan");
        CadastrarArtista("Gal Costa");

        // Act
        var artistas = await client.GetFromJsonAsync<List<ArtistaResponse>>("/artistas");

        // Assert
        artistas!.Select(a => a.Nome).Should().BeEquivalentTo(["Djavan", "Gal Costa"]);
    }

    [Fact]
    public async Task GetArtistas_DeveCalcularClassificacaoPelaMediaDasNotas()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");
        CadastrarAvaliacao(artista.Id, pessoa: 1, nota: 4);
        CadastrarAvaliacao(artista.Id, pessoa: 2, nota: 5);

        // Act
        var artistas = await client.GetFromJsonAsync<List<ArtistaResponse>>("/artistas");

        // Assert
        artistas!.Single().Classificacao.Should().Be(4.5);
    }

    // ---------- GET /artistas/{nome} ----------

    [Fact]
    public async Task GetArtistaPorNome_Existente_DeveRetornarArtista()
    {
        // Arrange
        CadastrarArtista("Djavan", "Cantor alagoano");

        // Act
        var response = await client.GetAsync("/artistas/Djavan");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var artista = await response.Content.ReadFromJsonAsync<ArtistaResponse>();
        artista!.Nome.Should().Be("Djavan");
        artista.Bio.Should().Be("Cantor alagoano");
    }

    [Fact]
    public async Task GetArtistaPorNome_DeveIgnorarMaiusculasEMinusculas()
    {
        // Arrange
        CadastrarArtista("Djavan");

        // Act
        var response = await client.GetAsync("/artistas/dJaVaN");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetArtistaPorNome_Inexistente_DeveRetornar404()
    {
        // Act
        var response = await client.GetAsync("/artistas/NaoExiste");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- POST /artistas ----------

    [Fact]
    public async Task PostArtista_DeveSalvarArtistaEFoto()
    {
        // Arrange
        var fotoBase64 = Convert.ToBase64String([1, 2, 3, 4]);
        var request = new ArtistaRequest("Djavan", "Cantor alagoano", fotoBase64);

        // Act
        var response = await client.PostAsJsonAsync("/artistas", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var salvo = factory.UsarContexto(c => c.Artistas.Single());
        salvo.Nome.Should().Be("Djavan");
        salvo.Bio.Should().Be("Cantor alagoano");
        salvo.FotoPerfil.Should().StartWith("/FotosPerfil/").And.EndWith(".Djavan.jpg");

        var arquivo = Path.Combine(factory.PastaFotosPerfil, Path.GetFileName(salvo.FotoPerfil));
        File.Exists(arquivo).Should().BeTrue();
        File.ReadAllBytes(arquivo).Should().Equal(1, 2, 3, 4);
    }

    // ---------- PUT /artistas ----------

    [Fact]
    public async Task PutArtista_Existente_DeveAtualizarNomeEBio()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan", "Bio antiga");
        var request = new ArtistaRequestEdit(artista.Id, "Djavan Caetano", "Bio nova", null);

        // Act
        var response = await client.PutAsJsonAsync("/artistas", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var atualizado = factory.UsarContexto(c => c.Artistas.Single(a => a.Id == artista.Id));
        atualizado.Nome.Should().Be("Djavan Caetano");
        atualizado.Bio.Should().Be("Bio nova");
    }

    [Fact]
    public async Task PutArtista_Inexistente_DeveRetornar404()
    {
        // Arrange
        var request = new ArtistaRequestEdit(999, "Nome", "Bio", null);

        // Act
        var response = await client.PutAsJsonAsync("/artistas", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- DELETE /artistas/{id} ----------

    [Fact]
    public async Task DeleteArtista_Existente_DeveRemoverERetornar204()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");
        CadastrarArtista("Gal Costa");

        // Act
        var response = await client.DeleteAsync($"/artistas/{artista.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        factory.UsarContexto(c => c.Artistas.Select(a => a.Nome).ToList())
            .Should().Equal("Gal Costa");
    }

    [Fact]
    public async Task DeleteArtista_Inexistente_DeveRetornar404()
    {
        // Act
        var response = await client.DeleteAsync("/artistas/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- POST /artistas/avaliacao ----------

    [Fact]
    public async Task PostAvaliacao_PrimeiraAvaliacao_DeveSalvarNota()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");

        // Act
        var response = await client.PostAsJsonAsync("/artistas/avaliacao", new AvaliacaoArtistaRequest(artista.Id, 4));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var avaliacao = factory.UsarContexto(c => c.AvaliacaoArtistas.AsNoTracking().Single());
        avaliacao.ArtistaId.Should().Be(artista.Id);
        avaliacao.PessoaId.Should().Be(pessoaId);
        avaliacao.Nota.Should().Be(4);
    }

    [Fact]
    public async Task PostAvaliacao_JaAvaliado_DeveAtualizarNota()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");
        CadastrarAvaliacao(artista.Id, pessoaId, nota: 2);

        // Act
        var response = await client.PostAsJsonAsync("/artistas/avaliacao", new AvaliacaoArtistaRequest(artista.Id, 5));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var avaliacoes = factory.UsarContexto(c => c.AvaliacaoArtistas.AsNoTracking().ToList());
        avaliacoes.Should().ContainSingle().Which.Nota.Should().Be(5);
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    public async Task PostAvaliacao_NotaForaDoIntervalo_DeveLimitarEntre1E5(int notaEnviada, int notaSalva)
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");

        // Act
        await client.PostAsJsonAsync("/artistas/avaliacao", new AvaliacaoArtistaRequest(artista.Id, notaEnviada));

        // Assert
        factory.UsarContexto(c => c.AvaliacaoArtistas.AsNoTracking().Single()).Nota.Should().Be(notaSalva);
    }

    [Fact]
    public async Task PostAvaliacao_ArtistaInexistente_DeveRetornar404()
    {
        // Act
        var response = await client.PostAsJsonAsync("/artistas/avaliacao", new AvaliacaoArtistaRequest(999, 5));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---------- GET /artistas/{id}/avaliacao ----------

    [Fact]
    public async Task GetAvaliacao_ComNotaDaPessoaLogada_DeveRetornarNota()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");
        CadastrarAvaliacao(artista.Id, pessoaId, nota: 3);

        // Act
        var avaliacao = await client.GetFromJsonAsync<AvaliacaoArtistaResponse>($"/artistas/{artista.Id}/avaliacao");

        // Assert
        avaliacao.Should().Be(new AvaliacaoArtistaResponse(artista.Id, 3));
    }

    [Fact]
    public async Task GetAvaliacao_SemNotaDaPessoaLogada_DeveRetornarZero()
    {
        // Arrange
        var artista = CadastrarArtista("Djavan");
        CadastrarAvaliacao(artista.Id, pessoa: pessoaId + 100, nota: 5); // nota de outra pessoa

        // Act
        var avaliacao = await client.GetFromJsonAsync<AvaliacaoArtistaResponse>($"/artistas/{artista.Id}/avaliacao");

        // Assert
        avaliacao.Should().Be(new AvaliacaoArtistaResponse(artista.Id, 0));
    }

    [Fact]
    public async Task GetAvaliacao_ArtistaInexistente_DeveRetornar404()
    {
        // Act
        var response = await client.GetAsync("/artistas/999/avaliacao");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
