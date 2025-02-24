using System.Reflection.Metadata;
using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.Aggregates;

public class SiteUser : Entity
{
    public bool IsRemoved { get; private set; }

}