using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;

namespace LetWeCook.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeTagRepository _recipeTagRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRequestRepository _userRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IRecipeTagRepository recipeTagRepository,
        IIdentityService identityService,
        IUserRepository userRepository,
        IUserRequestRepository userRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _recipeTagRepository = recipeTagRepository;
        _identityService = identityService;
        _userRepository = userRepository;
        _userRequestRepository = userRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateRecipeAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default)
    {
        // Get site user id, if null return
        var siteUserId = await _identityService.GetReferenceSiteUserIdAsync(appUserId, cancellationToken);
        if (siteUserId == null)
        {
            throw new RecipeCreationException("Site user not found.");
        }

        // cast request.Difficulty to DifficultyLevel enum
        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficultyLevel))
        {
            throw new RecipeCreationException($"Invalid difficulty level: {request.Difficulty}");
        }

        // cast request.MealCategory to MealCategory enum
        if (!Enum.TryParse<MealCategory>(request.MealCategory, true, out var mealCategory))
        {
            throw new RecipeCreationException($"Invalid meal category: {request.MealCategory}");
        }

        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        var siteUser = await _userRepository.GetByIdAsync(siteUserId.Value, cancellationToken);

        if (siteUser == null)
        {
            throw new RecipeCreationException("Site user not found.");
        }

        var recipe = new Recipe(
            request.Name,
            request.Description,
            request.Servings,
            request.PrepareTime,
            request.CookTime,
            difficultyLevel,
            mealCategory,
            coverImage,
            siteUser,
            false,
            true);

        var recipeTags = await _recipeTagRepository.GetByNamesAsync(request.Tags, cancellationToken);
        if (recipeTags.Count != request.Tags.Count)
        {
            var missingTags = request.Tags.Except(recipeTags.Select(tag => tag.Name)).ToList();
            throw new RecipeCreationException($"Missing recipe tags: {string.Join(", ", missingTags)}");
        }

        recipe.AddRecipeTags(recipeTags);

        var recipeIngredients = request.Ingredients.Select(ingredient => new RecipeIngredient(
            recipe.Id,
            ingredient.Id,
            ingredient.Quantity,
            Enum.TryParse<UnitEnum>(ingredient.Unit, true, out var unit) ? unit : UnitEnum.Unknown)).ToList();

        recipe.AddIngredients(recipeIngredients);

        var recipeSteps = request.Steps.Select(step => new RecipeDetail(
            new Detail(
                step.Title,
                step.Description,
                step.MediaUrls.Select(url => new MediaUrl(MediaType.Image, url)).ToList()
            ),
            step.Order)
        ).ToList();

        recipe.AddSteps(recipeSteps);

        await _recipeRepository.AddAsync(recipe, cancellationToken);

        var username = await _identityService.GetUserNameFromAppUserIdAsync(appUserId, cancellationToken);

        var createRecipeRequest = new UserRequest(
            UserRequestType.CREATE_RECIPE,
            null,
            recipe.Id,
            string.Empty,
            UserRequestStatus.PENDING,
            siteUserId.Value,
            username == ""
                ? "Unknown User"
                : username
        );

        await _userRequestRepository.AddAsync(createRecipeRequest, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return createRecipeRequest.Id;
    }

    public async Task<List<RecipeTagDto>> GetAllRecipeTagsAsync(CancellationToken cancellationToken = default)
    {
        var recipeTags = await _recipeTagRepository.GetAllAsync(cancellationToken);
        return recipeTags.Select(tag => new RecipeTagDto
        {
            Id = tag.Id,
            Name = tag.Name
        }).ToList();
    }

    public async Task<RecipeDto> GetRecipeOverviewByIdAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetOverviewByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not found.");
        }

        if (!bypassOwnershipCheck && recipe.CreatedBy.Id != siteUserId)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not owned by user {siteUserId}.");
        }

        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            PrepareTime = recipe.PrepareTime,
            CookTime = recipe.CookTime,
            Difficulty = recipe.DifficultyLevel.ToString(),
            MealCategory = recipe.MealCategory.ToString(),
            CoverImage = recipe.CoverMediaUrl.Url,
            CreatedAt = recipe.CreatedAt,
            AverageRating = recipe.AverageRating,
            TotalRatings = recipe.TotalRatings,
            TotalViews = recipe.TotalViews,
        };
    }

    public async Task<RecipeDto> GetRecipePreviewAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not found.");
        }

        if (!bypassOwnershipCheck && recipe.CreatedBy.Id != siteUserId)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not owned by user {siteUserId}.");
        }

        var authorProfile = new AuthorProfileDto();

        // check if profie is null then populate to author profile
        if (recipe.CreatedBy.Profile != null)
        {
            authorProfile = new AuthorProfileDto
            {
                Id = recipe.CreatedBy.Profile.Id,
                Name = recipe.CreatedBy.Profile.Name.FullName,
                ProfilePicUrl = recipe.CreatedBy.Profile.ProfilePic,
                Bio = recipe.CreatedBy.Profile.Bio,
                Facebook = recipe.CreatedBy.Profile.Facebook,
                Instagram = recipe.CreatedBy.Profile.Instagram
            };
        }

        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            PrepareTime = recipe.PrepareTime,
            CookTime = recipe.CookTime,
            Difficulty = recipe.DifficultyLevel.ToString(),
            MealCategory = recipe.MealCategory.ToString(),
            CoverImage = recipe.CoverMediaUrl.Url,
            CreatedAt = recipe.CreatedAt,
            AuthorProfile = authorProfile,
            AverageRating = recipe.AverageRating,
            TotalRatings = recipe.TotalRatings,
            TotalViews = recipe.TotalViews,
            Ingredients = recipe.RecipeIngredients.Select(ingredient => new RecipeIngredientDto
            {
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Ingredient.Name,
                CoverImageUrl = ingredient.Ingredient.CoverImageUrl.Url,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit.ToString()
            }).ToList(),
            Steps = recipe.RecipeDetails.Select(step => new RecipeStepDto
            {
                Title = step.Detail.Title,
                Description = step.Detail.Description,
                MediaUrls = step.Detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = step.Order
            }).ToList(),
            Tags = recipe.Tags.Select(tag => tag.Name).ToList(),
        };
    }
}