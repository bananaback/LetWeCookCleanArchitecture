using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Entities;

public class Detail : Entity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public List<MediaUrl> MediaUrls { get; private set; } = new List<MediaUrl>();
    public Detail() : base() { } // For EF Core

    public Detail(string title, string description, List<MediaUrl> mediaUrls, int order)
        : base(Guid.NewGuid())
    {
        Title = title;
        Description = description;
        MediaUrls = mediaUrls;
        Order = order;
    }
}
