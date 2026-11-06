using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Driver;

namespace CursoMongoDB.Programs
{
    public static class Program_3_1a
    {
        public static void Executar()
        {
            var connectionString = Configuracao.ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("NoticiasDB");
            var collection = database.GetCollection<dynamic>("noticias");
            database.CreateCollection("noticias");
            System.Console.WriteLine(
                "Banco de dados 'NoticiasDB' e coleção 'noticias' criados com sucesso!"
            );
        }
    }
}
