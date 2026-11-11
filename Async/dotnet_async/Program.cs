Task<string> conteudoTask = Task.Run(() => File.ReadAllTextAsync("voos.txt"));

async void LerArquivoAsync()
{
    try
    {
        await Task.Delay(new Random().Next(300, 8000));
        System.Console.WriteLine($"Conteúdo do arquivo: \n{conteudoTask.Result}");
    }
    catch (AggregateException ex)
    {
        System.Console.WriteLine($"Erro ao ler o arquivo: {ex.InnerException?.Message}");
    }
}

async void ExibirRelatorioAsync()
{
    System.Console.WriteLine("Exibindo relatório de compra de passagens.");
    await Task.Delay(new Random().Next(300, 8000));
}

//Task task1 = Task.Run(() => LerArquivo());
//Task task2 = Task.Run(() => ExibirRelatorio());

LerArquivoAsync();
ExibirRelatorioAsync();

System.Console.WriteLine("Outras operações.");
Console.ReadKey();
