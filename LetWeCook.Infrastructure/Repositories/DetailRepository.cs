using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class DetailRepository : Repository<Detail>, IDetailRepository
{
    public DetailRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }
}