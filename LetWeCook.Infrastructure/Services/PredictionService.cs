using System.Net.Http.Json;
using LetWeCook.Application.DTOs.Suggestions;
using LetWeCook.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LetWeCook.Infrastructure.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly HttpClient _httpClient;
        private string _apiUrl = string.Empty;
        public PredictionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration.GetSection("PredictionService:ApiUrl").Value
                ?? throw new InvalidOperationException("PredictionService:ApiUrl is not configured in appsettings.");
        }

        public async Task<List<Guid>> GetRecipeSuggestionsAsync(List<string> userPreferences, int count = 5, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Calling GetRecipeSuggestionsAsync with {count} and {userPreferences.Count} preferences");

            var prefRequest = MapUserPreferencesToRequest(userPreferences);

            var requestUri = $"{_apiUrl}/predict?n={count}"; // full URL

            try
            {
                var response = await _httpClient.PostAsJsonAsync(requestUri, prefRequest, cancellationToken);

                Console.WriteLine($"Prediction API responded with status code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var guidsAsStrings = await response.Content.ReadFromJsonAsync<List<string>>(cancellationToken: cancellationToken);

                if (guidsAsStrings == null)
                {
                    Console.WriteLine("Prediction API returned null or empty list.");
                    return new List<Guid>(); // fallback: empty list
                }

                var result = new List<Guid>();
                foreach (var idStr in guidsAsStrings)
                {
                    if (Guid.TryParse(idStr, out var guid))
                    {
                        result.Add(guid);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid GUID format from API: {idStr}");
                    }
                }

                Console.WriteLine($"Received {result.Count} recipe IDs from prediction service");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception calling prediction API: {ex.Message}");
                return new List<Guid>(); // fallback: empty list to trigger random suggestion fallback
            }
        }



        public PreferenceRequest MapUserPreferencesToRequest(List<string> userPreferences)
        {
            var prefRequest = new PreferenceRequest();
            var userPrefSet = new HashSet<string>(userPreferences, StringComparer.OrdinalIgnoreCase);

            if (userPrefSet.Contains("Vegetarian")) prefRequest.Pref_Vegetarian = true;
            if (userPrefSet.Contains("Vegan")) prefRequest.Pref_Vegan = true;
            if (userPrefSet.Contains("GlutenFree")) prefRequest.Pref_GlutenFree = true;
            if (userPrefSet.Contains("Pescatarian")) prefRequest.Pref_Pescatarian = true;
            if (userPrefSet.Contains("LowCalorie")) prefRequest.Pref_LowCalorie = true;
            if (userPrefSet.Contains("HighProtein")) prefRequest.Pref_HighProtein = true;
            if (userPrefSet.Contains("LowCarb")) prefRequest.Pref_LowCarb = true;
            if (userPrefSet.Contains("LowFat")) prefRequest.Pref_LowFat = true;
            if (userPrefSet.Contains("LowSugar")) prefRequest.Pref_LowSugar = true;
            if (userPrefSet.Contains("HighFiber")) prefRequest.Pref_HighFiber = true;
            if (userPrefSet.Contains("LowSodium")) prefRequest.Pref_LowSodium = true;

            return prefRequest;
        }
    }
}
