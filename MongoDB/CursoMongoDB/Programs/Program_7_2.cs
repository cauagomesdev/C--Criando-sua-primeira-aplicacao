using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_7_2
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            var Url = "evento_de_literatura_discute_inclusao_social";
            var comentario = new Comentario
            {
                Texto = "Incrível ver as pessoas tão engajadas com a inclusão social!",
                Curtidas = 3,
                Usuario = "MariaSilva",
                Data = DateTime.UtcNow,
            };

            try
            {
                await noticiaService.AdicionarComentarioAsync(Url, comentario);
                Console.WriteLine("Comentário adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar adicionar o comentário da Notícia:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
