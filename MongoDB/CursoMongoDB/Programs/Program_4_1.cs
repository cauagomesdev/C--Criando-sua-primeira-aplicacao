using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_4_1
    {
        public static void Executar()
        {
            var connectionString = Configuracao.ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            var collection = database.GetCollection<BsonDocument>("noticias");

            string jsonRecebido =
                @"{
    ""Titulo"": ""Vasco ganha fora de casa"",
    ""Texto"": ""Sendo visitante, o vasco bate o Athletico-pr e fica longe da zona..."",
    ""DataPublicacao"": ""2026-11-05T14:12:00Z"",
    ""Tags"": [ ""esporte"", ""Brasil""],
    ""Jornalistas"": [{ ""Nome"": ""Cauã"" }],
    ""Comentarios"": [{
        ""Texto"": ""Grande jogo"",
        ""Curtidas"": 0,
        ""Usuario"": ""Lescanofã"",
        ""Data"": ""2026-11-05T14:15:00Z""
    }],
    ""Anexos"": [{
        ""NomeArquivo"": ""foto-jogo.jpg"",
        ""Url"": ""https://meusite.com.br/foto/foto-jogo.jpg"",
        ""Tamanho"": 204800,
        ""Tipo"": ""imagem/jpg"",
        ""Cliques"": 0
    }],
    ""Visualizacoes"": 0,
    ""TotalComentarios"": 1,
    ""Gostei"": 0,
    ""NaoGostei"": 0,
    ""TempoMedioLeitura"": 0.0
}";

            var noticia = JsonConvert.DeserializeObject<Noticia>(jsonRecebido);

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
