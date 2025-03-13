using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.ValueObjects;

namespace LetWeCook.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IDietaryPreferenceRepository _dietaryPreferenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UserProfileService(IUserRepository userRepository, IDietaryPreferenceRepository dietaryPreferenceRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _dietaryPreferenceRepository = dietaryPreferenceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDTO?> GetProfileAsync(Guid siteUserId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetWithProfileByIdAsync(siteUserId, cancellationToken);

        if (user?.Profile == null)
        {
            return null;
        }

        return new UserProfileDTO
        {
            SiteUserId = user.Id,
            FirstName = user.Profile.Name.FirstName,
            LastName = user.Profile.Name.LastName,
            BirthDate = user.Profile.BirthDate,
            Gender = user.Profile.Gender.ToString(),
            Email = user.Profile.Email,
            HouseNumber = user.Profile.Address.HouseNumber,
            Street = user.Profile.Address.Street,
            Ward = user.Profile.Address.Ward,
            District = user.Profile.Address.District,
            City = user.Profile.Address.ProvinceOrCity,
            Bio = user.Profile.Bio,
            Facebook = user.Profile.Facebook,
            Instagram = user.Profile.Instagram,
            PhoneNumber = user.Profile.PhoneNumber,
            ProfilePic = user.Profile.ProfilePic,
            DietaryPreferences = user.Profile.DietaryPreferences.Select(dp => dp.Name).ToList()
        };
    }


    public async Task<UserProfileDTO> UpdateProfileAsync(Guid siteUserid, UpdateUserProfileRequestDTO request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithProfileByIdAsync(siteUserid, cancellationToken);

        if (user == null)
        {
            throw new UpdateProfileException($"Site user with id {siteUserid} not found.");
        }

        try
        {
            if (!Enum.TryParse<Gender>(request.Gender, true, out var genderEnum))
            {
                throw new UpdateProfileException($"Invalid gender value: {request.Gender}");
            }
            Name name = new Name(request.FirstName, request.LastName);
            Address address = new Address(request.Address.HouseNumber, request.Address.Street, request.Address.Ward, request.Address.District, request.Address.City);
            List<DietaryPreference> dietaryPreferences = await _dietaryPreferenceRepository.GetAllAsync(cancellationToken);

            user.UpdateProfile(
                name,
                request.BirthDate,
                request.Email,
                genderEnum,
                address,
                request.DietaryPreferences,
                dietaryPreferences,
                request.Bio,
                request.Facebook,
                request.Instagram,
                request.PhoneNumber,
                request.ProfilePicture);

            await _unitOfWork.CommitAsync(cancellationToken);

            return new UserProfileDTO
            {
                SiteUserId = user.Id,
                FirstName = user.Profile!.Name.FirstName,
                LastName = user.Profile!.Name.LastName,
                BirthDate = user.Profile!.BirthDate,
                Gender = user.Profile.Gender.ToString(),
                Email = user.Profile.Email,
                HouseNumber = user.Profile.Address.HouseNumber,
                Street = user.Profile.Address.Street,
                Ward = user.Profile.Address.Ward,
                District = user.Profile.Address.District,
                City = user.Profile.Address.ProvinceOrCity,
                Bio = user.Profile.Bio,
                Facebook = user.Profile.Facebook,
                Instagram = user.Profile.Instagram,
                PhoneNumber = user.Profile.PhoneNumber,
                ProfilePic = user.Profile.ProfilePic,
                DietaryPreferences = user.Profile.DietaryPreferences.Select(dp => dp.Name).ToList()
            };

        }
        catch (Exception ex)
        {
            throw new UpdateProfileException(ex.Message);
        }
    }
}