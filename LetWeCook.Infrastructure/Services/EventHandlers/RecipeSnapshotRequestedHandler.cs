using System.Net.Http.Headers;
using System.Text.Json;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.EventHandlers;

public class RecipeSnapshotRequestedHandler : INonBlockingDomainEventHandler<RecipeSnapshotRequestedEvent>
{
    private readonly IRecipeSnapshotService _recipeSnapshotService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecipeSnapshotRequestedHandler> _logger;

    public RecipeSnapshotRequestedHandler(
        IRecipeSnapshotService recipeSnapshotService,
        IHttpClientFactory httpClientFactory,
        ILogger<RecipeSnapshotRequestedHandler> logger)
    {
        _recipeSnapshotService = recipeSnapshotService;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task HandleAsync(RecipeSnapshotRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📤 Recipe snapshot request triggered");

        var recipeSnapshots = await _recipeSnapshotService.GetRecipeSnapshotsAsync(cancellationToken);
        _logger.LogInformation("✅ Retrieved {Count} recipe snapshots", recipeSnapshots.Count());

        var json = JsonSerializer.Serialize(recipeSnapshots, new JsonSerializerOptions { WriteIndented = true });
        var tempFilePath = Path.GetTempFileName();
        var jsonFilePath = Path.ChangeExtension(tempFilePath, ".json");
        await File.WriteAllTextAsync(jsonFilePath, json, cancellationToken);
        var fileName = "recipes-snapshot.json";

        try
        {
            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(jsonFilePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            form.Add(fileContent, "file", fileName);

            _logger.LogInformation("📡 Uploading {FileName} to Python server...", fileName);
            var response = await _httpClient.PostAsync("http://localhost:8000/upload", form, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Upload successful");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("❌ Upload failed with status {Status}: {Error}", response.StatusCode, error);
                throw new Exception($"Upload failed: {response.StatusCode} - {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🚨 Exception during recipe snapshot upload");
            throw;
        }
        finally
        {
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
                _logger.LogInformation("🧹 Temp file deleted: {Path}", jsonFilePath);
            }
        }
    }
}
