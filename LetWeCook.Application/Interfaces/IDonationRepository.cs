using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IDonationRepository : IRepository<Donation>
{
    Task<Donation?> GetDonationDetailsById(Guid id, CancellationToken cancellationToken);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
    Task<Donation?> GetWithRecipeDonatorAndAuthorAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Donation>> GetCompletedDonationsByRecipeId(Guid recipeId, CancellationToken cancellationToken);
    Task<int> GetRecipeTotalDonationAmount(Guid recipeId, CancellationToken cancellationToken);
}