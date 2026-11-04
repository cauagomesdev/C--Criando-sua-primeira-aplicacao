using ScreenSound.Menus;
using ScreenSound.Modelos;

Banda natiruts = new Banda("Natiruts");
natiruts.AdicionarNota(new Avaliacao(10));
natiruts.AdicionarNota(new Avaliacao(8));
natiruts.AdicionarNota(new Avaliacao(6));

Banda beatles = new Banda("Beatles");


Dictionary<string, Banda> bandasRegistradas = new();
bandasRegistradas.Add(natiruts.Nome, natiruts);
bandasRegistradas.Add(beatles.Nome, beatles);

Dictionary<int, Menu> opcoes = new();
opcoes.Add(1, new MenuRegistrarBanda());
opcoes.Add(2, new MenuRegistrarAlbum());
opcoes.Add(3, new MenuMostrarBandasRegistradas());
opcoes.Add(4, new MenuAvaliarBanda());
opcoes.Add(5, new MenuAvaliarAlbum());
opcoes.Add(6, new MenuExibirDetalhes());
opcoes.Add(-1, new MenuSair());

void ExibirLogo()
{
  Console.WriteLine(@"
    SCREEN SOUND
    ");
  System.Console.WriteLine("Boas Vindas ao Screen Sound 2.0");
}

void ExibirOpcoesDoMenu()
{
  ExibirLogo();
  Console.WriteLine("\nDigite 1 para registrar uma banda");
  Console.WriteLine("Digite 2 para registrar um álbum de uma banda");
  Console.WriteLine("Digite 3 para mostrar todas as bandas");
  Console.WriteLine("Digite 4 para avaliar uma banda");
  Console.WriteLine("Digite 5 para avaliar um álbum de uma banda");
  Console.WriteLine("Digite 6 para exibir as informações de uma banda");
  Console.WriteLine("Digite -1 para sair");

  Console.Write("\nDigite a sua opção: ");
  string opcaoEscolhida = Console.ReadLine()!;
  int opcaoEscolhidaNumerica = int.Parse(opcaoEscolhida);

  if (opcoes.ContainsKey(opcaoEscolhidaNumerica))
  {
    Menu menuASerExibido = opcoes[opcaoEscolhidaNumerica];
    menuASerExibido.Executar(bandasRegistradas);
    if (opcaoEscolhidaNumerica != -1)  ExibirOpcoesDoMenu();
  }
  else
  {
    Console.WriteLine("Opção inválida");
  }
  
}

ExibirOpcoesDoMenu();

