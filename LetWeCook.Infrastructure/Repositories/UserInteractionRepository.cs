using LetWeCook.Application.DTOs.UserInteractions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Repositories;

public class UserInteractionRepository : Repository<UserInteraction>, IUserInteractionRepository
{
    private readonly LetWeCookDbContext _db;

    public UserInteractionRepository(LetWeCookDbContext context) : base(context)
    {
        _db = context;
    }

    public async Task<List<AggregatedInteractionDto>> GetAggregatedInteractionsAsync()
    {
        return await _db.AggregatedInteractions
            .FromSqlRaw("EXEC AggregateUserRecipeInteractions")
            .ToListAsync();
    }
}