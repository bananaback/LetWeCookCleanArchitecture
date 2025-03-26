using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.Ingredient;

namespace LetWeCook.Application.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientCategoryDTO>> GetAllIngredientCategoriesAsync(CancellationToken cancellationToken);
    Task<IngredientDto> CreateIngredientAsync(CreateIngredientRequestDto request, CancellationToken cancellationToken);
    Task<IngredientDto> GetIngredientAsync(Guid id, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetRandomIngredientsAsync(int count, CancellationToken cancellationToken);
    Task<List<IngredientDto>> GetIngredientsByCategoryAsync(string category, int count, CancellationToken cancellationToken);
}