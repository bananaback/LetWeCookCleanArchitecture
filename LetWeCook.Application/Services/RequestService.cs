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
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMediaUrlRepository _mediaUrlRepository;
    public RequestService(
        IUnitOfWork unitOfWork,
        IUserRequestRepository userRequestRepository,
        IIngredientRepository ingredientRepository,
        IRecipeRepository recipeRepository,
        IDetailRepository detailRepository,
        IMediaUrlRepository mediaUrlRepository)
    {
        _unitOfWork = unitOfWork;
        _userRequestRepository = userRequestRepository;
        _ingredientRepository = ingredientRepository;
        _recipeRepository = recipeRepository;
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

            // First remove all existing details from the collection (but keep references)
            oldIngredient.IngredientDetails.Clear();

            // Add new details
            foreach (var sourceDetail in newIngredient.IngredientDetails)
            {
                var mediaUrls = sourceDetail.Detail.MediaUrls.Select(m => new MediaUrl(m.MediaType, m.Url)).ToList();
                var detail = new Detail(sourceDetail.Detail.Title, sourceDetail.Detail.Description, mediaUrls);
                await _detailRepository.AddAsync(detail, cancellationToken);
                var ingredientDetail = new IngredientDetail(detail, sourceDetail.Order);

                oldIngredient.AddDetail(ingredientDetail);
            }

            oldIngredient.SetVisible(true);
            oldIngredient.SetPreview(false);

            await _ingredientRepository.UpdateAsync(oldIngredient, cancellationToken);
            request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);

            await _userRequestRepository.UpdateAsync(request, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        else if (request.Type == UserRequestType.CREATE_RECIPE)
        {
            var recipeId = request.NewReferenceId;
            var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
            if (recipe == null)
            {
                throw new IngredientRetrievalException($"Recipe with ID {recipeId} not found.");
            }

            recipe.SetVisible(true);
            recipe.SetPreview(false);

            await _recipeRepository.UpdateAsync(recipe, cancellationToken);
            request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);

            await _userRequestRepository.UpdateAsync(request, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        else if (request.Type == UserRequestType.UPDATE_RECIPE)
        {
            if (request.OldReferenceId == null)
            {
                throw new InvalidOperationException("Old reference ID is required for update requests.");
            }
            var oldRecipe = await _recipeRepository.GetRecipeDetailsByIdAsync(request.OldReferenceId!.Value, cancellationToken);
            var newRecipe = await _recipeRepository.GetRecipeDetailsByIdAsync(request.NewReferenceId, cancellationToken);

            // check null of old and new recipe
            if (oldRecipe == null || newRecipe == null)
            {
                throw new RecipeRetrievalException($"Recipe with ID {request.OldReferenceId} or {request.NewReferenceId} not found.");
            }

            // Copy scalar properties
            oldRecipe.CopyFlatProperty(newRecipe);

            // First remove all existing details from the collection (but keep references)
            oldRecipe.RecipeDetails.Clear();

            // Add new details
            foreach (var sourceDetail in newRecipe.RecipeDetails)
            {
                var mediaUrls = sourceDetail.Detail.MediaUrls.Select(m => new MediaUrl(m.MediaType, m.Url)).ToList();
                var detail = new Detail(sourceDetail.Detail.Title, sourceDetail.Detail.Description, mediaUrls);
                await _detailRepository.AddAsync(detail, cancellationToken);
                var recipeDetail = new RecipeDetail(detail, sourceDetail.Order);

                oldRecipe.AddStep(recipeDetail);
            }

            // First remove all existing ingredients from the collection (but keep references)
            oldRecipe.RecipeIngredients.Clear();
            // Add new ingredients
            foreach (var sourceIngredient in newRecipe.RecipeIngredients)
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(sourceIngredient.IngredientId, cancellationToken);
                if (ingredient == null)
                {
                    throw new IngredientRetrievalException($"Ingredient with ID {sourceIngredient.IngredientId} not found.");
                }
                var recipeIngredient = new RecipeIngredient(oldRecipe.Id, ingredient.Id, sourceIngredient.Quantity, sourceIngredient.Unit);
                oldRecipe.AddIngredient(recipeIngredient);
            }

            // First remove all existing tags from the collection (but keep references)
            oldRecipe.Tags.Clear();
            // Add new tags
            oldRecipe.AddRecipeTags(newRecipe.Tags);

            oldRecipe.SetVisible(true);
            oldRecipe.SetPreview(false);

            await _recipeRepository.UpdateAsync(oldRecipe, cancellationToken);
            request.UpdateStatus(UserRequestStatus.APPROVED, responseMessage);

            await _userRequestRepository.UpdateAsync(request, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("Unsupported request type.");
        }
    }

    public Task<bool> DeleteRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<RequestDTO>> GetAllRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _userRequestRepository.GetAllAsync(cancellationToken);
        requests = requests.OrderByDescending(r => r.CreatedAt).ToList();
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
