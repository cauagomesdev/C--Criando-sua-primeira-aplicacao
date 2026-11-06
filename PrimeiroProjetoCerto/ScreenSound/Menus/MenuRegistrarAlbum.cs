using ScreenSound.Modelos;

namespace ScreenSound.Menus;

internal class MenuRegistrarAlbum : Menu
{
    public override void Executar(Dictionary<string, Banda> bandasRegistradas)
    {
        base.Executar(bandasRegistradas);
        ExibirTituloDaOpcao("Registro dos albúns");
        Console.Write("Digite o nome da banda que quer registrar no albúm: ");
        string nomeDaBanda = Console.ReadLine()!;
        if (bandasRegistradas.ContainsKey(nomeDaBanda))
        {
            System.Console.WriteLine("Digite o nome do albúm que deseja registrar: ");
            string tituloAlbum = Console.ReadLine()!;

            Banda banda = bandasRegistradas[nomeDaBanda];
            banda.AdicionarAlbum(new Album(tituloAlbum));

            Console.WriteLine($"O albúm {tituloAlbum} de {nomeDaBanda} foi registrada com sucesso!");
            Thread.Sleep(4000);
            Console.Clear();
        }
        else
        {
            Console.WriteLine($"\nA banda {nomeDaBanda} não foi encontrada");
            Console.WriteLine("Digite qualquer tecla para voltar ao menu principal");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
