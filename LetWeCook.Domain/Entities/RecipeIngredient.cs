using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Domain.Entities;

public class RecipeIngredient : Entity
{
    public Recipe Recipe { get; set; } = null!;
    public Guid RecipeId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public Guid IngredientId { get; set; }
    public float Quantity { get; set; } = 0.0F;
    public UnitEnum Unit { get; set; } = UnitEnum.Unknown;

    private RecipeIngredient() { } // for EF Core

    public RecipeIngredient(Guid recipeId, Guid ingredientId, float quantity, UnitEnum unit)
    {
        RecipeId = recipeId;
        IngredientId = ingredientId;
        Quantity = quantity;
        Unit = unit;
    }
}