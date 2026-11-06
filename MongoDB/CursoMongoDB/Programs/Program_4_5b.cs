using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_4_5b
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            var collection = database.GetCollection<BsonDocument>("noticias");

            string caminhoArquivo =
                "c:\\Users\\caua.gomes\\Desktop\\NET com MongoDB\\4905 - Vídeo 4.3 - Lista.js";

            if (!File.Exists(caminhoArquivo))
            {
                Console.WriteLine("Arquivo noticias.json não encontrado!");
                return;
            }

            string conteudoJson = File.ReadAllText(caminhoArquivo);
            var listaNoticias = JsonConvert.DeserializeObject<List<Noticia>>(conteudoJson);
            var listaBson = new List<BsonDocument>();

            foreach (var noticia in listaNoticias)
            {
                listaBson.Add(noticia.ToBson());
            }

            try
            {
                await collection.InsertManyAsync(listaBson);
                System.Console.WriteLine("Noticias incluidas com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar inserir a lista de noticias:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
