using MeetupApi.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupApi.Tests.Controllers;

/// <summary>
/// WebApplicationFactory que troca o provider MySQL por SQLite in-memory.
/// A conexão fica aberta durante o teste para manter o banco em memória vivo,
/// e cada instância da factory tem um banco isolado.
/// </summary>
public class MeetupWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public MeetupWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove o registro do DbContext com MySQL (provider único no ambiente de teste)
            services.RemoveAll(typeof(DbContextOptions<MeetupDbContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<MeetupDbContext>));
            services.RemoveAll(typeof(MeetupDbContext));

            services.AddDbContext<MeetupDbContext>(options =>
                options.UseSqlite(_connection)
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}

internal static class ServiceCollectionExtensions
{
    public static void RemoveAll(this IServiceCollection services, Type serviceType)
    {
        for (int i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType == serviceType)
            {
                services.RemoveAt(i);
            }
        }
    }
}
