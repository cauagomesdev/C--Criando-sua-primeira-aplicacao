using System.Diagnostics;
using UsuarioLib;

//  Usuario usuario = new Usuario("João Silva", "joao@email.com", new List<string> { "12345678" });

//  Usuario usuario2 = new Usuario("Maria Souza", "maria@email.com", new List<string> { "87654321" });

//  usuario.ExibeTelefones();
//  usuario.PadronizaTelefones();
//  usuario.ExibeTelefones();

// Coordenada coordenada = new Coordenada(-23.5505, -46.6333);

// Stopwatch sw = new Stopwatch();

// sw.Start();

// for (int i = 0; i < 10000000; i++)
// {
//     Coordenada c = new Coordenada(-23.5505, -46.6333);
//     var lat = c.Latitude;
//     var lon = c.Longitude;
// }

// sw.Stop();
// System.Console.WriteLine($"Tempo gasto: {sw.Elapsed.TotalMilliseconds} ms");

// FormularioDto formulario = new FormularioDto("Joao", "2243211", "desenvolvedor") { Idade = 20 };
// formulario.Idade = 25;
// Console.WriteLine($" Valor do nome: {formulario.Nome}");

// System.Console.WriteLine(
//     $"Nome: {formulario.Nome}, CPF: {formulario.Cpf}, Idade: {formulario.Idade}, Cargo: {formulario.Cargo}"
// );

// FormularioDto formulario2 = new FormularioDto();
// formulario2.Nome = "João Silva";
// formulario2.Cpf = "123.456.789-00";
// formulario2.Idade = 30;
// formulario2.Cargo = "Desenvolvedor";

// System.Console.WriteLine(formulario == formulario2); // false

// UsuarioDto usuarioDto = new UsuarioDto();
// usuarioDto.Nome = "João Silva";
// usuarioDto.Email = "joao@email.com";
// usuarioDto.Telefones = new List<string> { "12345678" };

// UsuarioDto usuarioDto2 = new UsuarioDto();
// usuarioDto2.Nome = "João Silva";
// usuarioDto2.Email = "joao@email.com";
// usuarioDto2.Telefones = new List<string> { "12345678" };

// System.Console.WriteLine(usuarioDto == usuarioDto2);

// for (int i = 0; i < 10000000; i++)
// {
//     FormularioDto formulario = new FormularioDto("Joao", "2243211", 20, "desenvolvedor");
// }

// FormularioDto dto = new FormularioDto("Daniel", "11111111111", 100, "Programador");

// for (int i = 0; i < 1000000000; i++)
// {
//     Coordenada coordenada = new Coordenada(123.456, -123.456);
//     var latitude = coordenada.Latitude;
//     var longitude = coordenada.Longitude;
// }

Stopwatch sw = new Stopwatch();

sw.Start();

for (int i = 0; i < 1000000000; i++)
{
    FormularioDtoClass dto = new FormularioDtoClass("Daniel", "11111111111", "Programador", 100);
}

sw.Stop();

Console.WriteLine($"Tempo class: {sw.Elapsed.TotalMilliseconds}");

sw.Restart();

for (int i = 0; i < 1000000000; i++)
{
    FormularioDtoRecord dto = new FormularioDtoRecord("Daniel", "11111111111", "Programador", 100);
}

sw.Stop();

Console.WriteLine($"Tempo record: {sw.Elapsed.TotalMilliseconds}");

sw.Restart();

for (int i = 0; i < 1000000000; i++)
{
    FormularioDtoStruct dto = new FormularioDtoStruct("Daniel", "11111111111", "Programador", 100);
}

sw.Stop();

Console.WriteLine($"Tempo struct: {sw.Elapsed.TotalMilliseconds}");

sw.Restart();

for (int i = 0; i < 1000000000; i++)
{
    FormularioDtoRecordStruct dto = new FormularioDtoRecordStruct(
        "Daniel",
        "11111111111",
        "Programador",
        100
    );
}

sw.Stop();

Console.WriteLine($"Tempo record struct: {sw.Elapsed.TotalMilliseconds}");
