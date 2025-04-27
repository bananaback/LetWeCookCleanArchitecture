using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class RecipeDetail : Entity
{
    public Detail Detail { get; private set; } = null!;
    public int Order { get; private set; }
}