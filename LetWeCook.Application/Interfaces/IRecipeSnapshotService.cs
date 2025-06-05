using LetWeCook.Application.DTOs.RecipeSnapshots;

namespace LetWeCook.Application.Interfaces;

public interface IRecipeSnapshotService
{
    Task<List<RecipeSnapshotDto>> GetRecipeSnapshotsAsync(
        CancellationToken cancellationToken = default);
}