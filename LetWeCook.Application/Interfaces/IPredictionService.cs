namespace LetWeCook.Application.Interfaces;

public interface IPredictionService
{
    Task<List<Guid>> GetRecipeSuggestionsAsync(
        List<string> userPreferences,
        int count = 5,
        CancellationToken cancellationToken = default);
}