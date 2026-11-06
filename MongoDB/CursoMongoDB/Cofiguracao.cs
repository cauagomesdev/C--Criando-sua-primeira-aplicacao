namespace CursoMongoDB;

public static class Configuracao
{
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("MONGODB_URI")
        ?? throw new InvalidOperationException(
            "Defina a variável de ambiente MONGODB_URI com a connection string do MongoDB."
        );
}
