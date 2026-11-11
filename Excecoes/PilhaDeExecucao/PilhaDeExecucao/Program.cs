public class PilhaDeExecucao
{
    public static void Metodo1()
    {
        Console.WriteLine("[Inicio] - Metodo1");

        Metodo2();
       
        Console.WriteLine("[Fim] - Metodo1");
    }

    public static void Metodo2()
    {
        Console.WriteLine("[Inicio] - Metodo2");

        Usuario usuario = null;

        Console.WriteLine(usuario.Nome);

        Console.WriteLine("[Fim] - Metodo2");
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("[Inicio] - Main");
        try
        {
            Metodo1();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Falha ao chamar método 1" + ex.Message);  
        }
        

        Console.WriteLine("[Fim] - Main");
    }
}

public class Usuario
{
    public string Nome { get; set; }

    public Usuario(string nome)
    {
        Nome = nome;
    }
}
