using System.Text;
using ByteBankIO;

partial class Program
{
    static void LidandoComFileStreamDiretamente()
    {
        var enderecoDoArquivo = "contas.txt";

        using (var fluxoDeArquivo = new FileStream(enderecoDoArquivo, FileMode.Open))
        {
            var numeroDeBytesLidos = -1;



            var buffer = new byte[1024]; // 1KB


            while (numeroDeBytesLidos != 0)
            {
                numeroDeBytesLidos = fluxoDeArquivo.Read(buffer, 0, 1024);
                Console.WriteLine($"\nBytes lidos: {numeroDeBytesLidos}");
                EscreverBuffer(buffer, numeroDeBytesLidos);
            }

            // public override int Read(byte[] array, int offset, int count)
            fluxoDeArquivo.Close();
        }
    }

    static void EscreverBuffer(byte[] buffer, int bytesLidos)
    {

        var utf8 = new UTF8Encoding();

        var texto = utf8.GetString(buffer, 0, bytesLidos);

        //public virtual string GetString(byte[] bytes, int index, int count)
        Console.Write(texto);

        // foreach (var meuByte in buffer)
        // {
        //     Console.Write(meuByte);
        //     Console.Write(" ");
        // }
    }
}