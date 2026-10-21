using System.Text;
using ReportGenerator.Util;

namespace ReportGenerator.Generators;

public abstract class ReporterGeneratorBase : IReportGenerator
{
    public List<Dictionary<string, string>> Input { get; set; }
    public string Title { get; set; }
    public string HeadLine { get; set; }
    public string FooterLine { get; set; }

    public ReporterGeneratorBase(List<Dictionary<string, string>> input)
    {
        Input = input;
    }

    public abstract string GenerateReport();
}
