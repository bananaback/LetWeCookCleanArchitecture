using LetWeCook.Application.DTOs.RecipeSnapshots;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Utilities;

namespace LetWeCook.Application.Services;

public class RecipeSnapshotService : IRecipeSnapshotService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeRatingRepository _recipeRatingRepository;
    private readonly IDonationRepository _donationRepository;

    public RecipeSnapshotService(IRecipeRepository recipeRepository, IRecipeRatingRepository recipeRatingRepository, IDonationRepository donationRepository)
    {
        _recipeRepository = recipeRepository;
        _recipeRatingRepository = recipeRatingRepository;
        _donationRepository = donationRepository;
    }

    public async Task<List<RecipeSnapshotDto>> GetRecipeSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeRepository.GetAllRecipesWithIngredientsAsync(cancellationToken);

        var recipeSnapshots = new List<RecipeSnapshotDto>();

        foreach (var recipe in recipes)
        {
            var nutritionSummary = RecipeNutritionCalculator.CalculateRecipeNutrition(recipe);
            var averageCommentLength = await _recipeRatingRepository.CountRecipeAverageCommentLengthAsync(recipe.Id, cancellationToken);
            var donationAmount = await _donationRepository.GetRecipeTotalDonationAmount(recipe.Id, cancellationToken);
            var recipeSnapshot = new RecipeSnapshotDto
            {
                Id = recipe.Id,
                Name = recipe.Name,

                Rating = recipe.AverageRating,
                CommentLength = averageCommentLength,
                DonatedAmount = donationAmount,
                ViewsCount = recipe.TotalViews,

                Calories = nutritionSummary.Calories ?? 0,
                Protein = nutritionSummary.Protein ?? 0,
                Carbohydrates = nutritionSummary.Carbohydrates ?? 0,
                Fat = nutritionSummary.Fat ?? 0,
                Sugar = nutritionSummary.Sugar ?? 0,
                Fiber = nutritionSummary.Fiber ?? 0,
                Sodium = nutritionSummary.Sodium ?? 0,

                Tag_Vietnamese = false,
                Tag_Brazilian = false,
                Tag_Chinese = false,
                Tag_Caribbean = false,
                Tag_Mexican = false,
                Tag_African = false,
                Tag_Italian = false,
                Tag_German = false,
                Tag_Indian = false,
                Tag_Thai = false,
                Tag_Japanese = false,
                Tag_Korean = false,
                Tag_American = false,
                Tag_French = false,
                Tag_Mediterranean = false,
                Tag_MiddleEastern = false,
                Tag_Spanish = false,
                Tag_Greek = false,
                Tag_Turkish = false,

                Meal_Main = false,
                Meal_Side = false,
                Meal_Soup = false,
                Meal_Dessert = false,
                Meal_Beverage = false,

                Diff_Easy = false,
                Diff_Medium = false,
                Diff_Hard = false
            };


            // Set cuisine tags
            foreach (var tag in recipe.Tags)
            {
                // check all the tag exist in the dto
                recipeSnapshot.GetType().GetProperty($"Tag_{tag.Name}")?.SetValue(recipeSnapshot, true);
            }

            // Set meal types, one recipe just have one meal type
            // get recipe.MealCategory then we lowercase and uppercase the first letter
            var mealTypeName = recipe.MealCategory.ToString();
            if (!string.IsNullOrEmpty(mealTypeName))
            {
                var mealTypeProperty = recipeSnapshot.GetType().GetProperty($"Meal_{char.ToUpper(mealTypeName[0]) + mealTypeName.Substring(1).ToLower()}");
                mealTypeProperty?.SetValue(recipeSnapshot, true);
            }

            // Set difficulty levels
            var difficultyLevelName = recipe.DifficultyLevel.ToString();
            if (!string.IsNullOrEmpty(difficultyLevelName))
            {
                var difficultyLevelProperty = recipeSnapshot.GetType().GetProperty($"Diff_{char.ToUpper(difficultyLevelName[0]) + difficultyLevelName.Substring(1).ToLower()}");
                difficultyLevelProperty?.SetValue(recipeSnapshot, true);
            }

            recipeSnapshots.Add(recipeSnapshot);
        }

        return recipeSnapshots;
    }
}
