object chave = new object();
Task<string> conteudoTask;
lock (chave)
{
    conteudoTask = Task.Run(() => File.ReadAllTextAsync("voos.txt"));
}

async Task LerArquivoAsync(CancellationToken token)
{
    try
    {
        await Task.Delay(new Random().Next(300, 8000));
        token.ThrowIfCancellationRequested();
        System.Console.WriteLine($"Conteúdo do arquivo: \n{conteudoTask.Result}");
    }
    catch (OperationCanceledException ex)
    {
        System.Console.WriteLine($"Tarefa cancelada: {ex.Message}");
    }
    catch (AggregateException ex)
    {
        System.Console.WriteLine($"Erro ao ler o arquivo: {ex.InnerException?.Message}");
    }
}

async Task ExibirRelatorioAsync(CancellationToken token)
{
    try
    {
        System.Console.WriteLine("Exibindo relatório de compra de passagens.");
        await Task.Delay(new Random().Next(300, 8000));
        token.ThrowIfCancellationRequested();
    }
    catch (OperationCanceledException ex)
    {
        System.Console.WriteLine($"Tarefa cancelada: {ex.Message}");
    }
}

CancellationTokenSource tokenDeCancelamento = new CancellationTokenSource();
Task tarefa = Task.WhenAll(
    LerArquivoAsync(tokenDeCancelamento.Token),
    ExibirRelatorioAsync(tokenDeCancelamento.Token)
);

await Task.Delay(1000).ContinueWith(_ => tokenDeCancelamento.Cancel());

System.Console.WriteLine("Outras operações.");
Console.ReadKey();
