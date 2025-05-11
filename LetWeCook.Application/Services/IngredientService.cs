using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.Ingredient;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRequestRepository _userRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    public IngredientService(
        IIngredientCategoryRepository ingredientCategoryRepository,
        IIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IUserRequestRepository userRequestRepository)
    {
        _ingredientCategoryRepository = ingredientCategoryRepository;
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _userRequestRepository = userRequestRepository;
    }


    public async Task<Guid?> CreateIngredientAsync(Guid appUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken)
    {
        // Get site user id, if null return
        var siteUserId = await _identityService.GetReferenceSiteUserIdAsync(appUserId, cancellationToken);
        if (siteUserId == null)
        {
            throw new IngredientCreationException("Site user not found.");
        }

        // check if ingredient with same name already exists
        var existingIngredientWithSameName = await _ingredientRepository.CheckExistByNameAsync(request.Name, cancellationToken);
        if (existingIngredientWithSameName == true)
        {
            throw new IngredientCreationException($"Ingredient with name {request.Name} already exists.");
        }


        // Create cover image (not yet saved)
        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        // Process media URLs for details but do not save yet
        var ingredientDetails = request.Details.Select(detail =>
        {
            var mediaUrls = detail.MediaUrls
                .Select(url => new MediaUrl(url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? MediaType.Video : MediaType.Image, url))
                .ToList();

            return new IngredientDetail(new Detail(detail.Title, detail.Description, mediaUrls), detail.Order);
        }).ToList();

        // Create Ingredient (respects constructor signature)
        var ingredient = new Ingredient(
            request.Name,
            request.Description,
            request.CategoryId,
            request.NutritionValues.Calories,
            request.NutritionValues.Protein,
            request.NutritionValues.Carbohydrates,
            request.NutritionValues.Fats,
            request.NutritionValues.Sugars,
            request.NutritionValues.Fiber,
            request.NutritionValues.Sodium,
            request.DietaryInfo.IsVegetarian,
            request.DietaryInfo.IsVegan,
            request.DietaryInfo.IsGlutenFree,
            request.DietaryInfo.IsPescatarian,
            false,
            true,
            coverImage,
            request.ExpirationDays,
            ingredientDetails,
            siteUserId ?? throw new IngredientCreationException("Site user ID is null.")
        );

        var username = await _identityService.GetUserNameFromAppUserIdAsync(appUserId, cancellationToken);

        var createIngredientRequest = new UserRequest(
            UserRequestType.CREATE_INGREDIENT,
            null,
            ingredient.Id,
            string.Empty,
            UserRequestStatus.PENDING,
            siteUserId.Value,
            username == ""
                ? "Unknown User"
                : username
        );

        await _ingredientRepository.AddAsync(ingredient, cancellationToken);
        await _userRequestRepository.AddAsync(createIngredientRequest, cancellationToken);

        // ✅ Commit all at once
        await _unitOfWork.CommitAsync(cancellationToken);
        return createIngredientRequest.Id;
    }

    public async Task<Guid?> CreateIngredientForSeedAsync(Guid appUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken)
    {
        // Get site user id, if null return
        var siteUserId = await _identityService.GetReferenceSiteUserIdAsync(appUserId, cancellationToken);
        if (siteUserId == null)
        {
            throw new IngredientCreationException("Site user not found.");
        }

        // check if ingredient with same name already exists
        var existingIngredientWithSameName = await _ingredientRepository.CheckExistByNameAsync(request.Name, cancellationToken);
        if (existingIngredientWithSameName == true)
        {
            throw new IngredientCreationException($"Ingredient with name {request.Name} already exists.");
        }

        // Create cover image (not yet saved)
        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        // Process media URLs for details but do not save yet
        var ingredientDetails = request.Details.Select(detail =>
        {
            var mediaUrls = detail.MediaUrls
                .Select(url => new MediaUrl(
                    url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? MediaType.Video : MediaType.Image,
                    url))
                .ToList();
            return new IngredientDetail(new Detail(detail.Title, detail.Description, mediaUrls), detail.Order);
        }).ToList();

        // Create Ingredient with Visible = true, Preview = false (as in constructor)
        var ingredient = new Ingredient(
            request.Name,
            request.Description,
            request.CategoryId,
            request.NutritionValues.Calories,
            request.NutritionValues.Protein,
            request.NutritionValues.Carbohydrates,
            request.NutritionValues.Fats,
            request.NutritionValues.Sugars,
            request.NutritionValues.Fiber,
            request.NutritionValues.Sodium,
            request.DietaryInfo.IsVegetarian,
            request.DietaryInfo.IsVegan,
            request.DietaryInfo.IsGlutenFree,
            request.DietaryInfo.IsPescatarian,
            true, // isVisible
            false,  // isPreview
            coverImage,
            request.ExpirationDays,
            ingredientDetails,
            siteUserId.Value
        );

        // Add to DB
        await _ingredientRepository.AddAsync(ingredient, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ingredient.Id;
    }


    public async Task<List<IngredientCategoryDTO>> GetAllIngredientCategoriesAsync(CancellationToken cancellationToken)
    {
        var ingredientCategories = await _ingredientCategoryRepository.GetAllAsync(cancellationToken);
        return ingredientCategories.Select(ingredientCategory => new IngredientCategoryDTO
        {
            Id = ingredientCategory.Id,
            Name = ingredientCategory.Name,
            Description = ingredientCategory.Description
        }).ToList();
    }

    public async Task<IngredientDto> GetEditingIngredientAsync(Guid ingredientId, Guid siteUserId, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(ingredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not found.");
        }
        if (ingredient.CreatedByUser.Id != siteUserId)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not owned by user {siteUserId}.");
        }

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
            Details = ingredient.IngredientDetails.Select(detail => new DetailDto
            {
                Title = detail.Detail.Title,
                Description = detail.Detail.Description,
                MediaUrls = detail.Detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
            }).ToList()
        };
    }

    public async Task<IngredientDto> GetIngredientAsync(Guid id, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(id, cancellationToken);

        if (ingredient == null)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {id} not found.");
        }

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
            Details = ingredient.IngredientDetails.Select(ingredientDetail => new DetailDto
            {
                Title = ingredientDetail.Detail.Title,
                Description = ingredientDetail.Detail.Description,
                MediaUrls = ingredientDetail.Detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = ingredientDetail.Order
            }).ToList()
        };
    }

    public async Task<IngredientDto> GetIngredientOverviewByIdAsync(Guid ingredientId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetIngredientOverviewByIdAsync(ingredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not found.");
        }

        if (!bypassOwnershipCheck && ingredient.CreatedByUser.Id != siteUserId)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not owned by user {siteUserId}.");
        }

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays
        };
    }

    public async Task<IngredientDto> GetIngredientPreviewAsync(Guid ingredientId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(ingredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not found.");
        }

        if (!bypassOwnershipCheck && ingredient.CreatedByUser.Id != siteUserId)
        {
            throw new IngredientRetrievalException($"Ingredient with ID {ingredientId} not owned by user {siteUserId}.");
        }

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
            Details = ingredient.IngredientDetails.Select(ingredientDetail => new DetailDto
            {
                Title = ingredientDetail.Detail.Title,
                Description = ingredientDetail.Detail.Description,
                MediaUrls = ingredientDetail.Detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = ingredientDetail.Order
            }).ToList()
        };
    }


    public async Task<List<IngredientDto>> GetIngredientsByCategoryAsync(string category, int count, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetIngredientOverviewsByCategoryNameAsync(category, count, cancellationToken);
        return ingredients.Select(ingredient => new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
        }).ToList();
    }

    public async Task<List<IngredientDto>> GetIngredientsOverviewAsync(CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetAllIngredientOverviewsAsync(cancellationToken);
        return ingredients.Select(ingredient => new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
        }).ToList();
    }

    public async Task<List<IngredientDto>> GetRandomIngredientsAsync(int count, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetRandomIngredientOverviewsAsync(count, cancellationToken);
        return ingredients.Select(ingredient => new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
        }).ToList();
    }

    public async Task<List<IngredientDto>> GetUserIngredientsAsync(Guid siteUserId, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetAllUserIngreidientOverviewsAsync(siteUserId, cancellationToken);
        return ingredients.Select(ingredient => new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
            CategoryName = ingredient.Category.Name,
            Calories = ingredient.Calories,
            Protein = ingredient.Protein,
            Carbohydrates = ingredient.Carbohydrates,
            Fats = ingredient.Fat,
            Sugars = ingredient.Sugar,
            Fiber = ingredient.Fiber,
            Sodium = ingredient.Sodium,
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            IsGlutenFree = ingredient.IsGlutenFree,
            IsPescatarian = ingredient.IsPescatarian,
            CoverImageUrl = ingredient.CoverImageUrl.Url,
            ExpirationDays = ingredient.ExpirationDays,
        }).ToList();
    }

    public async Task<Guid?> UpdateIngredientAsync(Guid ingredientId, Guid siteUserId, CreateIngredientRequestDto request, CancellationToken cancellationToken)
    {
        // check for existing ingredient
        var ingredient = _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(ingredientId, cancellationToken).Result;
        if (ingredient == null)
        {
            throw new IngredientUpdateException($"Ingredient with ID {ingredientId} not found.");
        }

        // check the ownership
        if (ingredient.CreatedByUser.Id != siteUserId)
        {
            throw new IngredientUpdateException($"Ingredient with ID {ingredientId} not owned by user {siteUserId}.");
        }

        // Create cover image (not yet saved)
        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        // Process media URLs for details but do not save yet
        var ingredientDetails = request.Details.Select(detail =>
        {
            var mediaUrls = detail.MediaUrls
                .Select(url => new MediaUrl(url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? MediaType.Video : MediaType.Image, url))
                .ToList();

            return new IngredientDetail(new Detail(detail.Title, detail.Description, mediaUrls), detail.Order);
        }).ToList();

        // Create Ingredient (respects constructor signature)
        var newIngredient = new Ingredient(
            request.Name,
            request.Description,
            request.CategoryId,
            request.NutritionValues.Calories,
            request.NutritionValues.Protein,
            request.NutritionValues.Carbohydrates,
            request.NutritionValues.Fats,
            request.NutritionValues.Sugars,
            request.NutritionValues.Fiber,
            request.NutritionValues.Sodium,
            request.DietaryInfo.IsVegetarian,
            request.DietaryInfo.IsVegan,
            request.DietaryInfo.IsGlutenFree,
            request.DietaryInfo.IsPescatarian,
            false,
            true,
            coverImage,
            request.ExpirationDays,
            ingredientDetails,
            siteUserId
        );

        // check for existing unapproved update request which have OldReferenceId == ingredientId
        var existingUpdateRequest = await _userRequestRepository.GetPendingByOldReferenceIdAsync(ingredientId, cancellationToken);
        // if exist, delete ingredient record with NewReferenceId == ingredientId and create new ingredient, alter the new reference id of user request to the newly created ingredient
        if (existingUpdateRequest != null)
        {
            var existingIngredientPendingVersion =
                await _ingredientRepository.GetIngredientByIdWithCategoryAndDetailsAsync(
                    existingUpdateRequest.NewReferenceId,
                    cancellationToken
                );

            if (existingIngredientPendingVersion != null)
            {
                await _ingredientRepository.RemoveAsync(existingIngredientPendingVersion, cancellationToken);
            }

            existingUpdateRequest.AlterNewRefId(newIngredient.Id);

            await _ingredientRepository.AddAsync(newIngredient, cancellationToken);
            await _userRequestRepository.UpdateAsync(existingUpdateRequest, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return existingUpdateRequest.Id;
        }
        else
        {
            var username = await _identityService.GetUserNameFromSiteUserIdAsync(siteUserId, cancellationToken);

            var updateIngredientRequest = new UserRequest(
                UserRequestType.UPDATE_INGREDIENT,
                ingredient.Id,
                newIngredient.Id,
                string.Empty,
                UserRequestStatus.PENDING,
                siteUserId,
                username == ""
                    ? "Unknown User"
                    : username
            );

            await _ingredientRepository.AddAsync(newIngredient, cancellationToken);
            await _userRequestRepository.AddAsync(updateIngredientRequest, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return updateIngredientRequest.Id;
        }
    }
}