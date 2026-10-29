using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenSound.API.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed de Gêneros
            migrationBuilder.InsertData(
                table: "Generos",
                columns: ["Nome", "Descricao"],
                values: new object[,]
                {
                    { "Rock", "Gênero musical que se desenvolveu a partir do rock and roll nas décadas de 1950 e 1960." },
                    { "Pop", "Gênero de música popular que se originou em sua forma moderna em meados da década de 1950 nos Estados Unidos e Reino Unido." },
                    { "MPB", "A Música Popular Brasileira é um gênero musical que surgiu no Brasil em meados dos anos 1960." }
                });

            // Seed de Artistas
            migrationBuilder.InsertData(
                table: "Artistas",
                columns: ["Nome", "Bio", "FotoPerfil"],
                values: new object[,]
                {
                    { "Foo Fighters", "Foo Fighters é uma banda de rock alternativo americana formada por Dave Grohl em 1995.", "Foto_001.png" },
                    { "Queen", "Queen foi uma banda de rock britânica formada em Londres em 1970.", "Foto_002.png" },
                    { "Led Zeppelin", "Led Zeppelin foi uma banda de rock britânica formada em Londres em 1968.", "Foto_003.png" },
                    { "Taylor Swift", "Taylor Alison Swift é uma cantora e compositora americana.", "Foto_004.png" },
                    { "Michael Jackson", "Michael Joseph Jackson foi um cantor, compositor e dançarino americano.", "Foto_005.png" },
                    { "Madonna", "Madonna Louise Ciccone é uma cantora, compositora, atriz e empresária americana.", "Foto_006.png" },
                    { "Djavan", "Djavan Caetano Viana é um cantor, compositor, arranjador, produtor musical, empresário, violonista e ex-futebolista brasileiro.", "Foto_007.png" },
                    { "Gilberto Gil", "Gilberto Passos Gil Moreira é um cantor, compositor, multi-instrumentista, produtor musical, político e escritor brasileiro.", "Foto_008.png" },
                    { "Caetano Veloso", "Caetano Emanuel Viana Telles Veloso é um cantor, compositor, violonista, escritor e produtor musical brasileiro.", "Foto_009.png" }
                });

            // Insere todas as músicas com ArtistaId e GeneroId via UPDATE
            migrationBuilder.InsertData(
                table: "Musicas",
                columns: ["Nome", "AnoLancamento"],
                values: new object[,]
                {
                    { "Everlong", 1997 },
                    { "The Pretender", 2007 },
                    { "Learn to Fly", 1999 },
                    { "Bohemian Rhapsody", 1975 },
                    { "Don't Stop Me Now", 1978 },
                    { "We Will Rock You", 1977 },
                    { "Stairway to Heaven", 1971 },
                    { "Whole Lotta Love", 1969 },
                    { "Black Dog", 1971 },
                    { "Blank Space", 2014 },
                    { "Shake It Off", 2014 },
                    { "Love Story", 2008 },
                    { "Billie Jean", 1982 },
                    { "Thriller", 1982 },
                    { "Beat It", 1982 },
                    { "Like a Virgin", 1984 },
                    { "Material Girl", 1984 },
                    { "Vogue", 1990 },
                    { "Oceano", 1989 },
                    { "Flor de Lis", 1976 },
                    { "Samurai", 1982 },
                    { "Andar com Fé", 1982 },
                    { "Drão", 1982 },
                    { "Expresso 2222", 1972 },
                    { "O Leãozinho", 1977 },
                    { "Sampa", 1978 },
                    { "Cajuína", 1979 }
                });

            // Atualiza ArtistaId e GeneroId das músicas (Rock)
            migrationBuilder.Sql(@"
                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Foo Fighters'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Rock' AND M.Nome IN ('Everlong', 'The Pretender', 'Learn to Fly');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Queen'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Rock' AND M.Nome IN ('Bohemian Rhapsody', 'Don''t Stop Me Now', 'We Will Rock You');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Led Zeppelin'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Rock' AND M.Nome IN ('Stairway to Heaven', 'Whole Lotta Love', 'Black Dog');
            ");

            // Pop
            migrationBuilder.Sql(@"
                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Taylor Swift'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Pop' AND M.Nome IN ('Blank Space', 'Shake It Off', 'Love Story');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Michael Jackson'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Pop' AND M.Nome IN ('Billie Jean', 'Thriller', 'Beat It');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Madonna'
                CROSS JOIN Generos G
                WHERE G.Nome = 'Pop' AND M.Nome IN ('Like a Virgin', 'Material Girl', 'Vogue');
            ");

            // MPB
            migrationBuilder.Sql(@"
                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Djavan'
                CROSS JOIN Generos G
                WHERE G.Nome = 'MPB' AND M.Nome IN ('Oceano', 'Flor de Lis', 'Samurai');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Gilberto Gil'
                CROSS JOIN Generos G
                WHERE G.Nome = 'MPB' AND M.Nome IN ('Andar com Fé', 'Drão', 'Expresso 2222');

                UPDATE Musicas
                SET ArtistaId = A.Id, GeneroId = G.Id
                FROM Musicas M
                INNER JOIN Artistas A ON A.Nome = 'Caetano Veloso'
                CROSS JOIN Generos G
                WHERE G.Nome = 'MPB' AND M.Nome IN ('O Leãozinho', 'Sampa', 'Cajuína');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Musicas");
            migrationBuilder.Sql("DELETE FROM Artistas");
            migrationBuilder.Sql("DELETE FROM Generos");
        }
    }
}
