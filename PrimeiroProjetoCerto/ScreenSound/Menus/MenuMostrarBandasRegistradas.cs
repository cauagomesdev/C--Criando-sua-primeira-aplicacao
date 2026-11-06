using ScreenSound.Modelos;

namespace ScreenSound.Menus;

internal class MenuMostrarBandasRegistradas : Menu
{
    public override void Executar(Dictionary<string, Banda> bandasRegistradas)
    {
        base.Executar(bandasRegistradas);
        ExibirTituloDaOpcao("Exibindo todas as bandas registradas");

        foreach (string banda in bandasRegistradas.Keys)
        {
            Console.WriteLine($"Banda: {banda}");
        }

        Console.WriteLine("\nDigite qualquer tecla para voltar ao menu principal");
        Console.ReadKey();
        Console.Clear();
    }
}
