using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IDonationRepository : IRepository<Donation>
{
    Task<Donation?> GetDonationDetailsById(Guid id, CancellationToken cancellationToken);
    Task<List<Donation>> GetCompletedDonationsByRecipeId(Guid recipeId, CancellationToken cancellationToken);
}