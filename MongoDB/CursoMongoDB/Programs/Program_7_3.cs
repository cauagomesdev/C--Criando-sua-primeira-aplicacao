using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_7_3
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            var Url = "evento_de_literatura_discute_inclusao_social";
            var comentarioTexto = "Incrível ver as pessoas tão engajadas com a inclusão social!";

            try
            {
                await noticiaService.RemoverComentarioAsync(Url, comentarioTexto);
                Console.WriteLine("Comentário excluido com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao excluir o comentário:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
