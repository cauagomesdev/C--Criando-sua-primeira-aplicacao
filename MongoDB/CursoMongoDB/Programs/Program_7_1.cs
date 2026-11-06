using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_7_1
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            var Url = "nova_lei_incentiva_o_uso_de_energias_renovaveis";
            var NovoTexto =
                "Benefícios fiscais para empresas que investirem em energia limpa vão ser incentivadas para a melhoria do mundo.";

            try
            {
                await noticiaService.AtualizarTextoPorUrlAsync(Url, NovoTexto);
                Console.WriteLine("Texto da Notícia alterada com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar alterar o texto da Notícia:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
