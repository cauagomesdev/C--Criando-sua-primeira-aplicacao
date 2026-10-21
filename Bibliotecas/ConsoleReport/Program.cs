using ReportGenerator;
using ReportGenerator.Generators;

List<Dictionary<string, string>> data = new()
{
    new Dictionary<string, string>
    {
        { "Nome", "Alice" },
        { "Idade", "30" },
        { "Cidade", "São Paulo" },
    },
    new Dictionary<string, string>
    {
        { "Nome", "Roberto" },
        { "Idade", "25" },
        { "Cidade", "Salvador" },
    },
    new Dictionary<string, string>
    {
        { "Nome", "Carlos" },
        { "Idade", "35" },
        { "Cidade", "Rio de Janeiro" },
    },
};

PDFReporterGenerator reporterGenerator = new PDFReporterGenerator(data);

reporterGenerator.Title = "Relatório de pessoas";
reporterGenerator.HeadLine = "Lista de pessoas cadastradas no sistema";
reporterGenerator.FooterLine = "Total de pessoas: " + data.Count;

string caminhoArquivo = reporterGenerator.GenerateReport();

System.Console.WriteLine("O arquivo foi salvo em: " + caminhoArquivo);
