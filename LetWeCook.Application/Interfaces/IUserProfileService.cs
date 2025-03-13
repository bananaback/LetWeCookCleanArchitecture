using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDTO> UpdateProfileAsync(Guid siteUserId, UpdateUserProfileRequestDTO request, CancellationToken cancellationToken);
    Task<UserProfileDTO?> GetProfileAsync(Guid siteUserId, CancellationToken cancellationToken);
    Task<List<DietaryPreferenceDTO>> GetAllSystemDietaryPreferencesAsync(CancellationToken cancellationToken);
}