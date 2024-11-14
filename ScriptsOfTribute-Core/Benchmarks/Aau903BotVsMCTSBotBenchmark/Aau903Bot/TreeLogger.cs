using System.Text;

namespace Aau903Bot;

public class CsvLogger
{
    private readonly string _filePath;
    private bool _headersWritten;
    private const string ResultsFolder = "results";

    public CsvLogger(string fileName)
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

public class TreeLogger
{
    private CsvLogger logger;

    public TreeLogger(string? filePath = null)
    {
        logger = new CsvLogger(filePath == null ? "tree.csv" : filePath);
    }

    public void LogTree(Node node)
    {
        var childrenHashes = new List<int>();
        var nodeHash = Utility.PreciseHash(node.GameState);
        foreach (var childNode in node.ChildNodes)
        {
            var childHash = Utility.PreciseHash(childNode.GameState);
            if (childHash != nodeHash)
            {
                childrenHashes.Add(childHash);
            }
        }

        var data = new Dictionary<string, object>
        {
            {"Player", node.GameState.CurrentPlayer.PlayerID},
            {"Node", nodeHash},
            {"Type", node is ChanceNode ? "ChanceNode" : "Node"},
            {"Applied Move", node.AppliedMove},
            {"Children", string.Join(",", childrenHashes)},
            {"Coins", node.GameState.CurrentPlayer.Coins},
            {"Power", node.GameState.CurrentPlayer.Power},
            {"Prestige", node.GameState.CurrentPlayer.Prestige},
            {"Visit", node.VisitCount},
            {"Score", node.TotalScore}
        };
        logger.Log(data);

        foreach (var child in node.ChildNodes)
        {
            LogTree(child);
        }
    }
}
