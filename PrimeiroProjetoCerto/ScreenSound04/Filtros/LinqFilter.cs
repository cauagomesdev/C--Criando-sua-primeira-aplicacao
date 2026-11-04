using System.Runtime.InteropServices;
using ScreenSound04.Modelos;

namespace ScreenSound04.Filtros;

internal class LinqFilter
{
    public static void FiltrarTodosOsGenerosMusicais(List<Musica> musicas)
    {
        var todosOsGenerosMusicais = musicas.Select(generos => generos.Genero).Distinct().ToList();
        foreach (var genero in todosOsGenerosMusicais)
        {
            System.Console.WriteLine($"- {genero}");
        }
    }

    public static void FiltrarArtistaPorGeneroMusical(List<Musica> musicas, string genero)
    {
        var artistasPorGeneroMusical = musicas.Where(musica => musica.Genero!.Contains(genero)).Select(musica => musica.Artista).Distinct().ToList();
        System.Console.WriteLine($"Exibindo os artistas por gênero musical >>> {genero}");
        foreach (var artista in artistasPorGeneroMusical)
        {
            System.Console.WriteLine($"- {artista}");
        } 
    }

    public static void FiltrarMusicasDeUmArtista(List<Musica> musicas, string nomeDoArtista)
    {
        var musicaDoArtista = musicas.Where(musica => musica.Artista!.Equals(nomeDoArtista)).ToList();
        System.Console.WriteLine($"Exibindo as músicas do artista >>> {nomeDoArtista}");
        foreach (var musica in musicaDoArtista)
        {
            Console.WriteLine($"- {musica.Nome}");
        }
    }

    public static void FiltrarMusicasPorAno(List<Musica> musicas, int ano)
    {
        var musicasDoAno = musicas.Where(musica => musica.Ano! == ano).OrderBy(musicas => musicas.Nome).Select(musicas => musicas.Nome).Distinct().ToList();
        System.Console.WriteLine($"Exibindo as músicas do ano >>> {ano}");
        foreach (var musica in musicasDoAno)
        {
            System.Console.WriteLine($"- {musica}");
        }
    }

    internal static void FiltrarMusicasEmCSharp(List<Musica> musicas)
    {
        var musicasEmCSharp = musicas.Where(musica => musica.Tonalidade!.Equals("C#")).Select(musica => musica.Nome).ToList();
        System.Console.WriteLine($"Exibindo as músicas na tonalidade de C#");
        foreach (var musica in musicasEmCSharp)
        {
            System.Console.WriteLine($"- {musica}");
        }
    }
}
