using System.Text;

namespace CsvLoggerLibrary;

public class CsvBenchmarkLogger
{
    private readonly string _filePath;
    private bool _headersWritten;
    private const string ResultsFolder = "results";

    public CsvBenchmarkLogger(string fileName = "results.csv")
    {
        Directory.CreateDirectory(ResultsFolder);
        _filePath = Path.Combine(ResultsFolder, fileName);

        // Check if file exists and contains any content
        if (File.Exists(_filePath) && new FileInfo(_filePath).Length > 0)
        {
            _headersWritten = true; // Assume headers are already written
        }
        else
        {
            File.Create(_filePath).Dispose();
        }
    }

    public void Log(Dictionary<string, object> benchmarkData)
    {
        if (!_headersWritten)
        {
            WriteHeaders(benchmarkData.Keys);
            _headersWritten = true;
        }

        WriteRow(benchmarkData.Values);
    }

    private void WriteHeaders(IEnumerable<string> headers)
    {
        var headerLine = string.Join(",", headers);
        File.AppendAllText(_filePath, headerLine + Environment.NewLine);
    }

    private void WriteRow(IEnumerable<object> values)
    {
        var row = new StringBuilder();
        foreach (var value in values)
        {
            row.Append(value?.ToString()?.Replace(",", ";") ?? "");
            row.Append(",");
        }
        row.Length--;
        File.AppendAllText(_filePath, row + Environment.NewLine);
    }
}
