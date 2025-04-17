using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Services;

public class RequestService : IRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRequestRepository _userRequestRepository;
    public RequestService(IUnitOfWork unitOfWork, IUserRequestRepository userRequestRepository)
    {
        _unitOfWork = unitOfWork;
        _userRequestRepository = userRequestRepository;
    }

    public async Task<bool> ApproveRequestByRequestIdAsync(Guid requestId, string responseMessage, CancellationToken cancellationToken = default)
    {
        var request = await _userRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request == null)
        {
            return false;
        }
        request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);
        await _userRequestRepository.UpdateAsync(request, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        return result > 0;
    }


    public Task<bool> DeleteRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<RequestDTO>> GetAllRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _userRequestRepository.GetAllAsync(cancellationToken);
        return requests.Select(r => new RequestDTO
        {
            RequestId = r.Id,
            Type = r.Type.ToString(),
            OldReferenceId = r.OldReferenceId,
            NewReferenceId = r.NewReferenceId,
            ResponseMessage = r.ResponseMessage,
            Status = r.Status.ToString(),
            CreatedByUserId = r.CreatedByUserId,
            CreatedByUserName = r.CreatedByUserName,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });
    }

    public Task<RequestDTO> GetRequestByIdAsync(int requestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RequestDTO> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<RequestDTO>> GetUserRequestsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var requests = await _userRequestRepository.GetByUserIdAsync(userId, cancellationToken);
        return requests.Select(r => new RequestDTO
        {
            Type = r.Type.ToString(),
            OldReferenceId = r.OldReferenceId,
            NewReferenceId = r.NewReferenceId,
            ResponseMessage = r.ResponseMessage,
            Status = r.Status.ToString(),
            CreatedByUserId = r.CreatedByUserId,
            CreatedByUserName = r.CreatedByUserName,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });
    }

    public async Task<bool> RejectRequestByRequestIdAsync(Guid requestId, string responseMessage, CancellationToken cancellationToken = default)
    {
        var request = await _userRequestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request == null)
        {
            return false;
        }
        request.UpdateStatus(UserRequestStatus.REJECTED, responseMessage);
        await _userRequestRepository.UpdateAsync(request, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        return result > 0;
    }
}
