using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_5_3a
    {
        public static async Task ExecutarAsync()
        {
            var connectionString =
                "mongodb+srv://cauaaguiarti_db_user:zxX0IrVFIeNfJI6O@clusterteste.x2ruob5.mongodb.net/";
            // var client = new MongoClient(connectionString);
            // var database = client.GetDatabase("NoticiasDB");
            // var collection = database.GetCollection<BsonDocument>("noticias");

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            string jsonRecebido =
                @"{
    ""Titulo"": ""Festival de Música agita a cidade do Rio de Janeiro"",
    ""Texto"": ""O festival reuniu artistas nacionais e internacionais..."",
    ""DataPublicacao"": ""2024-11-22T20:30:00Z"",
    ""Tags"": [""cultura"", ""musica"", ""brasil""],
    ""Jornalistas"": [
        { ""Nome"": ""João Almeida"" }
    ],
    ""Comentarios"": [
        {
            ""Texto"": ""Evento sensacional!"",
            ""Curtidas"": 19,
            ""Usuario"": ""LarissaSantos"",
            ""Data"": ""2024-11-22T22:10:00Z""
        }
    ],
    ""Anexos"": [
        {
            ""NomeArquivo"": ""festival-musica.jpg"",
            ""Url"": ""https://meusite.com/img/festival-musica.jpg"",
            ""Tamanho"": 98000,
            ""Tipo"": ""image/jpeg"",
            ""Cliques"": 31
        }
    ],
    ""Visualizacoes"": 950,
    ""TotalComentarios"": 1,
    ""Gostei"": 76,
    ""NaoGostei"": 2,
    ""TempoMedioLeitura"": 3.1
}";

            var noticia = JsonConvert.DeserializeObject<Noticia>(jsonRecebido);

            try
            {
                await noticiaService.InserirNoticiaAsync(noticia);
                Console.WriteLine("Notícia incluida com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar inserir o JSON como string:");
                Console.WriteLine(ex.Message);
            }
            // var noticiaBson = noticia.ToBson();

            // try
            // {
            //     await collection.InsertOneAsync(noticiaBson);
            //     System.Console.WriteLine("Noticia incluida com sucesso!");
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("Ocorreu um erro ao tentar inserir o BSON como string:");
            //     Console.WriteLine(ex.Message);
            // }
        }
    }
}
