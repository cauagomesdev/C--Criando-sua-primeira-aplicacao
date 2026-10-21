using System.Text.Json;
using System.Text.RegularExpressions;

using var arquivo = new FileStream("musicas.csv", FileMode.Open, FileAccess.Read);
using var stream = new StreamReader(arquivo);

// var musicas = ObterMusicas(stream).Take(20);
// ExibirMusicasEmTabela(musicas);

/*

    1. Crie uma coleção de artistas com suas músicas ordenadas por data de lançamento. Inclua o total de músicas em uma propriedade separada.
    2. Gere um arquivo no formato JSON com essa coleção
 
*/

var artistas = ObterMusicas(stream)
    .GroupBy(m => m.Artista)
    .Select(g => new
    {
        Artista = g.Key,
        Musicas = g.OrderBy(m => m.Lancamento),
        Total = g.Count(),
    })
    .ToList();

var options = new JsonSerializerOptions { WriteIndented = true };

var nomeArquivo = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "artistas.json"
);
using var arquivoJson = new FileStream(nomeArquivo, FileMode.Create, FileAccess.Write);
JsonSerializer.Serialize(arquivoJson, artistas, options);
System.Console.WriteLine("Serialização concluida!");

void ExibirMusicas(IEnumerable<Musica> musicas)
{
    var titulo = $"\nExibindo as músicas: "; //string literal
    System.Console.WriteLine(titulo);
    foreach (var musica in musicas)
    {
        var linha =
            $"\t - {musica.Titulo} ({musica.Artista}) - {musica.Duracao}s [{musica.Lancamento}]";
        System.Console.WriteLine(linha);
    }
}

void ExibirMusicasEmTabela(IEnumerable<Musica> musicas)
{
    var titulo = $"\nExibindo as músicas: "; //string literal
    System.Console.WriteLine(titulo);

    var colunaTitulo = "titulo".PadRight(40);
    var colunaArtista = "Artista".PadRight(30);
    var colunaDuracao = "Duração".PadRight(10);
    var colunaLancamento = "Lançada Em".PadRight(15);
    System.Console.WriteLine($"{colunaTitulo}{colunaArtista}{colunaDuracao}{colunaLancamento}");
    var borda = "".PadRight(100, '=');
    System.Console.WriteLine(borda);

    foreach (var musica in musicas)
    {
        var linha =
            $"{musica.Titulo, -40}{musica.Artista, -30}{musica.Duracao / 60.0, -10:F3}{musica.Lancamento, -15:dd/MM//yyyy}";
        System.Console.WriteLine(linha);
    }
}

IEnumerable<Musica> ObterMusicas(StreamReader stream)
{
    var linha = stream.ReadLine();
    while (linha is not null)
    {
        var partes = linha.Split(';');

        int duracao = 350;
        var match = Regex.Match(linha, @"(\d?\d):(\d\d)");

        if (match.Success)
        {
            var minutos = int.Parse(match.Groups[1].Value);
            var segundos = int.Parse(match.Groups[2].Value);
            duracao = (minutos * 60) + segundos;
        }
        if (partes.Length == 5)
        {
            // bool sucesso = int.TryParse(partes[2], out int duracao);

            var musica = new Musica
            {
                Titulo = string.IsNullOrWhiteSpace(partes[0]) ? "Titulo não encontrado" : partes[0],
                Artista = string.IsNullOrWhiteSpace(partes[1])
                    ? "Artista não encontrado"
                    : partes[1],
                Duracao = duracao,
                Generos = partes[3].Split(',', StringSplitOptions.TrimEntries),
                Lancamento = DateTime.TryParse(partes[4], out var data) ? data : DateTime.Today,
            };
            yield return musica;
        }
        linha = stream.ReadLine();
    }
}

class Musica
{
    public string Titulo { get; set; }
    public string Artista { get; set; }
    public int Duracao { get; set; }
    public IEnumerable<string> Generos { get; set; }
    public DateTime Lancamento { get; set; }

    public override string ToString()
    {
        return $"{Titulo} ({Artista}) - {Duracao}s [{Lancamento}]";
        ;
    }
}
