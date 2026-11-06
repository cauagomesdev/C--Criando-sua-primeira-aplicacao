Console.WriteLine("Bem-vindo ao jogo de advinhar");
Console.WriteLine("Tente advinhar um número de 1 a 100");

Random numAleatorio = new Random();
int numCerto = numAleatorio.Next(1, 101);

do
{
    Console.WriteLine(numCerto);
    Console.Write("Digite o número: ");

    int numTentado = int.Parse(Console.ReadLine()!);

    if (numTentado > numCerto)
    {
        Console.WriteLine($"o número {numTentado} é maior que o número certo");
    } else if (numTentado < numCerto)
    {
        Console.WriteLine($"o número {numTentado} é menor que o número certo");
    }else
    {
        Console.WriteLine("Parabéns, você acertou o número de 0 a 100");  
        break; 
    }
} while (true);

