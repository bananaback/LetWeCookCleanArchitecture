using LetWeCook.Application.DTOs.Recipe;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeTagDto>> GetAllRecipeTagsAsync(CancellationToken cancellationToken = default);
}