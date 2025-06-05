using System.Text.Json;

namespace LetWeCook.Infrastructure.Persistence;

public class SeedingConfigHelper
{
    private static readonly string ConfigFileName = "seeding-configs.json";

    public static string FindConfigPath()
    {
        string? current = AppContext.BaseDirectory;

        while (current != null)
        {
            string potential = Path.Combine(current, ConfigFileName);
            if (File.Exists(potential))
                return potential;

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException($"Could not find {ConfigFileName} in any parent directory of {AppContext.BaseDirectory}");
    }

    public static SeedingConfig LoadConfig()
    {
        string path = FindConfigPath();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SeedingConfig>(json)!;
    }

    public static void SaveConfig(SeedingConfig config)
    {
        string path = FindConfigPath();

        File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }
}