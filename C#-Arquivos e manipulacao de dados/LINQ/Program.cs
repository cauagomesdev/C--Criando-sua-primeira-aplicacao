/*
Fluxo Padrão: Estágio 1 (Origem Dados) > Estágio 2 > ... > Estágio N

LINQ - Categorias de operações para manipulação de coleções
============================================================

Filtro (+)      | coleção c/ tam menor/igual atendendo condição | Where, Distinct
Projeção (+)    | coleção transformada, do mesmo tipo ou não     | Select, SelectMany
Ordenação (+)   | coleção ordenada pela expressão lambda        | OrderBy, ThenBy
Agregação (*)   | valor único a partir de operação de acúmulo   | Sum, Min, Max
Agrupamento (+) | coleção de grupos onde a chave é o argumento  | GroupBy
Elementos (*)   | elemento único T a partir do argumento        | First, Last, MinBy
Existência (*)  | booleano a partir da operação e argumento     | All, Any, Contains
Conversão (*)   | coleção em outra estrutura                    | ToList, ToArray

+ operações avalidas sob demanda (yield)
* operações avalidas imediatamente
*/

using var arquivo = new FileStream("musicas.csv", FileMode.Open, FileAccess.Read);
using var stream = new StreamReader(arquivo);

OperacoesDeVerificacaoDeExistencia(stream);

void OperacoesDeVerificacaoDeExistencia(StreamReader stream)
{
    var musicas = ObterMusicas(stream).ToList();

    var artistas = musicas
        .GroupBy(m => m.Artista)
        .Where(g => g.Any(m => m.Duracao >= 480));

        System.Console.WriteLine("\nArtistas com músicas de pelo menos 8 min: ");
        foreach (var artista in artistas)
        {
            System.Console.WriteLine($"\t - {artista.Key}");
        }

    var reggae = musicas
        .GroupBy(m => m.Artista)
        .Where(g => g.Any(m => m.Generos.Contains("Reggae")));

        System.Console.WriteLine("\nArtistas com músicas de reggae: ");
        foreach (var artista in reggae)
        {
            System.Console.WriteLine($"\t - {artista.Key}");
        }
}

void ArtistaComMaiorQtDeMusicas(StreamReader stream)
{
var artistaComMaiorQtDeMusicas = ObterMusicas(stream)
    .GroupBy(m => m.Artista)
    .Select(g => new {Artista = g.Key, Musicas = g, Total = g.Count()})
    .MaxBy(a => a.Total);

    if(artistaComMaiorQtDeMusicas is not null)
    System.Console.WriteLine($"""
    O artista com maior quantidade de músicas é o {artistaComMaiorQtDeMusicas.Artista} com {artistaComMaiorQtDeMusicas.Total} músicas
    """);
}

void OperacoesDeObtencaoDeElementos(StreamReader stream)
{
    var musicas = ObterMusicas(stream).ToList();

    var primeiraMusica = musicas.First();
    System.Console.WriteLine($"A primeira música é {primeiraMusica.Titulo}");

    var maiorDuracao = musicas.MaxBy(m => m.Duracao);
    if (maiorDuracao is not null)
    System.Console.WriteLine($"A música com maior duração é {maiorDuracao.Titulo} com duração de {maiorDuracao.Duracao} segundos");
}

void OperacoesDeAgrupamento(StreamReader stream)
{
    var artistas = ObterMusicas(stream)
        .GroupBy(m => m.Artista);

    System.Console.WriteLine("\nExibindo as músicas de cada artista: ");
    foreach (var artista in artistas.Take(5))
    {
        System.Console.WriteLine($"Artista: {artista.Key} com {artista.Count()} músicas");
        foreach (var musica in artista)
        {
            System.Console.WriteLine($"\t - {musica.Titulo}");
        }
    }
}

void EstatisticasDeMusicas(StreamReader stream)
{
    var musicas = ObterMusicas(stream).ToList();

    Console.WriteLine($"\nExistem {musicas.Count()} músicas na coleção.");
    Console.WriteLine($"\nExistem {musicas.Count(m => m.Duracao >= 600)} músicas com mais do que 10 minutos na coleção.");
    Console.WriteLine($"\nA música com menor duração da coleção leva {musicas.Min(m => m.Duracao)} segundos.");
    Console.WriteLine($"\nA música com maior duração da coleção leva {musicas.Max(m => m.Duracao)} segundos.");
    Console.WriteLine($"\nA duração média das músicas da coleção é {musicas.Average(m => m.Duracao)} segundos.");
    Console.WriteLine($"\nVocê vai levar {musicas.Sum(m => m.Duracao) / (3600 * 24)} dias para ouvir toda a coleção!");
}

void OperacoesDeProjecao2(StreamReader stream)
{
    var generos = ObterMusicas(stream)
    .SelectMany(m => m.Generos)
    .Distinct()
    .OrderBy(g => g);

    foreach (var genero in generos)
    {
        System.Console.WriteLine(genero);
    }

}

void OperacaoesDeProjecao(StreamReader stream)
{
    var artistas = ObterMusicas(stream)
        .Select(m => m.Artista)
        .Distinct()
        .OrderBy(a => a);

    foreach (var artista in artistas)
    {
        System.Console.WriteLine(artista);
    }
}

void OperacoesDeFiltroEOrdenacao(StreamReader stream)
{
    var musicasDoColdPlay =
        ObterMusicas(stream)
        .Where(musica => musica.Artista == "Coldplay")
        .OrderBy(m => m.Titulo)
        .Skip(5 * 2)
        .Take(5);
    ExibirMusicas(musicasDoColdPlay);
}

void ExibirMusicas(IEnumerable<Musica> musicas)
{
    var contador = 1;
    System.Console.WriteLine($"\nExibindo as músicas: ");
    foreach (var musica in musicas)
    {
        System.Console.WriteLine($"\t - {musica.Titulo} ({musica.Artista}) - {musica.Duracao} segs");
        contador++;
        if (contador > 10) break;
    }
}

IEnumerable<Musica> ObterMusicas(StreamReader stream)
{
    var linha = stream.ReadLine();
    while (linha is not null)
    {
        var partes = linha.Split(';');
        var musica = new Musica
        {
            Titulo = partes[0],
            Artista = partes[1],
            Duracao = Convert.ToInt32(partes[2]),
            Generos = partes[3].Split(',').Select(g => g.Trim())
        };
        yield return musica;
        linha = stream.ReadLine();
    }
    System.Console.WriteLine("Chegou ao fim do processamento");
}

class Musica
{
    public string Titulo { get; set; }
    public string Artista { get; set; }
    public int Duracao { get; set; }
    public IEnumerable<string> Generos { get; set; }
}
