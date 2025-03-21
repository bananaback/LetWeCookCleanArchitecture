using LetWeCook.Domain.Common;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Domain.Entities;

public class MediaUrl : Entity
{
    public MediaType MediaType { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public MediaUrl() : base() { } // For EF Core

    public MediaUrl(MediaType mediaType, string url) : base(Guid.NewGuid())
    {
        MediaType = mediaType;
        Url = url;
    }
}