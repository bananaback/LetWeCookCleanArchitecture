using LetWeCook.Domain.Aggregates;
using Microsoft.AspNetCore.Identity;

namespace LetWeCook.Infrastructure.Persistence;


public class ApplicationUser : IdentityUser<Guid>
{
    public Guid SiteUserId { get; set; }
    public SiteUser SiteUser { get; set; } = null!;

    public ApplicationUser()
    {
        Id = Guid.NewGuid(); // Explicitly set its own ID
    }

    public void SyncFromSiteUser(SiteUser siteUser)
    {
        SiteUserId = siteUser.Id;
        SiteUser = siteUser;
    }
}