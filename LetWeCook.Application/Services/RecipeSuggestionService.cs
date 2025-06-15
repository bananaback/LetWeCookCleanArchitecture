using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Services;

public class RecipeSuggestionService : IRecipeSuggestionService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserInteractionRepository _userInteractionRepository;
    private readonly IPredictionService _predictionService;
    private readonly ISuggestionFeedbackRepository _suggestionFeedbackRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RecipeSuggestionService(
        IRecipeRepository recipeRepository,
        ISuggestionFeedbackRepository suggestionFeedbackRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserInteractionRepository userInteractionRepository,
        IPredictionService predictionService)
    {
        _recipeRepository = recipeRepository;
        _suggestionFeedbackRepository = suggestionFeedbackRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userInteractionRepository = userInteractionRepository;
        _predictionService = predictionService;
    }

    public async Task<List<RecipeDto>> GetRandomSuggestionsAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeRepository.GetRandomRecipesAsync(count, cancellationToken);
        return recipes.Select(r => new RecipeDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Servings = r.Servings,
            PrepareTime = r.PrepareTime,
            CookTime = r.CookTime,
            Difficulty = r.DifficultyLevel.ToString(),
            MealCategory = r.MealCategory.ToString(),
            CoverImage = r.CoverMediaUrl.Url,
            CreatedAt = r.CreatedAt,
            AverageRating = r.AverageRating,
            TotalRatings = r.TotalRatings,
            TotalViews = r.TotalViews,
        }).ToList();
    }

    public async Task<List<RecipeDto>> GetUserSpecificSuggestionsAsync(Guid siteUserId, int count = 5, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithProfileByIdAsync(siteUserId, cancellationToken);
        var userPreferences = user?.Profile?.DietaryPreferences.Select(d => d.Name).ToList() ?? new List<string>();

        List<Guid> recipeIds;

        try
        {
            recipeIds = await _predictionService.GetRecipeSuggestionsAsync(userPreferences, count, cancellationToken);
        }
        catch (Exception)
        {
            // Log the exception if you have a logger, e.g.
            // _logger.LogError(ex, "Prediction service failed, fallback to random recipes");

            // Fallback: return random recipes
            return await GetRandomSuggestionsAsync(count, cancellationToken);
        }

        if (recipeIds == null || recipeIds.Count == 0)
        {
            // If prediction returns no results, fallback
            return await GetRandomSuggestionsAsync(count, cancellationToken);
        }

        var recipes = await _recipeRepository.GetByIdsAsync(recipeIds, cancellationToken);

        // Ensure order according to prediction result
        var recipeDict = recipes.ToDictionary(r => r.Id);

        var orderedRecipes = recipeIds
            .Where(id => recipeDict.ContainsKey(id))
            .Select(id => recipeDict[id])
            .ToList();

        return orderedRecipes.Select(r => new RecipeDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Servings = r.Servings,
            PrepareTime = r.PrepareTime,
            CookTime = r.CookTime,
            Difficulty = r.DifficultyLevel.ToString(),
            MealCategory = r.MealCategory.ToString(),
            CoverImage = r.CoverMediaUrl.Url,
            CreatedAt = r.CreatedAt,
            AverageRating = r.AverageRating,
            TotalRatings = r.TotalRatings,
            TotalViews = r.TotalViews,
        }).ToList();
    }


    public async Task ProcessFeedbackAsync(Guid recipeId, bool isLike, Guid siteUserId, CancellationToken cancellationToken = default)
    {
        // check if the recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new ArgumentException("Recipe not found", nameof(recipeId));
        }

        // check if site user exists
        var siteUser = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (siteUser == null)
        {
            throw new ArgumentException("Site user not found", nameof(siteUserId));
        }

        var feedback = new SuggestionFeedback(
            siteUserId,
            recipeId,
            isLike
        );

        var interaction = new UserInteraction(
            siteUserId,
            recipeId,
            isLike ? "like" : "dislike",
            1
        );

        await _suggestionFeedbackRepository.AddAsync(feedback, cancellationToken);
        await _userInteractionRepository.AddAsync(interaction, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

}