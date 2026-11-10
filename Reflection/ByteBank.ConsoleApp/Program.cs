using System.Reflection;
using ByteBank.Common;

MostrarBanner();

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

static void MostrarBanner()
{
    Console.WriteLine(
        @"


    ____        __       ____              __      
   / __ )__  __/ /____  / __ )____ _____  / /__    
  / __  / / / / __/ _ \/ __  / __ `/ __ \/ //_/    
 / /_/ / /_/ / /_/  __/ /_/ / /_/ / / / / ,<       
/_____/\__, /\__/\___/_____/\__,_/_/ /_/_/|_|      
      /____/                                       
                                
        "
    );
}

static void MostrarMenu()
{
    Console.WriteLine("\nEscolha uma opção:");
    Console.WriteLine();
    Console.WriteLine("1. Ler arquivo de boletos");
    Console.WriteLine();
    Console.WriteLine("2. Gravar arquivo com boletos agrupados por cedente");
    Console.WriteLine();
    Console.WriteLine("3. Executar plugins");
    Console.WriteLine();
    Console.Write("Digite o número da opção desejada: ");
}

static void ExecutarEscolha(int escolha)
{
    switch (escolha)
    {
        case 1:
            LerArquivoBoletos();
            break;
        case 2:
            GravarGrupoBoletos();
            break;
        case 3:
            ExecutarPlugins();
            break;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}

static void LerArquivoBoletos()
{
    Console.WriteLine("Lendo arquivo de boletos...");

    var leitorDeBoleto = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeBoleto.LerBoletos("Boletos.csv");

    foreach (var boleto in boletos)
    {
        Console.WriteLine(
            $"Cedente: {boleto.CedenteNome}, Valor: {boleto.Valor:#0.00}, Vencimento: {boleto.DataVencimento}"
        );
    }
}

static void GravarGrupoBoletos()
{
    System.Console.WriteLine("Gravando arquivo de boletos...");

    var leitorDeBoleto = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeBoleto.LerBoletos("Boletos.csv");

    // RelatorioDeBoleto gravadorDeCSV = new RelatorioDeBoleto("BoletosPorCedente.csv");
    // gravadorDeCSV.Processar(boletos);

    var nomeParametroConstrutor = "nomeArquivoSaida";
    var parametroConstrutor = "BoletosPorCedente.csv";
    var nomeMetodo = "Processar";
    var parametroMetodo = boletos;

    ProcessarDinamicamente(
        nomeParametroConstrutor,
        parametroConstrutor,
        nomeMetodo,
        parametroMetodo
    );
}

static void ProcessarDinamicamente(
    string nomeParametroConstrutor,
    string parametroConstrutor,
    string nomeMetodo,
    List<Boleto> parametroMetodo
)
{
    var tipoClasserelatorio = typeof(RelatorioDeBoleto);
    var construtores = tipoClasserelatorio.GetConstructors();

    // foreach (var construtor in construtores)
    // {
    //     System.Console.WriteLine($"Construtor: {construtor.Name}");
    //     System.Console.WriteLine($"número de parâmetros: {construtor.GetParameters().Length}");
    // }

    var construtor = construtores.Single(c =>
        c.GetParameters().Length == 1
        && c.GetParameters().Any(p => p.Name == nomeParametroConstrutor)
    );
    var instanciaClasse = construtor.Invoke(new object[] { parametroConstrutor });

    var metodoProcessar = tipoClasserelatorio.GetMethod(nomeMetodo);
    metodoProcessar.Invoke(instanciaClasse, new object[] { parametroMetodo });
}

static void ExecutarPlugins()
{
    //Ler boletos a partir do arquivo CSV
    var leitorDeCSV = new LeitorDeBoleto();
    List<Boleto> boletos = leitorDeCSV.LerBoletos("Boletos.csv");

    //Obter classes de plugin
    List<Type> classesDePlugin = ObterClassesDePlugin<IRelatorio<Boleto>>();

    foreach (var classe in classesDePlugin)
    {
        // Criar uma instância do plugin
        var plugin = Activator.CreateInstance(classe, new object[] { "BoletosPorCedente.csv" });

        // Chamar o método Processar usando Reflection
        MethodInfo metodoSalvar = classe.GetMethod("Processar");
        metodoSalvar.Invoke(plugin, new object[] { boletos });
    }
}

static List<Type> ObterClassesDePlugin<T>()
{
    var tiposEncontrados = new List<Type>();

    //Pegar o assembly que está em execução
    Assembly assemblyEmExecucao = Assembly.GetExecutingAssembly();

    //Pegar o assembly onde um tipo é declarado
    Assembly assemblyDosPlugins = typeof(IRelatorio<Boleto>).Assembly;

    //Descobre todos os tipos do assembly
    var tipos = assemblyDosPlugins.GetTypes();

    foreach (var tipo in tipos)
    {
        System.Console.WriteLine($"Nome: {tipo.Name}");
        System.Console.WriteLine($"Nome completo: {tipo.FullName}");
        System.Console.WriteLine($"É classe: {tipo.IsClass}");
        System.Console.WriteLine($"É interface: {tipo.IsInterface}");
        System.Console.WriteLine($"É abstrata: {tipo.IsAbstract}");

        System.Console.WriteLine("Interfaces implementadas:");
        foreach (var interfaceImplementada in tipo.GetInterfaces())
        {
            System.Console.WriteLine($"  - {interfaceImplementada.Name}");
        }
        System.Console.WriteLine();
    }

    //Encontrar tipos que implementam a interface T

    var tiposImplementandoT = tipos.Where(t =>
        typeof(T).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract
    );

    tiposEncontrados.AddRange(tiposImplementandoT);

    return tiposEncontrados;
}
