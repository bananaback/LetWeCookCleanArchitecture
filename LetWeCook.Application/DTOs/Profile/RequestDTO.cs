namespace LetWeCook.Application.DTOs.Profile;

public class RequestDTO
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public Guid? OldReferenceId { get; set; }
    public Guid NewReferenceId { get; set; }
    public string ResponseMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null;
}