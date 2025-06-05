namespace LetWeCook.Application.DTOs.Suggestions;

public class PreferenceRequest
{
    public bool Pref_Vegetarian { get; set; } = false;
    public bool Pref_Vegan { get; set; } = false;
    public bool Pref_GlutenFree { get; set; } = false;
    public bool Pref_Pescatarian { get; set; } = false;
    public bool Pref_LowCalorie { get; set; } = false;
    public bool Pref_HighProtein { get; set; } = false;
    public bool Pref_LowCarb { get; set; } = false;
    public bool Pref_LowFat { get; set; } = false;
    public bool Pref_LowSugar { get; set; } = false;
    public bool Pref_HighFiber { get; set; } = false;
    public bool Pref_LowSodium { get; set; } = false;
}

