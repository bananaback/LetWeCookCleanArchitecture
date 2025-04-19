using LetWeCook.Application.DTOs.Profile;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Services;

public class RequestService : IRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRequestRepository _userRequestRepository;
    private readonly IDetailRepository _detailRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMediaUrlRepository _mediaUrlRepository;
    public RequestService(
        IUnitOfWork unitOfWork,
        IUserRequestRepository userRequestRepository,
        IIngredientRepository ingredientRepository,
        IDetailRepository detailRepository,
        IMediaUrlRepository mediaUrlRepository)
    {
        _unitOfWork = unitOfWork;
        _userRequestRepository = userRequestRepository;
        _ingredientRepository = ingredientRepository;
        _detailRepository = detailRepository;
        _mediaUrlRepository = mediaUrlRepository;
    }

    public async Task ApproveRequestByNewRefIdAsync(Guid newReferenceId, string responseMessage, CancellationToken cancellationToken = default)
    {
        var request = await _userRequestRepository.GetPendingByNewReferenceIdAsync(newReferenceId, cancellationToken);
        if (request == null)
        {
            throw new RequestRetrievalException("Request not found or already processed.");
        }

        if (request.Type == UserRequestType.CREATE_INGREDIENT)
        {
            var ingredientId = request.NewReferenceId;
            var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId, cancellationToken);
            if (ingredient == null)
            {
                throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not found.");
            }

            ingredient.SetVisible(true);
            ingredient.SetPreview(false);

            await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);
            request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);

            await _userRequestRepository.UpdateAsync(request, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        else if (request.Type == UserRequestType.UPDATE_INGREDIENT)
        {
            if (request.OldReferenceId == null)
            {
                throw new InvalidOperationException("Old reference ID is required for update requests.");
            }
            var oldIngredient = await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(request.OldReferenceId!.Value, cancellationToken);
            var newIngredient = await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(request.NewReferenceId, cancellationToken);

            // check null of old and new ingredient
            if (oldIngredient == null || newIngredient == null)
            {
                throw new IngredientRetrievalException($"Ingredient with ID {request.OldReferenceId} or {request.NewReferenceId} not found.");
            }

            // Copy scalar properties
            oldIngredient.CopyFrom(newIngredient);

            // Get reference to details before clearing
            var detailsToRemove = oldIngredient.Details.ToList();

            // First remove all existing details from the collection (but keep references)
            oldIngredient.Details.Clear();

            // Now tell the context to remove these details
            //foreach (var detail in detailsToRemove)
            //{
            //    await _detailRepository.RemoveAsync(detail, cancellationToken);
            //}

            // Add new details
            foreach (var sourceDetail in newIngredient.Details)
            {
                var mediaUrls = sourceDetail.MediaUrls.Select(m => new MediaUrl(m.MediaType, m.Url)).ToList();
                var detail = new Detail(sourceDetail.Title, sourceDetail.Description, mediaUrls, sourceDetail.Order);

                oldIngredient.AddDetail(detail);

                await _detailRepository.AddAsync(detail, cancellationToken);
            }

            oldIngredient.SetVisible(true);
            oldIngredient.SetPreview(false);

            await _ingredientRepository.UpdateAsync(oldIngredient, cancellationToken);
            request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);

            await _userRequestRepository.UpdateAsync(request, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
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

    public async Task RejectRequestByNewRefIdAsync(Guid newReferenceId, string responseMessage, CancellationToken cancellationToken = default)
    {
        var request = await _userRequestRepository.GetPendingByNewReferenceIdAsync(newReferenceId, cancellationToken);

        if (request == null)
        {
            throw new RequestRetrievalException("Request not found or already processed.");
        }

        request.UpdateStatus(UserRequestStatus.REJECTED, responseMessage);

        await _userRequestRepository.UpdateAsync(request, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
