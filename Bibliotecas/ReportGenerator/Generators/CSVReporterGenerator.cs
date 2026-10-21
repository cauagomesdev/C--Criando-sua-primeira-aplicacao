using System.Text;
using ReportGenerator.Util;

namespace ReportGenerator.Generators;

public class CSVReporterGenerator : ReporterGeneratorBase
{
    public CSVReporterGenerator(List<Dictionary<string, string>> input)
        : base(input) { }

    public override string GenerateReport()
    {
        if (Input == null || Input.Count == 0)
            throw new Exception("Input de dados inválido");

        StringBuilder report = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(Title))
        {
            report.AppendLine(Title);
            report.AppendLine("");
        }

        if (!string.IsNullOrWhiteSpace(HeadLine))
        {
            report.AppendLine(HeadLine);
            report.AppendLine("");
        }

        string header = string.Join(';', Input.First().Keys);

        report.AppendLine(header);

        foreach (var record in Input)
        {
            string line = string.Join(';', record.Values);
            report.AppendLine(line);
        }

        if (!string.IsNullOrWhiteSpace(FooterLine))
        {
            report.AppendLine("");
            report.AppendLine(FooterLine);
        }

        DateFormatter dateFormatter = new DateFormatter();

        report.AppendLine(dateFormatter.FormatData(DateTime.Now.ToString()));

        File.WriteAllText("report.csv", report.ToString(), Encoding.UTF8);

        return Path.GetFullPath("report.csv");
    }
}
