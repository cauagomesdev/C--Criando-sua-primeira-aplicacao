using var arquivo = new FileStream("musicas.csv", FileMode.Open, FileAccess.Read);
using var stream = new StreamReader(arquivo);

var musicas = ObterMusicas(stream).Take(20);
ExibirMusicasEmTabela(musicas);

void Interning()
{
    var artista1 = "Coldplay";
    var artista2 = "Coldplay";
    var artista3 = new string("Coldplay");
    var artista4 = "COLDPLAY";
    var artista5 = string.Intern(artista1.ToUpper());

    System.Console.WriteLine(artista1 == artista2);
    System.Console.WriteLine(artista1, artista3);
    System.Console.WriteLine(artista1, artista3);
    System.Console.WriteLine(artista4, artista5);
}

void ComparandoStrings(StreamReader stream)
{
    var musicas = ObterMusicas(stream)
        .Where(musica => musica.Artista.Equals("COLDPLAY", StringComparison.OrdinalIgnoreCase))
        .Take(20);
    // métodos que utilizam StringComparison
    "Coldplay".Equals("coldplay", StringComparison.OrdinalIgnoreCase);
    "Coldplay".StartsWith("cold", StringComparison.OrdinalIgnoreCase);
    "Coldplay".EndsWith("coldplay", StringComparison.OrdinalIgnoreCase);
    "Coldplay".IndexOf("coldplay", StringComparison.OrdinalIgnoreCase);
    "Coldplay".Contains("OLD", StringComparison.OrdinalIgnoreCase);
    "Coldplay".Replace("cold", "warm", StringComparison.OrdinalIgnoreCase);

    ExibirMusicasEmTabela(musicas);
}

void AlterandoOTitulo(StreamReader stream)
{
    var musica = ObterMusicas(stream).Where(m => m.Titulo.StartsWith('T')).FirstOrDefault();

    if (musica is not null)
    {
        System.Console.WriteLine($"Titulo da música: {musica.Titulo}");

        musica.Titulo = musica.Titulo.Replace("The ", "");
        System.Console.WriteLine($"Titulo da música: {musica.Titulo}");
    }
}

// var titulo = "Músicas do arquivo";
// foreach (var letra in titulo)
// {
//     System.Console.WriteLine(letra);
// }

void ValidandoSenha()
{
    var senha = "AndreAll8#";

    var totalCaracteres = senha.Length;
    var totalLetrasMaiusculas = senha.Count(c => char.IsUpper(c));
    var totalLetrasMinusculas = senha.Count(c => char.IsLower(c));
    var totalNumeros = senha.Count(c => char.IsDigit(c));
    var totalSimbolos = senha.Count(c => !char.IsLetterOrDigit(c));

    if (
        totalCaracteres < 8
        || totalLetrasMaiusculas == 0
        || totalLetrasMinusculas == 0
        || totalNumeros == 0
        || totalSimbolos == 0
    )
    {
        System.Console.WriteLine("A senha é fraca");
    }
    else
    {
        System.Console.WriteLine("A senha é forte");
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

        if (partes.Length == 5)
        {
            // bool sucesso = int.TryParse(partes[2], out int duracao);

            var musica = new Musica
            {
                Titulo = string.IsNullOrWhiteSpace(partes[0]) ? "Titulo não encontrado" : partes[0],
                Artista = string.IsNullOrWhiteSpace(partes[1])
                    ? "Artista não encontrado"
                    : partes[1],
                Duracao = int.TryParse(partes[2], out int duracao) ? duracao : 350,
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
