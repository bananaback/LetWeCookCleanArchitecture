using LetWeCook.Application.Dtos.Recipe;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeSuggestionService
{
    Task<List<RecipeDto>> GetRandomSuggestionsAsync(int count = 5, CancellationToken cancellationToken = default);
    Task ProcessFeedbackAsync(Guid recipeId, bool isLike, Guid siteUserId, CancellationToken cancellationToken = default);
    Task<List<RecipeDto>> GetUserSpecificSuggestionsAsync(Guid siteUserId, int count = 5, CancellationToken cancellationToken = default);
}