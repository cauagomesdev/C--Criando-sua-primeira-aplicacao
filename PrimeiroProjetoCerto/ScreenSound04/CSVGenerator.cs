using System;
using ReportGenerator;

namespace ScreenSound04;

internal class CSVGenerator : ReporterGenerator
{
    public CSVGenerator(List<Dictionary<string, string>> input)
        : base(input) { }

    public string GenerateCSV(string fileName)
    {
        GenerateReport();
        File.Move("report.csv", fileName);
        return Path.GetFullPath(fileName);
    }
}
