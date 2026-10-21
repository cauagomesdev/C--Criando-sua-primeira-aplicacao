
using var arquivo = new FileStream("musicas.csv", FileMode.Open, FileAccess.Read);
using var stream = new StreamReader(arquivo);

var musicasDoColdPlay =
    ObterMusicas(stream) // 1. obtenção dos dados
    .Where( musica => musica.Titulo.StartsWith('C')) // 2. filtragem por artista
    .Where(m => m.Duracao <= 350); // 3. filtragem por duração

ExibirMusicas(musicasDoColdPlay);

void ExibirMusicas(IEnumerable<Musica> musicas)
{
    var contador = 1;
    System.Console.WriteLine($"\nExibindo as músicas: ");
    foreach (var musica in musicas)
    {
        System.Console.WriteLine($"\t - {musica.Titulo} ({musica.Artista}) - {musica.Duracao} segs");
        contador++;
        if(contador > 10) break;
    }
}



IEnumerable<Musica> ObterMusicas(StreamReader stream)
{
    var linha = stream.ReadLine();
    while(linha is not null)
    {
        var partes = linha.Split(';');
        var musica = new Musica
        {
            Titulo = partes[0],
            Artista = partes[1],
            Duracao = Convert.ToInt32(partes[2])
        };
        yield return musica;
        linha = stream.ReadLine();
    }
}

bool FiltrarPorArtista(Musica musica) => musica.Artista == "Coldplay";
bool FiltrarMaisLongas(Musica m) => m.Duracao >= 400;
bool FiltrarPorMettalica(Musica mus) => mus.Artista == "Metallica";
bool FiltrarPorTituloQueComecaComA(Musica musica) => musica.Titulo.StartsWith('A');

Func<Musica, bool> condicao = FiltrarMaisLongas;

static class MusicasExtensions
{
    public static IEnumerable<T> FiltrarPor(this IEnumerable<T> colecao, Func<T, bool> condicao)
{
    foreach (var elemento in colecao)
    {
        if (condicao(elemento)) yield return elemento;
    }
}

}

public class T
{
}

class Musica
{
    public string Titulo { get; set; }
    public string Artista { get; set; }

    public int Duracao { get; set; }
}
