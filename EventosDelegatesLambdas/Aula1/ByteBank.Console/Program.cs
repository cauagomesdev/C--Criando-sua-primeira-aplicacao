internal class Program
{
    private static void Main(string[] args)
    {

        new Logo().MostrarBanner();

        while (true)
        {
            MostrarMenu();

            if (int.TryParse(Console.ReadLine(), out int escolha))
            {
                ExecutarEscolha(escolha);
            }
            else
            {
                Console.WriteLine("Opção inválida. Tente novamente.");
            }
        }

    }

    static void MostrarMenu()
    {
        Console.WriteLine("\nEscolha uma opção:");
        Console.WriteLine();
        Console.WriteLine("1. Saldo");
        Console.WriteLine("2. Depositar valores");
        Console.WriteLine("3. Sacar valores");
        Console.WriteLine("4. Extrato");
        Console.WriteLine();
        Console.Write("Digite o número da opção desejada: ");
    }

    static void ExecutarEscolha(int escolha)
    {
        switch (escolha)
        {
            case 1:
                Saldo();
                break;
            case 2:
                Depositar();
                break;
            case 3:
                Sacar();
                break;
            case 4:
                Extrato();
                break;

            default:
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }

    static CaixaEletronico caixaEletronico = new CaixaEletronico();

    private static void Saldo()
    {
        caixaEletronico.Saldo();
    }

    private static void Depositar()
    {
        caixaEletronico.Depositar(100);
        caixaEletronico.Depositar(40);
        caixaEletronico.Depositar(25);
    }

    private static void Sacar()
    {
        caixaEletronico.Sacar(50);
        caixaEletronico.Sacar(20);
    }

    private static void Extrato()
    {
        caixaEletronico.Extrato();
    }
}