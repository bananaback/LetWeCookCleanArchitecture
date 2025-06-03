namespace LetWeCook.Application.DTOs.RecipeCollections;

public class RecipeCollectionQueryRequestDto
{
    public string SearchTerm { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public string SortBy { get; set; } = string.Empty;
    public bool IsAscending { get; set; } = true;
}