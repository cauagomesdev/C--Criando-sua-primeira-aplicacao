using System;
using System.Collections.Generic;
using CursoMongoDB.Models;

namespace CursoMongoDB.Programs
{
    public static class Program_2_4
    {
        public static void Executar()
        {
            var noticia = new Noticia
            {
                Titulo = "Vasco ganha fora de casa",
                Texto = "Sendo visitante, o vasco bate o Athletico-pr e fica longe da zona...",
                DataPublicacao = DateTime.Parse("2026-11-05T14:12:00Z"),
                Tags = new List<string> { "esporte", "Brasil" },
                Jornalistas = new List<Jornalista> { new Jornalista { Nome = "Cauã" } },
                Comentarios = new List<Comentario>
                {
                    new Comentario
                    {
                        Texto = "Grande jogo",
                        Curtidas = 0,
                        Usuario = "Lescanofã",
                        Data = DateTime.Parse("2026-11-05T14:15:00Z"),
                    },
                },
                Anexos = new List<Anexo>
                {
                    new Anexo
                    {
                        NomeArquivo = "foto-jogo.jpg",
                        Url = "https://meusite.com.br/foto/foto-jogo.jpg",
                        Tamanho = 204800,
                        Tipo = "imagem/jpg",
                        Cliques = 0,
                    },
                },
                Visualizacoes = 0,
                TotalComentarios = 1,
                Gostei = 0,
                NaoGostei = 0,
                TempoMedioLeitura = 0.0,
            };
            Console.WriteLine("Notícia criada:");
            Console.WriteLine($"Título: {noticia.Titulo}");
            Console.WriteLine($"Data: {noticia.DataPublicacao}");
            Console.WriteLine($"Texto: {noticia.Texto}");
            Console.WriteLine($"Tags: {string.Join(", ", noticia.Tags)}");
            Console.WriteLine(
                $"Jornalista(s): {string.Join(", ", noticia.Jornalistas.ConvertAll(j => j.Nome))}"
            );
            Console.WriteLine($"Visualizações: {noticia.Visualizacoes}");
            Console.WriteLine($"Gostei: {noticia.Gostei} / Não Gostei: {noticia.NaoGostei}");
            Console.WriteLine($"Tempo médio de leitura: {noticia.TempoMedioLeitura} min");
            Console.WriteLine($"Total de comentários: {noticia.TotalComentarios}");
            Console.WriteLine(
                $"Anexos: {string.Join(", ", noticia.Anexos.ConvertAll(a => a.NomeArquivo))}"
            );
        }
    }
}
