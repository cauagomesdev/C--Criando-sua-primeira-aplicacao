using ScreenSound.Modelos;

namespace ScreenSound.Menus;

internal class MenuExibirDetalhes : Menu
{

    public override void Executar(Dictionary<string, Banda> bandasRegistradas)
    {
        base.Executar(bandasRegistradas);
        ExibirTituloDaOpcao("Exibir detalhes da banda");
        Console.Write("Digite o nome da banda que você deseja ver as informações: ");
        string nomeDaBanda = Console.ReadLine()!;

        if (bandasRegistradas.ContainsKey(nomeDaBanda))
        {
            Banda banda = bandasRegistradas[nomeDaBanda];
            Console.WriteLine($"\nA média da banda {nomeDaBanda} é {banda.Media}");

            Console.WriteLine("\nDiscografia: ");
            foreach (Album album in banda.Albuns)
            {
                System.Console.WriteLine($"\nO álbum {album.Nome} possui uma média de {album.Media}");
            }

            Console.WriteLine("\nDigite qualquer tecla para voltar ao menu principal");
            Console.ReadKey();
            Console.Clear();
        }
        else
        {
            Console.WriteLine($"\nA banda {nomeDaBanda} não foi encontrada");
            Console.WriteLine("\nDigite qualquer tecla para voltar ao menu principal");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
