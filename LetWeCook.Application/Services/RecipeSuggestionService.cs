using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Services;

public class RecipeSuggestionService : IRecipeSuggestionService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ISuggestionFeedbackRepository _suggestionFeedbackRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RecipeSuggestionService(IRecipeRepository recipeRepository, ISuggestionFeedbackRepository suggestionFeedbackRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _recipeRepository = recipeRepository;
        _suggestionFeedbackRepository = suggestionFeedbackRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
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

        await _suggestionFeedbackRepository.AddAsync(feedback, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

}