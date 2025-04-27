using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class IngredientDetail : Entity
{
    public Detail Detail { get; private set; } = null!;
    public int Order { get; private set; }

    private IngredientDetail() { }

    public IngredientDetail(Detail detail, int order)
    {
        Detail = detail;
        Order = order;
    }
}