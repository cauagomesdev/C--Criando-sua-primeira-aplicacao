using System;

namespace ReportGenerator.Util;

internal class DateFormatter : IDataFormatter
{
    public string FormatData(string input)
    {
        if (DateTime.TryParse(input, out DateTime date))
        {
            return date.ToString("dd/MM/yyyy");
        }
        return input;
    }
}
