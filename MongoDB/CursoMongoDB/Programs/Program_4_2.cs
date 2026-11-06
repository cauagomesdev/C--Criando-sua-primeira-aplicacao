using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_4_2
    {
        public static void Executar()
        {
            var connectionString =
                "mongodb+srv://cauaaguiarti_db_user:zxX0IrVFIeNfJI6O@clusterteste.x2ruob5.mongodb.net/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            var collection = database.GetCollection<BsonDocument>("noticias");

            string caminhoArquivo =
                "c:\\Users\\caua.gomes\\Desktop\\NET com MongoDB\\4905 - Vídeo 4.2 - Lista.js";

            if (!File.Exists(caminhoArquivo))
            {
                Console.WriteLine("Arquivo noticias.json não encontrado!");
                return;
            }

            string conteudoJson = File.ReadAllText(caminhoArquivo);
            var listaNoticias = JsonConvert.DeserializeObject<List<Noticia>>(conteudoJson);
            int contador = 1;
            foreach (var noticia in listaNoticias)
            {
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
                Console.WriteLine(
                    $"Notícia {contador} inserida no MongoDB com sucesso: {noticia.Titulo}"
                );
                contador++;
            }
        }
    }
}
