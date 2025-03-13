using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class DietaryPreferenceRepository : Repository<DietaryPreference>, IDietaryPreferenceRepository
{
    public DietaryPreferenceRepository(LetWeCookDbContext context) : base(context)
    {
    }
}