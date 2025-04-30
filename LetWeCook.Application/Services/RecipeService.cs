using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Interfaces;

namespace LetWeCook.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeTagRepository _recipeTagRepository;

    public RecipeService(IRecipeTagRepository recipeTagRepository)
    {
        _recipeTagRepository = recipeTagRepository;
    }
    public async Task<List<RecipeTagDto>> GetAllRecipeTagsAsync(CancellationToken cancellationToken = default)
    {
        var recipeTags = await _recipeTagRepository.GetAllAsync(cancellationToken);
        return recipeTags.Select(tag => new RecipeTagDto
        {
            Id = tag.Id,
            Name = tag.Name
        }).ToList();
    }
}