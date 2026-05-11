using System.Text.Json;

namespace Buggernaut.Generator;

public static class MetaReader
{
    private record Meta(
        string Title       = "",
        string Description = "",
        string Hint        = "",
        string Explanation = "");

    public static void ShowHint(string className)    => ShowField(className, "hint");
    public static void ShowExplanation(string className) => ShowField(className, "explain");

    private static void ShowField(string className, string command)
    {
        var meta = ReadMeta(className);
        if (meta is null) return;

        Printer.H2(meta.Title);

        if (command == "hint")
        {
            Printer.Info("Ledtråd", indent: 0);
            Printer.Line(meta.Hint, indent: 1);
        }
        else
        {
            Printer.Info("Förklaring", indent: 0);
            Printer.Line(meta.Explanation, indent: 1);
        }
        Printer.Blank();
    }

    private static Meta? ReadMeta(string className)
    {
        var metaPath = Path.Combine(
            SolutionRootFinder.Find(), "solutions", $"{className}.meta.json");

        if (!File.Exists(metaPath))
        {
            Printer.Error($"Ingen meta-fil hittades för \"{className}\".");
            Printer.Dim($"Tänk på att generera övningen med: dotnet run -- generate", indent: 1);
            return null;
        }

        var json = File.ReadAllText(metaPath);
        return JsonSerializer.Deserialize<Meta>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}