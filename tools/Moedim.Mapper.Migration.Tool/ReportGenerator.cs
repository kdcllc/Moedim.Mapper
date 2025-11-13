namespace Moedim.Mapper.Migration.Tool;

public static class ReportGenerator
{
    public static async Task GenerateAsync(string path, string format, string? outputFile)
    {
        Console.WriteLine($"📊 Generating migration report ({format})...");

        // Use analyzer to get data
        var tempFile = Path.GetTempFileName();
        await MigrationAnalyzer.AnalyzeAsync(path, false, tempFile);

        var analysisText = await File.ReadAllTextAsync(tempFile);
        File.Delete(tempFile);

        string report = format.ToLower() switch
        {
            "json" => GenerateJsonReport(analysisText),
            "html" => GenerateHtmlReport(analysisText),
            _ => analysisText
        };

        if (outputFile != null)
        {
            await File.WriteAllTextAsync(outputFile, report);
            Console.WriteLine($"✓ Report saved to: {outputFile}");
        }
        else
        {
            Console.WriteLine(report);
        }
    }

    private static string GenerateJsonReport(string markdown)
    {
        return $$"""
        {
          "tool": "Moedim.Mapper Migration Tool",
          "timestamp": "{{DateTime.UtcNow:O}}",
          "analysis": "{{markdown.Replace("\"", "\\\"").Replace("\n", "\\n")}}"
        }
        """;
    }

    private static string GenerateHtmlReport(string markdown)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
    <title>AutoMapper to Moedim.Mapper Migration Report</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; background: #f5f5f5; }}
        .container {{ max-width: 900px; margin: auto; background: white; padding: 40px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h1 {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }}
        h2 {{ color: #34495e; margin-top: 30px; }}
        pre {{ background: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .timestamp {{ color: #7f8c8d; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>🔄 AutoMapper to Moedim.Mapper Migration Report</h1>
        <p class=""timestamp"">Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
        <pre>{System.Net.WebUtility.HtmlEncode(markdown)}</pre>
    </div>
</body>
</html>";
    }
}
