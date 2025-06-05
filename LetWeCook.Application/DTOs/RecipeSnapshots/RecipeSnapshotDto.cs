
namespace LetWeCook.Application.DTOs.RecipeSnapshots;

public class RecipeSnapshotDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Numerical fields
    public double Rating { get; set; }
    public int CommentLength { get; set; }
    public double DonatedAmount { get; set; }
    public int ViewsCount { get; set; }

    public float Calories { get; set; }
    public float Protein { get; set; }
    public float Carbohydrates { get; set; }
    public float Fat { get; set; }
    public float Sugar { get; set; }
    public float Fiber { get; set; }
    public float Sodium { get; set; }

    // Cuisine Tags (boolean)
    public bool Tag_Vietnamese { get; set; }
    public bool Tag_Brazilian { get; set; }
    public bool Tag_Chinese { get; set; }
    public bool Tag_Caribbean { get; set; }
    public bool Tag_Mexican { get; set; }
    public bool Tag_African { get; set; }
    public bool Tag_Italian { get; set; }
    public bool Tag_German { get; set; }
    public bool Tag_Indian { get; set; }
    public bool Tag_Thai { get; set; }
    public bool Tag_Japanese { get; set; }
    public bool Tag_Korean { get; set; }
    public bool Tag_American { get; set; }
    public bool Tag_French { get; set; }
    public bool Tag_Mediterranean { get; set; }
    public bool Tag_MiddleEastern { get; set; }
    public bool Tag_Spanish { get; set; }
    public bool Tag_Greek { get; set; }
    public bool Tag_Turkish { get; set; }

    // Meal types (boolean)
    public bool Meal_Main { get; set; }
    public bool Meal_Side { get; set; }
    public bool Meal_Soup { get; set; }
    public bool Meal_Dessert { get; set; }
    public bool Meal_Beverage { get; set; }

    // Difficulty (boolean)
    public bool Diff_Easy { get; set; }
    public bool Diff_Medium { get; set; }
    public bool Diff_Hard { get; set; }
}
