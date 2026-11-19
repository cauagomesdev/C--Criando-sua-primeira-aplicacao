using Microsoft.EntityFrameworkCore;
using ScreenSound.Banco;

namespace ScreenSound.Tests.Unitarios;

/// <summary>
/// Cria um ScreenSoundContext que usa o banco em memória do EF Core,
/// assim os testes não dependem do SQL Server.
/// </summary>
internal static class ContextoEmMemoria
{
    /// <summary>
    /// Gera opções para um banco novo e isolado (nome único por teste).
    /// </summary>
    public static DbContextOptions<ScreenSoundContext> CriarOpcoes() =>
        new DbContextOptionsBuilder<ScreenSoundContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    public static ScreenSoundContext CriarContexto(DbContextOptions<ScreenSoundContext> opcoes) =>
        new(opcoes);
}
