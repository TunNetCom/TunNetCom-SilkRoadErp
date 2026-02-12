using System.Text.Json;

namespace TunNetCom.SilkRoadErp.Sales.WebApp;

internal static class DebugLog
{
    private const string LogPath = @"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log";

    public static void Write(string location, string message, object? data = null, string? hypothesisId = null)
    {
        try
        {
            var entry = new Dictionary<string, object?>
            {
                ["id"] = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ["location"] = location,
                ["message"] = message,
                ["data"] = data
            };
            if (!string.IsNullOrEmpty(hypothesisId))
                entry["hypothesisId"] = hypothesisId;
            var line = JsonSerializer.Serialize(entry) + "\n";
            File.AppendAllText(LogPath, line);
        }
        catch { /* ignore */ }
    }
}
