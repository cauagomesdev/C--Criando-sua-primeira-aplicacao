using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CursoMongoDB.Programs
{
    public static class Program_3_3
    {
        public static void Executar()
        {
            var connectionString =
                "mongodb+srv://cauaaguiarti_db_user:zxX0IrVFIeNfJI6O@clusterteste.x2ruob5.mongodb.net/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            var collection = database.GetCollection<BsonDocument>("noticias");

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

            var noticiaBson = noticia.ToBson();

            try
            {
                collection.InsertOne(noticiaBson);
                System.Console.WriteLine("Noticia incluida com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar inserir o BSON como string:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
