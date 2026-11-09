using System.Diagnostics;
using UsuarioLib;

// Usuario usuario = new Usuario("João Silva", "joao@email.com", new List<string> { "12345678" });

// // Usuario usuario2 = new Usuario("Maria Souza", "maria@email.com", new List<string> { "87654321" });

// usuario.ExibeTelefones();
// usuario.PadronizaTelefones();
// usuario.ExibeTelefones();

Coordenada coordenada = new Coordenada(-23.5505, -46.6333);

Stopwatch sw = new Stopwatch();

sw.Start();

for (int i = 0; i < 10000000; i++)
{
    Coordenada c = new Coordenada(-23.5505, -46.6333);
    var lat = c.Latitude;
    var lon = c.Longitude;
}

sw.Stop();
System.Console.WriteLine($"Tempo gasto: {sw.Elapsed.TotalMilliseconds} ms");
