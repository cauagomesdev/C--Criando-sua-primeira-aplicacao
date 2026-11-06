using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_6_3
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);
            try
            {
                await noticiaService.AtualizarUrlsFaltantesAsync();
                Console.WriteLine("URLs atualizadas com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar atualizar as URLs:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
