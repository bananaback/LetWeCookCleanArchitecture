using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    public UserProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserProfile> UpdateProfileAsync(Guid siteUserid, UpdateUserProfileRequestDTO request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithProfileByIdAsync(siteUserid, cancellationToken);

        if (user == null)
        {

        }
        return null;
    }
}