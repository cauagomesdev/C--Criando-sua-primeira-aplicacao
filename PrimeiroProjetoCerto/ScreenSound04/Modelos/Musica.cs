using System.Text.Json.Serialization;

namespace ScreenSound04.Modelos;

internal class Musica
{

    private string[] tonalidades = new string[] { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "Aa", "A", "Bb", "B" };

    [JsonPropertyName("song")]
    public string? Nome { get; set; }

    [JsonPropertyName("artist")]
    public string? Artista { get; set; }

    [JsonPropertyName("duration_ms")]
    public int? Duracao { get; set; }

    [JsonPropertyName("genre")]
    public string? Genero { get; set; }

    [JsonPropertyName("key")]
    public int Key { get; set; }
    public string Tonalidade
     { get
        {
            return tonalidades[Key];
        }  
     }

    [JsonPropertyName("year")]
    public string? AnoString { get; set; }

    public int Ano 
    {
         get
        {
            return int.Parse(AnoString!);
        } 
    }
    public void ExibirDetalhesDaMusica()
    {
        System.Console.WriteLine($"Artista: {Artista}");
        System.Console.WriteLine($"Música: {Nome}");
        System.Console.WriteLine($"Duração em segundos: {Duracao/1000}");
        System.Console.WriteLine($"Gênero musical: {Genero}");
        System.Console.WriteLine($"Ano de lançamento: {Ano}");
        System.Console.WriteLine($"Tonalidade: {Tonalidade}");
    }
}
