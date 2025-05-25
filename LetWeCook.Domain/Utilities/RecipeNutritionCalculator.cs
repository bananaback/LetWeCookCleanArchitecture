using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Domain.Utilities;

public class RecipeNutritionCalculator
{
    // Conversion factors to grams
    private static readonly Dictionary<UnitEnum, double> UnitToGramConversions = new()
    {
        { UnitEnum.Gram, 1.0 },
        { UnitEnum.Kilogram, 1000.0 },
        { UnitEnum.Milliliter, 1.0 }, // Assumes density similar to water (1 mL = 1 g)
        { UnitEnum.Liter, 1000.0 }, // Assumes density similar to water
        { UnitEnum.Ounce, 28.3495 },
        { UnitEnum.Pound, 453.592 },
        { UnitEnum.Cup, 240.0 }, // Approximate, varies by ingredient
        { UnitEnum.Tablespoon, 15.0 }, // Approximate
        { UnitEnum.Teaspoon, 5.0 }, // Approximate
        { UnitEnum.Piece, 50.0 }, // Highly variable, placeholder value
        { UnitEnum.Slice, 25.0 }, // Highly variable, placeholder value
        { UnitEnum.Pinch, 0.3 }, // Approximate
        { UnitEnum.Dash, 0.15 }, // Approximate
        { UnitEnum.Unknown, 1.0 } // Fallback, assumes gram
    };

    public class NutritionSummary
    {
        public float? Calories { get; set; }
        public float? Protein { get; set; }
        public float? Carbohydrates { get; set; }
        public float? Fat { get; set; }
        public float? Sugar { get; set; }
        public float? Fiber { get; set; }
        public float? Sodium { get; set; }
    }

    public static NutritionSummary CalculateRecipeNutrition(Recipe recipe)
    {
        if (recipe == null) throw new ArgumentNullException(nameof(recipe));

        var summary = new NutritionSummary();
        float? totalCalories = 0f;
        float? totalProtein = 0f;
        float? totalCarbohydrates = 0f;
        float? totalFat = 0f;
        float? totalSugar = 0f;
        float? totalFiber = 0f;
        float? totalSodium = 0f;

        foreach (var recipeIngredient in recipe.RecipeIngredients)
        {
            if (recipeIngredient.Ingredient == null) continue;

            // Convert quantity to grams
            double conversionFactor = UnitToGramConversions[recipeIngredient.Unit];
            double quantityInGrams = recipeIngredient.Quantity * conversionFactor;

            // Sum nutritional values if they are not null
            if (recipeIngredient.Ingredient.Calories.HasValue)
                totalCalories = (totalCalories ?? 0f) + (float)(recipeIngredient.Ingredient.Calories.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Protein.HasValue)
                totalProtein = (totalProtein ?? 0f) + (float)(recipeIngredient.Ingredient.Protein.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Carbohydrates.HasValue)
                totalCarbohydrates = (totalCarbohydrates ?? 0f) + (float)(recipeIngredient.Ingredient.Carbohydrates.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Fat.HasValue)
                totalFat = (totalFat ?? 0f) + (float)(recipeIngredient.Ingredient.Fat.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Sugar.HasValue)
                totalSugar = (totalSugar ?? 0f) + (float)(recipeIngredient.Ingredient.Sugar.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Fiber.HasValue)
                totalFiber = (totalFiber ?? 0f) + (float)(recipeIngredient.Ingredient.Fiber.Value * quantityInGrams);
            if (recipeIngredient.Ingredient.Sodium.HasValue)
                totalSodium = (totalSodium ?? 0f) + (float)(recipeIngredient.Ingredient.Sodium.Value * quantityInGrams);
        }

        // Assign values to summary, preserving null if no valid data was summed
        summary.Calories = totalCalories != 0f ? totalCalories : null;
        summary.Protein = totalProtein != 0f ? totalProtein : null;
        summary.Carbohydrates = totalCarbohydrates != 0f ? totalCarbohydrates : null;
        summary.Fat = totalFat != 0f ? totalFat : null;
        summary.Sugar = totalSugar != 0f ? totalSugar : null;
        summary.Fiber = totalFiber != 0f ? totalFiber : null;
        summary.Sodium = totalSodium != 0f ? totalSodium : null;

        return summary;
    }
}