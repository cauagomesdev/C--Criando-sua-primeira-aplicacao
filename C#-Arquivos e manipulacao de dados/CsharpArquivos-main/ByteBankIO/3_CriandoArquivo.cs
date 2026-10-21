using System.Text;
using ByteBankIO;


partial class Program
{
    static void CriarArquivo()
    {
        var caminhoNovoArquivo = "contasExportadas.csv";
        
        using(var fluxoDeArquivo = new FileStream(caminhoNovoArquivo, FileMode.Create))
        {
            var contaComoString = "456, 7895, 4780.40, Gustavo Santos";

            var encondig = Encoding.UTF8;

            var bytes = encondig.GetBytes(contaComoString);

            fluxoDeArquivo.Write(bytes, 0, bytes.Length);
        }
    }

    static void CriarArquivoComWriter()
    {
        var caminhoNovoArquivo = "contasExportadas.csv";

        using(var fluxoDeArquivo = new FileStream(caminhoNovoArquivo, FileMode.Create))
        using(var escritor = new StreamWriter(fluxoDeArquivo))
        {
            escritor.Write("456, 65465, 456.0, Pedro");
        }
    }

    static void TestaEscrita()
    {
        var caminhoNovoArquivo = "teste.txt";

        using(var fluxoDeArquivo = new FileStream(caminhoNovoArquivo, FileMode.Create))
        using(var escritor = new StreamWriter(fluxoDeArquivo))
        {
            for(int i = 0; i < 100000; i++)
            {
            escritor.WriteLine($"Linha {i}");
            escritor.Flush(); // despeja o buffer para o Stream
            System.Console.WriteLine($"Linha {i} foi escrita no arquivo. Tecle enter...");
            System.Console.ReadLine();
            }
        }
    }
}
