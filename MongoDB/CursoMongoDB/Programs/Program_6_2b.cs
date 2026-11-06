using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_6_2b
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            string jsonRecebido =
                @"{
    ""Titulo"": ""Arqueólogos Encontram Templo Perdido de Civilização Pré-Inca"",
    ""Texto"": ""Durante escavações na região andina do Peru, pesquisadores encontraram vestígios de um templo cerimonial que antecede o Império Inca, com artefatos preservados e inscrições desconhecidas."",
    ""DataPublicacao"": ""2025-07-10T14:20:00Z"",
    ""Jornalistas"": [
        { ""Nome"": ""Sofia Ramires"" }
    ],
    ""Comentarios"": [
        {
            ""XXXXXXXXXXXX"": ""Incrível avanço para o entendimento das culturas pré-colombianas!"",
            ""Curtidas"": 31,
            ""Usuario"": ""ExploradorAndino"",
            ""Data"": ""2025-07-10T16:10:00Z""
        }
    ],
    ""Anexos"": [
        {
            ""NomeArquivo"": ""templo-perdido.jpg"",
            ""Url"": ""https://meusite.com/img/templo-perdido.jpg"",
            ""Tamanho"": 102400,
            ""Tipo"": ""image/jpeg"",
            ""Cliques"": 48
        }
    ],
    ""Visualizacoes"": 780,
    ""TotalComentarios"": 1,
    ""Gostei"": 89,
    ""NaoGostei"": 1,
    ""TempoMedioLeitura"": 3.8
}";

            var noticia = JsonConvert.DeserializeObject<Noticia>(jsonRecebido);

            try
            {
                await noticiaService.InserirNoticiaAsync(noticia);
                Console.WriteLine("Notícia incluida com sucesso!");
            }
            catch (MongoWriteException mwx) when (mwx.WriteError?.Code == 121)
            {
                NoticiaService.TratarErroValidacaoMongo(mwx);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar inserir o JSON como string:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
