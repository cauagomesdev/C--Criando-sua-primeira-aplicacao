namespace ScreenSound.Modelos;

internal class Musica
{

    public Musica(Banda artista, string nome)
    {
        Artista = artista;
        Nome = nome;
    }

    public string Nome {get;}
    public Banda Artista {get;}
    public int Duracao {get; set;}
    public bool Disponivel {get; set;}
    public string DescricaoResumida =>  $"A música {Nome} pertence à {Artista}";

    public void ExibirFichaTecnica()
    {
        System.Console.WriteLine($"Nome: {Nome}");
        System.Console.WriteLine($"Artista: {Artista.Nome}");
        System.Console.WriteLine($"Duração: {Duracao}");
        if (Disponivel)
        {
            System.Console.WriteLine("Disponivel no plano.\n");
        } else
        {
            System.Console.WriteLine("Adquira o plano Plus+.\n");
        }
    }

}
