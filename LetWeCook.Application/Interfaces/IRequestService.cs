using LetWeCook.Application.DTOs.Profile;

namespace LetWeCook.Application.Interfaces;

public interface IRequestService
{
    //Task<bool> CreateRequestAsync(CreateRequestDto requestDto, CancellationToken cancellationToken = default);
    //Task<bool> UpdateRequestAsync(UpdateRequestDto requestDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteRequestAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<RequestDTO> GetRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDTO>> GetAllRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDTO>> GetUserRequestsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ApproveRequestByRequestIdAsync(Guid requestId, string responseMessage, CancellationToken cancellationToken = default);
    Task<bool> RejectRequestByRequestIdAsync(Guid requestId, string responseMessage, CancellationToken cancellationToken = default);
}