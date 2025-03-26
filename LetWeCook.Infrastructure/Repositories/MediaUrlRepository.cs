using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class MediaUrlRepository : Repository<MediaUrl>, IMediaUrlRepository
{
    public MediaUrlRepository(LetWeCookDbContext dbContext) : base(dbContext)
    {
    }
}