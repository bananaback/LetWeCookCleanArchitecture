using LetWeCook.Domain.Entities;

namespace LetWeCook.Infrastructure.Persistence.Models;

public class DetailMediaUrl
{
    public Guid DetailId { get; set; }
    public Guid MediaUrlId { get; set; }
    public Detail Detail { get; set; } = null!;
    public MediaUrl MediaUrl { get; set; } = null!;
}