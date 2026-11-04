using System.Text.Json;
using ReportGenerator;
using ScreenSound04;
using ScreenSound04.Filtros;
using ScreenSound04.Modelos;

using (HttpClient client = new HttpClient())
{
    try
    {
        // LinqFilter.FiltrarMusicasEmCSharp(musicas);
        // musicas[1].ExibirDetalhesDaMusica();
        // LinqFilter.FiltrarTodosOsGenerosMusicais(musicas);
        // LinqOrder.ExibirListaDeArtistasOrdenados(musicas);
        // LinqFilter.FiltrarArtistaPorGeneroMusical(musicas, "pop");
        // LinqFilter.FiltrarMusicasDeUmArtista(musicas, "Billie Eilish");
        // LinqFilter.FiltrarMusicasPorAno(musicas, 2020);

        // var musicasPreferidasDoCaua = new MusicasPreferidas("Caua");
        // musicasPreferidasDoCaua.AdicionarMusicasFavoritas(musicas[1]);
        // musicasPreferidasDoCaua.AdicionarMusicasFavoritas(musicas[44]);
        // musicasPreferidasDoCaua.AdicionarMusicasFavoritas(musicas[302]);
        // musicasPreferidasDoCaua.AdicionarMusicasFavoritas(musicas[1990]);
        // musicasPreferidasDoCaua.AdicionarMusicasFavoritas(musicas[10]);

        // musicasPreferidasDoCaua.ExibirMusicasFavoritas();
        // musicasPreferidasDoCaua.GerarArquivoJson();

        // var musicasPreferidasDaAna = new MusicasPreferidas("Ana");
        // musicasPreferidasDaAna.AdicionarMusicasFavoritas(musicas[77]);
        // musicasPreferidasDaAna.AdicionarMusicasFavoritas(musicas[1898]);
        // musicasPreferidasDaAna.AdicionarMusicasFavoritas(musicas[22]);
        // musicasPreferidasDaAna.AdicionarMusicasFavoritas(musicas[1998]);
        // musicasPreferidasDaAna.AdicionarMusicasFavoritas(musicas[26]);

        // musicasPreferidasDaAna.ExibirMusicasFavoritas();
        string resposta = await client.GetStringAsync(
            "https://guilhermeonrails.github.io/api-csharp-songs/songs.json"
        );
        var musicas = JsonSerializer.Deserialize<List<Musica>>(resposta)!;

        List<Dictionary<string, string>> reportData = new List<Dictionary<string, string>>();

        foreach (var musica in musicas)
        {
            var record = new Dictionary<string, string>
            {
                { "Nome", musica.Nome },
                { "Artista", musica.Artista },
                { "Genero", musica.Genero },
                { "Tonalidade", musica.Tonalidade },
            };
            reportData.Add(record);

            CSVGenerator reportGenerator = new CSVGenerator(reportData);
            string local = reportGenerator.GenerateCSV("musicas.csv");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro: {ex.Message}");
    }
}
