using System.Text.Json;
using LetWeCook.Application.DTOs.RecipeSnapshots;

namespace LetWeCook.Infrastructure.Persistence;

public static class TrainingDataConfigHelper
{
    private static readonly string ConfigFileName = "training-data-config.json";
    private static readonly TimeSpan UploadInterval = TimeSpan.FromHours(6);

    public static string FindConfigPath()
    {
        string? current = AppContext.BaseDirectory;

        while (current != null)
        {
            string potential = Path.Combine(current, ConfigFileName);
            if (File.Exists(potential))
            {
                Console.WriteLine($"[TrainingDataConfigHelper] Found config at: {potential}");
                return potential;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException($"Could not find {ConfigFileName} in any parent directory of {AppContext.BaseDirectory}");
    }


    public static TrainingDataConfig LoadConfig()
    {
        string path = FindConfigPath();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TrainingDataConfig>(json)
               ?? new TrainingDataConfig(); // fallback if null
    }

    public static void SaveConfig(TrainingDataConfig config)
    {
        string path = FindConfigPath();

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    public static void UpdateCsvTimestamp(DateTime timestamp)
    {
        var config = LoadConfig();
        config.TrainingDataCsvLastSent = timestamp;
        SaveConfig(config);
    }

    public static void UpdateSnapshotTimestamp(DateTime timestamp)
    {
        var config = LoadConfig();
        config.RecipesSnapshotLastSent = timestamp;
        SaveConfig(config);
    }

    public static bool ShouldUploadCsv()
    {
        var config = LoadConfig();
        if (config.TrainingDataCsvLastSent == null)
            return true;

        return DateTime.UtcNow - config.TrainingDataCsvLastSent >= UploadInterval;
    }

    public static bool ShouldUploadSnapshot()
    {
        var config = LoadConfig();
        if (config.RecipesSnapshotLastSent == null)
            return true;

        return DateTime.UtcNow - config.RecipesSnapshotLastSent >= UploadInterval;
    }

    public static void UpdateLastUploadTime()
    {
        var config = LoadConfig();
        var now = DateTime.UtcNow;
        config.TrainingDataCsvLastSent = now;
        SaveConfig(config);
    }

    public static void UpdateLastSnapshotUploadTime()
    {
        var config = LoadConfig();
        var now = DateTime.UtcNow;
        config.RecipesSnapshotLastSent = now;
        SaveConfig(config);
    }
}
