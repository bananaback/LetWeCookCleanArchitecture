using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfile> UpdateProfileAsync(Guid siteUserId, UpdateUserProfileRequestDTO request, CancellationToken cancellationToken);
}