namespace LetWeCook.Domain.Entities;

public class UserDietaryPreference
{
    public Guid UserProfileId { get; set; }
    public Guid DietaryPreferenceId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public DietaryPreference DietaryPreference { get; set; } = null!;

    private UserDietaryPreference() { }

    public UserDietaryPreference(Guid userProfileId, Guid dietaryPreferenceId)
    {
        UserProfileId = userProfileId;
        DietaryPreferenceId = dietaryPreferenceId;
    }
}