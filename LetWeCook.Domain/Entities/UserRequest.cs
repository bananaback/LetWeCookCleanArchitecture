using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Domain.Entities;

public class UserRequest : Entity
{
    public UserRequestType Type { get; private set; }
    public Guid? OldReferenceId { get; private set; }
    public Guid NewReferenceId { get; private set; }
    public string ResponseMessage { get; private set; } = string.Empty;
    public UserRequestStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string CreatedByUserName { get; private set; } = string.Empty;
    public SiteUser CreatedByUser { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; } = null;

    public UserRequest() { } // For EF Core

    public UserRequest(
        UserRequestType type,
        Guid? oldreferenceId,
        Guid newReferenceId,
        string responseMessage,
        UserRequestStatus status,
        Guid createdByUserId,
        string createdByUserName
    )
    {
        Type = type;
        OldReferenceId = oldreferenceId;
        NewReferenceId = newReferenceId;
        ResponseMessage = responseMessage;
        Status = status;
        CreatedByUserId = createdByUserId;
        CreatedByUserName = createdByUserName;
    }

    public void UpdateStatus(UserRequestStatus status, string responseMessage)
    {
        Status = status;
        ResponseMessage = responseMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AlterNewRefId(Guid newReferenceId)
    {
        NewReferenceId = newReferenceId;
    }
}