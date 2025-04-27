using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Infrastructure.Persistence.Models;

public class RecipeTagging
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public Guid RecipeTagId { get; set; }
    public RecipeTag RecipeTag { get; set; } = null!;
}