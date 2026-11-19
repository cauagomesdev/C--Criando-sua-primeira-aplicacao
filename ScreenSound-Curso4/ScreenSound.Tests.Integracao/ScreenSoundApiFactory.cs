using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScreenSound.Banco;
using ScreenSound.Shared.Dados.Modelos;

namespace ScreenSound.Tests.Integracao;

/// <summary>
/// Sobe a ScreenSound.API inteira em memória (sem precisar rodar a task),
/// trocando apenas o que depende do mundo externo:
///  - SQL Server  -> banco InMemory do EF Core
///  - Login real  -> autenticação de teste (header "X-Test-User")
///  - wwwroot     -> pasta temporária (o POST de artista grava a foto no disco)
/// </summary>
public class ScreenSoundApiFactory : WebApplicationFactory<Program>
{
    public const string EmailUsuarioTeste = "teste@screensound.com";

    private readonly string nomeBanco = Guid.NewGuid().ToString();

    public string PastaConteudo { get; } =
        Path.Combine(Path.GetTempPath(), "ScreenSoundTests", Guid.NewGuid().ToString());

    public string PastaFotosPerfil => Path.Combine(PastaConteudo, "wwwroot", "FotosPerfil");

    public ScreenSoundApiFactory()
    {
        Directory.CreateDirectory(PastaFotosPerfil);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(PastaConteudo);

        builder.ConfigureTestServices(services =>
        {
            // Remove a configuração do SQL Server feita no Program.cs
            var registrosDoBanco = services
                .Where(s =>
                    s.ServiceType == typeof(DbContextOptions<ScreenSoundContext>)
                    || s.ServiceType == typeof(DbContextOptions)
                    || s.ServiceType == typeof(IDbContextOptionsConfiguration<ScreenSoundContext>))
                .ToList();
            foreach (var registro in registrosDoBanco)
                services.Remove(registro);

            services.AddDbContext<ScreenSoundContext>(options => options
                .UseInMemoryDatabase(nomeBanco)
                .UseLazyLoadingProxies());

            // Autenticação de teste vira o esquema padrão
            services
                .AddAuthentication(TestAuthHandler.Esquema)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Esquema, _ => { });
            services.PostConfigure<AuthenticationOptions>(o =>
            {
                o.DefaultScheme = TestAuthHandler.Esquema;
                o.DefaultAuthenticateScheme = TestAuthHandler.Esquema;
                o.DefaultChallengeScheme = TestAuthHandler.Esquema;
                o.DefaultForbidScheme = TestAuthHandler.Esquema;
            });
        });
    }

    /// <summary>Cliente que já vai "logado" como o usuário de teste.</summary>
    public HttpClient CriarClienteAutenticado()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.Header, EmailUsuarioTeste);
        return client;
    }

    /// <summary>
    /// Limpa o banco e cadastra o usuário de teste (usado nos endpoints de avaliação).
    /// Retorna o Id desse usuário.
    /// </summary>
    public int ResetarBanco()
    {
        using var scope = Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<ScreenSoundContext>();
        contexto.Database.EnsureDeleted();
        contexto.Database.EnsureCreated();

        var pessoa = new PessoaComAcesso
        {
            UserName = EmailUsuarioTeste,
            Email = EmailUsuarioTeste,
        };
        contexto.Users.Add(pessoa);
        contexto.SaveChanges();
        return pessoa.Id;
    }

    /// <summary>Executa algo direto no banco (para preparar dados ou conferir resultados).</summary>
    public T UsarContexto<T>(Func<ScreenSoundContext, T> acao)
    {
        using var scope = Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<ScreenSoundContext>();
        return acao(contexto);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(PastaConteudo))
            Directory.Delete(PastaConteudo, recursive: true);
    }
}

/// <summary>
/// Autentica a requisição se ela tiver o header "X-Test-User: email".
/// Sem o header, a requisição fica anônima e a API responde 401.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Esquema = "Test";
    public const string Header = "X-Test-User";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Header, out var email) || string.IsNullOrEmpty(email))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email!),
            new Claim(ClaimTypes.Email, email!),
        };
        var identidade = new ClaimsIdentity(claims, Esquema);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identidade), Esquema);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
