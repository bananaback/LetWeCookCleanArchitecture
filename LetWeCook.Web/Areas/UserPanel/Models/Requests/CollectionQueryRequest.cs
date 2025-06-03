namespace LetWeCook.Web.Areas.UserPanel.Models.Requests;

public class CollectionQueryRequest
{
    public string SearchTerm { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public string SortBy { get; set; } = string.Empty;
    public bool IsAscending { get; set; } = true;

    // Additional properties can be added as needed for filtering or sorting
}