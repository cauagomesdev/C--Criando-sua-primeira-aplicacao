using System;
using System.Net.Http.Headers;

namespace ReportGenerator.Util;

internal interface IDataFormatter
{
    string FormatData(string input);
}
