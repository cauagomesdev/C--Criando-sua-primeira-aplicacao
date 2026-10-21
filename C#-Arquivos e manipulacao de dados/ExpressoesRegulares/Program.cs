using System.Text.RegularExpressions;

using var arquivo = new FileStream("musicas.csv", FileMode.Open, FileAccess.Read);
using var stream = new StreamReader(arquivo);

// var musicas = ObterMusicas(stream).Take(20);
// ExibirMusicasEmTabela(musicas);
MusicasComNumerosRomanos();

void MusicasComNumerosRomanos()
{
    var regex = new Regex(@"\b[IVXLCDM]+\b");
    var musicas = ObterMusicas(stream).Where(m => regex.IsMatch(m.Titulo)).Take(20);
    ExibirMusicasEmTabela(musicas);
}

void MusicasComLetrasRepetidas()
{
    var regex = new Regex(@"^(?i)(\w).*\1$");
    var musicas = ObterMusicas(stream).Where(m => regex.IsMatch(m.Titulo)).Take(20);
    ExibirMusicasEmTabela(musicas);
}

void MusicasQueComecamETerminamComAMesmaPalavra()
{
    var regex = new Regex(@"^(\w+)\b.*\b\1$");
    var musicas = ObterMusicas(stream).Where(m => regex.IsMatch(m.Titulo)).Take(20);
    ExibirMusicasEmTabela(musicas);
}

void MusicasComDuasPalavras()
{
    var regex = new Regex(@"^\w+ \w+$");
    var musicas = ObterMusicas(stream).Where(m => regex.IsMatch(m.Titulo)).Take(20);
    ExibirMusicasEmTabela(musicas);
}

void ArtistasComCaracteresEspeciais()
{
    var regex = new Regex(@"[^a-zA-Z0-9 ]");

    var artistas = ObterMusicas(stream)
        .Where(m => regex.IsMatch(m.Artista))
        .Select(m => m.Artista)
        .Distinct()
        .OrderBy(a => a);
    foreach (var artista in artistas)
    {
        System.Console.WriteLine(artista);
    }
}

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
