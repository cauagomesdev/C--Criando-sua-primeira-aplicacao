using System.Collections;

var musica1 = new Musica { Titulo = "Bandoleira", Artista = "Ryan SP", Duracao = 450 };
var musica2 = new Musica { Titulo = "B.E.D", Artista = "Jacquees", Duracao = 310 };
var musica3 = new Musica { Titulo = "Cadeira Cativa", Artista = "Zé Neto e Cristiano", Duracao = 200 };
var musica4 = new Musica { Titulo = "Licor 43", Artista = "Kayblack", Duracao = 180 };
var musica5 = new Musica { Titulo = "Andei Só", Artista = "Natiruts", Duracao = 220 };
var musica6 = new Musica { Titulo = "Sal e pimenta", Artista = "Kayblack", Duracao = 240 };
var musica7 = new Musica { Titulo = "Discreto", Artista = "Kayblack", Duracao = 230 };


var playlistDoCaua = new Playlist { Nome = "Playlist Eclética" };
playlistDoCaua.Add(musica1);
playlistDoCaua.Add(musica2);
playlistDoCaua.Add(musica3);
playlistDoCaua.Add(musica4);
playlistDoCaua.Add(musica5);
playlistDoCaua.Add(musica6);
playlistDoCaua.Add(musica7);
playlistDoCaua.Add(new Musica { Titulo = "Cadeira Cativa", Artista = "Zé Neto e Cristiano", Duracao = 200 });

// ExibirPlaylist(playlistDoCaua);

var kayblack = new Playlist() { Nome = "Mais populares do Kayblack" };
kayblack.Add(musica4);
kayblack.Add(musica6);
kayblack.Add(musica7);

// ExibirPlaylist(kayblack);
// ExibirMaisTocadas(playlistDoCaua, kayblack);

var player = new PlayerDeMusica();
player.AdicionarNaFila(musica1);
player.AdicionarNaFila(playlistDoCaua);


ExibirFila(player);
ExibirHistorico(player);

var proxima = player.ProximaMusicaDaFila();
if (proxima is not null)
{
    System.Console.WriteLine($"\nTocando a música {proxima.Titulo}");
}
else
{
    System.Console.WriteLine("Fila de reprodução vazia");
}

ExibirFila(player);
ExibirHistorico(player);

var anterior = player.MusicaAnterior();
if (anterior is not null)
{
    System.Console.WriteLine($"\nTocando a música {anterior.Titulo}");
}
else
{
    System.Console.WriteLine("Histórico de reprodução vazio");
}

ExibirFila(player);
ExibirHistorico(player);

void ExibirHistorico(PlayerDeMusica player)
{
    System.Console.WriteLine($"\nExibindo histórico: ");
    foreach (var musica in player.Historico())
    {
        System.Console.WriteLine($"\t - {musica.Titulo}");
    }
}

void ExibirFila(PlayerDeMusica player)
{

    System.Console.WriteLine($"\nExibindo a fila de reprodução: ");
    foreach (var musica in player.Fila())
    {
        System.Console.WriteLine($"\t - {musica.Titulo}");
    }
}

void ExibirMaisTocadas(Playlist playlist1, Playlist playlist2)
{
    // Musica (chave/key), Contagem (valor/value) 
    Dictionary<Musica, int> ranking = [];

    foreach (var musica in playlist1)
    {
        ranking.Add(musica, 1);
    }
    foreach (var musica in playlist2)
    {
        if (ranking.TryGetValue(musica, out int contagem))
        {
            contagem++;
            ranking[musica] = contagem;
        }
        else
        {
            ranking[musica] = 1;
        }
    }

    KeyValuePair<Musica, int> elemento;
    List<KeyValuePair<Musica, int>> top = new(ranking);//[..ranking];
    top.Sort(new PorContagem());

    System.Console.WriteLine("\nTop 3 músicas mais incluídas nas playlists");
    int contador = 1;
    foreach (var par in top)
    {
        System.Console.WriteLine($"\t - {par.Key.Titulo}");
        contador++;
        if (contador > 3) break;
    }
}

// playlistDoCaua.OrdenarPorDuracao();
// ExibirPlaylist(playlistDoCaua);

// playlistDoCaua.OrdenarPorArtista();
// ExibirPlaylist(playlistDoCaua);

void ExibirPlaylist(Playlist playlist)
{
    System.Console.WriteLine($"\n Tocando as música de {playlist.Nome}");
    foreach (var musica in playlist)
    {
        System.Console.WriteLine($"\t - {musica.Titulo} ({musica.Artista}) - {musica.Duracao} segundos");
    }
}

void RemoverMusicaPeloTitulo(Playlist playlist, string titulo)
{
    var musicaEncontrada = playlistDoCaua.ObterPeloTitulo(titulo);
    if (musicaEncontrada is not null)
    {
        System.Console.WriteLine("\nRemovendo música...");
        playlistDoCaua.Remove(musicaEncontrada);
    }
    else
    {
        System.Console.WriteLine("\nMúsica não encontrada!");
    }
    ExibirPlaylist(playlistDoCaua);
}

void ExibirMusicaAleatoria(Playlist playlist)
{
    var musicaAleatoria = playlistDoCaua.ObterAleatoria();
    if (musicaAleatoria is not null)
    {
        System.Console.WriteLine($"\nA música aleatória é {musicaAleatoria.Titulo}");
    }
}

class PorContagem : IComparer<KeyValuePair<Musica, int>>
{
    public int Compare(KeyValuePair<Musica, int> x, KeyValuePair<Musica, int> y)
    {
        return y.Value.CompareTo(x.Value);
    }
}

class PorArtista : IComparer<Musica>
{
    public int Compare(Musica? x, Musica? y)
    {
        if (x is null || y is null) return 0;
        if (x is null) return 1;
        if (y is null) return -1;
        return x.Artista.CompareTo(y.Artista);
    }
}

class PorTitulo : IComparer<Musica>
{
    public int Compare(Musica? x, Musica? y)
    {
        if (x is null || y is null) return 0;
        if (x is null) return 1;
        if (y is null) return -1;
        return x.Titulo.CompareTo(y.Titulo);
    }
}

class Musica : IComparable
{
    public string Titulo { get; set; }
    public string Artista { get; set; }
    public int Duracao { get; set; }
    public int CompareTo(object? other) // iguais: 0; menor = -1; maior = 1;
    {
        if (other is null) return -1;
        if (other is Musica outraMusica) return this.Duracao.CompareTo(outraMusica.Duracao);
        return -1;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is Musica outraMusica)
            return this.Titulo.Equals(outraMusica.Titulo) && this.Artista.Equals(outraMusica.Artista);
        return false;
    }

    public override int GetHashCode()
    {
        return this.Titulo.GetHashCode() ^ this.Artista.GetHashCode();
    }
}

class Playlist : ICollection<Musica>
{
    private HashSet<Musica> set = [];
    private List<Musica> lista = [];
    public string Nome { get; set; }

    public int Count => lista.Count;

    public bool IsReadOnly => false;

    public void Add(Musica musica)
    {
        if (set.Add(musica))
        {
            lista.Add(musica);
        }
    }

    public void Clear()
    {
        lista.Clear();
    }

    public bool Contains(Musica item)
    {
        return lista.Contains(item);
    }

    public Musica? ObterPeloTitulo(string titulo)
    {
        foreach (var musica in lista)
        {
            if (musica.Titulo == titulo) return musica;
        }
        return null;
    }
    public Musica? ObterAleatoria()
    {
        if (lista.Count == 0) return null;
        var random = new Random();
        var indiceAleatorio = random.Next(0, lista.Count - 1);
        return lista[indiceAleatorio];
    }
    public void CopyTo(Musica[] array, int arrayIndex)
    {
        lista.CopyTo(array, arrayIndex);
    }

    public void OrdenarPorDuracao()
    {
        lista.Sort(); //duracao IComparable
    }

    public void OrdenarPorArtista()
    {
        lista.Sort(new PorArtista());
    }
    public IEnumerator<Musica> GetEnumerator()
    {
        return lista.GetEnumerator();
    }

    public bool Remove(Musica item)
    {
        return lista.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class PlayerDeMusica
{
    private Queue<Musica> fila = []; //FiFo
    private Stack<Musica> pilha = []; //LiFo
    public void AdicionarNaFila(Musica musica)
    {
        fila.Enqueue(musica);
    }

    public void AdicionarNaFila(Playlist playlist)
    {
        foreach (var musica in playlist)
        {
            AdicionarNaFila(musica);
        }
    }
    public Musica? ProximaMusicaDaFila()
    {
        if (fila.Count == 0) return null;
        var musica = fila.Dequeue();
        pilha.Push(musica);
        return musica;
    }

    public Musica? MusicaAnterior()
    {
        if (pilha.Count == 0) return null;
        return pilha.Pop();
    }

    public IEnumerable<Musica> Fila()
    {
        foreach (var musica in fila)
        {
            yield return musica;
        }
    }
    public IEnumerable<Musica> Historico()
    {
        foreach (var musica in pilha)
            yield return musica;
    }
}