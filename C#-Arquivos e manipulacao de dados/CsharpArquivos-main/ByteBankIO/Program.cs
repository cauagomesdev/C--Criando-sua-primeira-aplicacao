using System.Text;
using ByteBankIO;

partial class Program
{
    static void Main(string[] args)
    {
        var linhas = File.ReadAllLines("contas.txt");
        System.Console.WriteLine(linhas.Length);

        // foreach (var linha in linhas)
        // {
        //     System.Console.WriteLine(linha);
        // }

        var bytesArquivo = File.ReadAllBytes("contas.txt");
        System.Console.WriteLine($"Arquivo contas.txt possui {bytesArquivo.Length} bytes");
        File.WriteAllText("escrevendoComAClasseFile.txt", "Testando File.WriteAllText");
        // EscritaBinaria();
        // LeituraBinaria();
        // UsarStreamDeEntrada();
        System.Console.WriteLine("Aplicação finalizada... ");
        Console.ReadLine();

    }

}
