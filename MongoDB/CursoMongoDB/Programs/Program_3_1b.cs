using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Driver;

namespace CursoMongoDB.Programs
{
    public static class Program_3_1b
    {
        public static void Executar()
        {
            var connectionString = Configuracao.ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            client.DropDatabase("NoticiasDB");
            System.Console.WriteLine("Banco de dados 'NoticiasDB' removido com sucesso!");
        }
    }
}
