using System;
using System.Collections.Generic;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using CursoMongoDB.Services;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs
{
    public static class Program_6_5
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = Configuracao.ConnectionString;

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);
            try
            {
                await noticiaService.RemoverNoticiasComUrlDuplicadaAsync();
                Console.WriteLine("URLs repetidas removidas com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar remover as URLs repetidas:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
