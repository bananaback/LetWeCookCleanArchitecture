using LetWeCook.Application.Dtos.Recipe;
using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Enums;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Recipes;
using LetWeCook.Application.Recipes.Filters;
using LetWeCook.Application.Recipes.Sorts;
using LetWeCook.Application.Recipes.Specifications;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeTagRepository _recipeTagRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRequestRepository _userRequestRepository;
    private readonly IUserInteractionRepository _userInteractionRepository;
    private readonly IDetailRepository _detailRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IRecipeTagRepository recipeTagRepository,
        IIdentityService identityService,
        IUserRepository userRepository,
        IUserRequestRepository userRequestRepository,
        IUserInteractionRepository userInteractionRepository,
        IUnitOfWork unitOfWork,
        IDetailRepository detailRepository,
        IIngredientRepository ingredientRepository)
    {
        _recipeRepository = recipeRepository;
        _recipeTagRepository = recipeTagRepository;
        _identityService = identityService;
        _userRepository = userRepository;
        _userRequestRepository = userRequestRepository;
        _userInteractionRepository = userInteractionRepository;
        _unitOfWork = unitOfWork;
        _detailRepository = detailRepository;
        _ingredientRepository = ingredientRepository;
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

    public async Task<Guid> AcceptRecipeAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default)
    {
        // Get site user id, if null return
        var siteUserId = await _identityService.GetReferenceSiteUserIdAsync(appUserId, cancellationToken);
        if (siteUserId == null)
        {
            throw new RecipeCreationException("Site user not found.");
        }

        // Cast request.Difficulty to DifficultyLevel enum
        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficultyLevel))
        {
            throw new RecipeCreationException($"Invalid difficulty level: {request.Difficulty}");
        }

        // Cast request.MealCategory to MealCategory enum
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
            true, // IsPending set to true
            false // IsDraft set to false
        );

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

        await _unitOfWork.CommitAsync(cancellationToken);

        return recipe.Id;
    }

    public async Task<Guid> CreateRecipeForSeedAsync(Guid appUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default)
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
            true,
            false);

        // random the view and set for recipe
        var random = new Random();
        var randomView = random.Next(100, 1000);
        recipe.SetViewForSeeding(randomView);


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

        await _unitOfWork.CommitAsync(cancellationToken);

        return recipe.Id;
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
                Instagram = recipe.CreatedBy.Profile.Instagram,
                PayPalEmail = recipe.CreatedBy.Profile.PayPalEmail
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
                IngredientId = ingredient.Ingredient.Id,
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

    public async Task<RecipeDto> GetRecipeDetailsAsync(Guid recipeId, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not found.");
        }

        recipe.IncreaseView();
        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var authorProfile = new AuthorProfileDto();

        // Populate author profile if exists
        if (recipe.CreatedBy.Profile != null)
        {
            authorProfile = new AuthorProfileDto
            {
                Id = recipe.CreatedBy.Profile.Id,
                Name = recipe.CreatedBy.Profile.Name.FullName,
                ProfilePicUrl = recipe.CreatedBy.Profile.ProfilePic,
                Bio = recipe.CreatedBy.Profile.Bio,
                Facebook = recipe.CreatedBy.Profile.Facebook,
                Instagram = recipe.CreatedBy.Profile.Instagram,
                PayPalEmail = recipe.CreatedBy.Profile.PayPalEmail
            };
        }

        // check if recipe is visible
        if (!recipe.IsVisible)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} is not visible.");
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
                IngredientId = ingredient.Ingredient.Id,
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

    public async Task<RecipeDto> GetRecipeDetailsWithTrackingAsync(Guid recipeId, Guid? siteUserId, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} not found.");
        }

        recipe.IncreaseView();
        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var authorProfile = new AuthorProfileDto();

        // Populate author profile if exists
        if (recipe.CreatedBy.Profile != null)
        {
            authorProfile = new AuthorProfileDto
            {
                Id = recipe.CreatedBy.Profile.Id,
                Name = recipe.CreatedBy.Profile.Name.FullName,
                ProfilePicUrl = recipe.CreatedBy.Profile.ProfilePic,
                Bio = recipe.CreatedBy.Profile.Bio,
                Facebook = recipe.CreatedBy.Profile.Facebook,
                Instagram = recipe.CreatedBy.Profile.Instagram,
                PayPalEmail = recipe.CreatedBy.Profile.PayPalEmail
            };
        }

        // check if recipe is visible
        if (!recipe.IsVisible)
        {
            throw new RecipeRetrievalException($"Recipe with ID {recipeId} is not visible.");
        }

        if (siteUserId.HasValue)
        {
            var interaction = new UserInteraction
            (
                recipe.Id,
                siteUserId.Value,
                "view",
                1
            );
            await _userInteractionRepository.AddAsync(interaction, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
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
                IngredientId = ingredient.Ingredient.Id,
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


    public async Task<PaginatedResult<RecipeDto>> GetRecipes(RecipeQueryOptions recipeQueryOptions, CancellationToken cancellationToken = default)
    {
        // create new specification for recipe query options
        var recipeSpecification = new RecipeSpecification();

        // check name search option
        if (!string.IsNullOrEmpty(recipeQueryOptions.Name) && !string.IsNullOrEmpty(recipeQueryOptions.NameMatchMode))
        {
            // switch case the string to enum
            if (!Enum.TryParse<TextMatchMode>(recipeQueryOptions.NameMatchMode, true, out var nameMatchMode))
            {
                throw new ArgumentException($"Invalid name match mode: {recipeQueryOptions.NameMatchMode}");
            }
            // add name filter to specification
            recipeSpecification.AddFilter(new RecipeNameFilter(recipeQueryOptions.Name, nameMatchMode));
        }

        // check difficulties
        if (recipeQueryOptions.Difficulties.Count > 0)
        {
            // convert difficulties to enum
            var difficulties = recipeQueryOptions.Difficulties.Select(difficulty => Enum.TryParse<DifficultyLevel>(difficulty, true, out var difficultyLevel) ? difficultyLevel : throw new ArgumentException($"Invalid difficulty level: {difficulty}")).ToList();
            recipeSpecification.AddFilter(new RecipeDifficultyFilter(difficulties));
        }

        // check meal categories
        if (recipeQueryOptions.MealCategories.Count > 0)
        {
            // convert meal categories to enum
            var mealCategories = recipeQueryOptions.MealCategories.Select(mealCategory => Enum.TryParse<MealCategory>(mealCategory, true, out var mealCategoryEnum) ? mealCategoryEnum : throw new ArgumentException($"Invalid meal category: {mealCategory}")).ToList();
            recipeSpecification.AddFilter(new RecipeMealCategoryFilter(mealCategories));
        }

        recipeSpecification
            .AddFilter(new RecipeServingsFilter(recipeQueryOptions.MinServings, recipeQueryOptions.MaxServings))
            .AddFilter(new RecipePrepareTimeFilter(recipeQueryOptions.MinPrepareTime, recipeQueryOptions.MaxPrepareTime))
            .AddFilter(new RecipeCookTimeFilter(recipeQueryOptions.MinCookTime, recipeQueryOptions.MaxCookTime))
            .AddFilter(new RecipeRatingFilter(recipeQueryOptions.MinAverageRating, recipeQueryOptions.MaxAverageRating))
            .AddFilter(new RecipeViewsFilter(recipeQueryOptions.MinTotalViews, recipeQueryOptions.MaxTotalViews))
            .AddFilter(new RecipeCreatedAtFilter(recipeQueryOptions.CreatedFrom, recipeQueryOptions.CreatedTo))
            .AddFilter(new RecipeUpdatedAtFilter(recipeQueryOptions.UpdatedFrom, recipeQueryOptions.UpdatedTo));

        // fetch recipe tags by names
        var recipeTags = await _recipeTagRepository.GetByNamesAsync(recipeQueryOptions.Tags, cancellationToken);
        if (recipeTags.Count != recipeQueryOptions.Tags.Count)
        {
            var missingTags = recipeQueryOptions.Tags.Except(recipeTags.Select(tag => tag.Name)).ToList();
            throw new RecipeRetrievalException($"Missing recipe tags: {string.Join(", ", missingTags)}");
        }

        recipeSpecification.AddFilter(new RecipeTagsFilter(recipeTags));

        var query = _recipeRepository.GetQueryable(cancellationToken);

        // add sorting to specification
        foreach (var sortOption in recipeQueryOptions.SortOptions)
        {
            var criteria = sortOption.Criteria.ToLower();
            var direction = sortOption.Direction.ToLower() == "descending" ? SortDirection.Desc : SortDirection.Asc;
            // switch case the string to enum
            switch (criteria)
            {
                case "name":
                    recipeSpecification.AddSort(new RecipeNameSortFilter(direction));
                    break;
                case "servings":
                    recipeSpecification.AddSort(new RecipeServingsSortFilter(direction));
                    break;
                case "prepare-time":
                    recipeSpecification.AddSort(new RecipePrepareTimeSortFilter(direction));
                    break;
                case "cook-time":
                    recipeSpecification.AddSort(new RecipeCookTimeSortFilter(direction));
                    break;
                case "difficulty-level":
                    recipeSpecification.AddSort(new RecipeDifficultySortFilter(direction));
                    break;
                case "rating":
                    recipeSpecification.AddSort(new RecipeRatingSortFilter(direction));
                    break;
                case "views":
                    recipeSpecification.AddSort(new RecipeViewsSortFilter(direction));
                    break;
                case "created-at":
                    recipeSpecification.AddSort(new RecipeCreatedAtSortFilter(direction));
                    break;
                default:
                    throw new ArgumentException($"Invalid sort option: {sortOption}");
            }
        }


        // apply filters and sorts to query
        query = recipeSpecification.Apply(query);

        var totalCount = await _recipeRepository.CountAsync(query, cancellationToken);


        // apply pagination to query
        var paginatedQuery = recipeSpecification.ApplyPagination(query, recipeQueryOptions.PageNumber, recipeQueryOptions.PageSize);

        var recipes = await _recipeRepository.QueryableToListAsync(paginatedQuery, cancellationToken);

        return new PaginatedResult<RecipeDto>(
            recipes.Select(recipe => new RecipeDto
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
            }),
            totalCount,
            recipeQueryOptions.PageNumber,
            recipeQueryOptions.PageSize
        );
    }

    public async Task<PaginatedResult<RecipeDto>> GetPersonalRecipes(Guid siteUserId, RecipeQueryOptions recipeQueryOptions, CancellationToken cancellationToken = default)
    {
        // create new specification for recipe query options
        var recipeSpecification = new RecipeSpecification();

        recipeSpecification.AddFilter(new RecipeCreatedByUserIdFilter(siteUserId));

        // check name search option
        if (!string.IsNullOrEmpty(recipeQueryOptions.Name) && !string.IsNullOrEmpty(recipeQueryOptions.NameMatchMode))
        {
            // switch case the string to enum
            if (!Enum.TryParse<TextMatchMode>(recipeQueryOptions.NameMatchMode, true, out var nameMatchMode))
            {
                throw new ArgumentException($"Invalid name match mode: {recipeQueryOptions.NameMatchMode}");
            }
            // add name filter to specification
            recipeSpecification.AddFilter(new RecipeNameFilter(recipeQueryOptions.Name, nameMatchMode));
        }

        // check difficulties
        if (recipeQueryOptions.Difficulties.Count > 0)
        {
            // convert difficulties to enum
            var difficulties = recipeQueryOptions.Difficulties.Select(difficulty => Enum.TryParse<DifficultyLevel>(difficulty, true, out var difficultyLevel) ? difficultyLevel : throw new ArgumentException($"Invalid difficulty level: {difficulty}")).ToList();
            recipeSpecification.AddFilter(new RecipeDifficultyFilter(difficulties));
        }

        // check meal categories
        if (recipeQueryOptions.MealCategories.Count > 0)
        {
            // convert meal categories to enum
            var mealCategories = recipeQueryOptions.MealCategories.Select(mealCategory => Enum.TryParse<MealCategory>(mealCategory, true, out var mealCategoryEnum) ? mealCategoryEnum : throw new ArgumentException($"Invalid meal category: {mealCategory}")).ToList();
            recipeSpecification.AddFilter(new RecipeMealCategoryFilter(mealCategories));
        }

        recipeSpecification
            .AddFilter(new RecipeServingsFilter(recipeQueryOptions.MinServings, recipeQueryOptions.MaxServings))
            .AddFilter(new RecipePrepareTimeFilter(recipeQueryOptions.MinPrepareTime, recipeQueryOptions.MaxPrepareTime))
            .AddFilter(new RecipeCookTimeFilter(recipeQueryOptions.MinCookTime, recipeQueryOptions.MaxCookTime))
            .AddFilter(new RecipeRatingFilter(recipeQueryOptions.MinAverageRating, recipeQueryOptions.MaxAverageRating))
            .AddFilter(new RecipeViewsFilter(recipeQueryOptions.MinTotalViews, recipeQueryOptions.MaxTotalViews))
            .AddFilter(new RecipeCreatedAtFilter(recipeQueryOptions.CreatedFrom, recipeQueryOptions.CreatedTo))
            .AddFilter(new RecipeUpdatedAtFilter(recipeQueryOptions.UpdatedFrom, recipeQueryOptions.UpdatedTo));

        // fetch recipe tags by names
        var recipeTags = await _recipeTagRepository.GetByNamesAsync(recipeQueryOptions.Tags, cancellationToken);
        if (recipeTags.Count != recipeQueryOptions.Tags.Count)
        {
            var missingTags = recipeQueryOptions.Tags.Except(recipeTags.Select(tag => tag.Name)).ToList();
            throw new RecipeRetrievalException($"Missing recipe tags: {string.Join(", ", missingTags)}");
        }

        recipeSpecification.AddFilter(new RecipeTagsFilter(recipeTags));

        var query = _recipeRepository.GetQueryable(cancellationToken);

        // add sorting to specification
        foreach (var sortOption in recipeQueryOptions.SortOptions)
        {
            var criteria = sortOption.Criteria.ToLower();
            var direction = sortOption.Direction.ToLower() == "descending" ? SortDirection.Desc : SortDirection.Asc;
            // switch case the string to enum
            switch (criteria)
            {
                case "name":
                    recipeSpecification.AddSort(new RecipeNameSortFilter(direction));
                    break;
                case "servings":
                    recipeSpecification.AddSort(new RecipeServingsSortFilter(direction));
                    break;
                case "prepare-time":
                    recipeSpecification.AddSort(new RecipePrepareTimeSortFilter(direction));
                    break;
                case "cook-time":
                    recipeSpecification.AddSort(new RecipeCookTimeSortFilter(direction));
                    break;
                case "difficulty-level":
                    recipeSpecification.AddSort(new RecipeDifficultySortFilter(direction));
                    break;
                case "rating":
                    recipeSpecification.AddSort(new RecipeRatingSortFilter(direction));
                    break;
                case "views":
                    recipeSpecification.AddSort(new RecipeViewsSortFilter(direction));
                    break;
                case "created-at":
                    recipeSpecification.AddSort(new RecipeCreatedAtSortFilter(direction));
                    break;
                default:
                    throw new ArgumentException($"Invalid sort option: {sortOption}");
            }
        }


        // apply filters and sorts to query
        query = recipeSpecification.Apply(query);

        var totalCount = await _recipeRepository.CountAsync(query, cancellationToken);


        // apply pagination to query
        var paginatedQuery = recipeSpecification.ApplyPagination(query, recipeQueryOptions.PageNumber, recipeQueryOptions.PageSize);

        var recipes = await _recipeRepository.QueryableToListAsync(paginatedQuery, cancellationToken);

        return new PaginatedResult<RecipeDto>(
            recipes.Select(recipe => new RecipeDto
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
            }),
            totalCount,
            recipeQueryOptions.PageNumber,
            recipeQueryOptions.PageSize
        );
    }

    public async Task<Guid?> UpdateRecipeAsync(Guid recipeId, Guid siteUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default)
    {
        // check for existing recipe
        var existingRecipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (existingRecipe == null)
        {
            throw new RecipeUpdateException($"Recipe with ID {recipeId} not found.");
        }

        // check for ownership
        if (existingRecipe.CreatedBy.Id != siteUserId)
        {
            throw new RecipeUpdateException($"Recipe with ID {recipeId} not owned by user {siteUserId}.");
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

        var siteUser = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);

        if (siteUser == null)
        {
            throw new RecipeCreationException("Site user not found.");
        }

        var newRecipe = new Recipe(
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

        newRecipe.AddRecipeTags(recipeTags);

        var recipeIngredients = request.Ingredients.Select(ingredient => new RecipeIngredient(
            newRecipe.Id,
            ingredient.Id,
            ingredient.Quantity,
            Enum.TryParse<UnitEnum>(ingredient.Unit, true, out var unit) ? unit : UnitEnum.Unknown)).ToList();

        newRecipe.AddIngredients(recipeIngredients);

        var recipeSteps = request.Steps.Select(step => new RecipeDetail(
            new Detail(
                step.Title,
                step.Description,
                step.MediaUrls.Select(url => new MediaUrl(MediaType.Image, url)).ToList()
            ),
            step.Order)
        ).ToList();

        newRecipe.AddSteps(recipeSteps);

        var existingUpdateRequest = await _userRequestRepository.GetPendingByOldReferenceIdAsync(recipeId, cancellationToken);

        // if exist, delete recipe record with NewReferenceId == recipeId and create new recipe, alter the new reference id of user request to the newly created recipe
        if (existingUpdateRequest != null)
        {
            var existingRecipePendingVersion =
                await _recipeRepository.GetRecipeDetailsByIdAsync(
                    existingUpdateRequest.NewReferenceId,
                    cancellationToken
                );

            if (existingRecipePendingVersion != null)
            {
                await _recipeRepository.RemoveAsync(existingRecipePendingVersion, cancellationToken);
            }
            existingUpdateRequest.AlterNewRefId(newRecipe.Id);

            await _recipeRepository.AddAsync(newRecipe, cancellationToken);
            await _userRequestRepository.UpdateAsync(existingUpdateRequest, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return existingUpdateRequest.Id;
        }
        else
        {
            var username = await _identityService.GetUserNameFromSiteUserIdAsync(siteUserId, cancellationToken);

            var updateRecipeRequest = new UserRequest(
                UserRequestType.UPDATE_RECIPE,
                existingRecipe.Id,
                newRecipe.Id,
                string.Empty,
                UserRequestStatus.PENDING,
                siteUserId,
                username == ""
                    ? "Unknown User"
                    : username
            );

            await _recipeRepository.AddAsync(newRecipe, cancellationToken);
            await _userRequestRepository.AddAsync(updateRecipeRequest, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return updateRecipeRequest.Id;
        }
    }

    public async Task<Guid> AcceptUpdateRecipeAsync(Guid recipeId, Guid siteUserId, CreateRecipeRequestDto request, CancellationToken cancellationToken = default)
    {
        // Check for existing recipe
        var existingRecipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (existingRecipe == null)
            throw new RecipeUpdateException($"Recipe with ID {recipeId} not found.");

        // Check for ownership
        if (existingRecipe.CreatedBy.Id != siteUserId)
            throw new RecipeUpdateException($"Recipe with ID {recipeId} not owned by user {siteUserId}.");

        // Parse enums
        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficultyLevel))
            throw new RecipeCreationException($"Invalid difficulty level: {request.Difficulty}");

        if (!Enum.TryParse<MealCategory>(request.MealCategory, true, out var mealCategory))
            throw new RecipeCreationException($"Invalid meal category: {request.MealCategory}");

        var siteUser = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (siteUser == null)
            throw new RecipeCreationException("Site user not found.");

        // Parse Cover Image
        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        // Copy flat properties
        existingRecipe.UpdateProperties(
            name: request.Name,
            description: request.Description,
            servings: request.Servings,
            prepareTime: request.PrepareTime,
            cookTime: request.CookTime,
            difficultyLevel: difficultyLevel,
            mealCategory: mealCategory,
            coverMediaUrl: coverImage,
            isVisible: true,
            isPreview: false
        );

        // Update Tags
        var recipeTags = await _recipeTagRepository.GetByNamesAsync(request.Tags, cancellationToken);
        if (recipeTags.Count != request.Tags.Count)
        {
            var missingTags = request.Tags.Except(recipeTags.Select(t => t.Name)).ToList();
            throw new RecipeCreationException($"Missing recipe tags: {string.Join(", ", missingTags)}");
        }
        existingRecipe.Tags.Clear();
        existingRecipe.AddRecipeTags(recipeTags);

        // Update Ingredients
        existingRecipe.RecipeIngredients.Clear();
        foreach (var ing in request.Ingredients)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(ing.Id, cancellationToken);
            if (ingredient == null)
            {
                var ex = new IngredientRetrievalException($"Ingredient with ID {ing.Id} not found.", ErrorCode.INGREDIENT_NOT_FOUND);
                ex.AddContext("IngredientId", ing.Id.ToString());
                throw ex;
            }

            if (!Enum.TryParse<UnitEnum>(ing.Unit, true, out var unit))
                throw new RecipeCreationException($"Invalid unit: {ing.Unit}");

            var recipeIngredient = new RecipeIngredient(existingRecipe.Id, ingredient.Id, ing.Quantity, unit);
            existingRecipe.AddIngredient(recipeIngredient);
        }

        // Update Details (Steps)
        existingRecipe.RecipeDetails.Clear();
        foreach (var step in request.Steps)
        {
            var mediaUrls = step.MediaUrls.Select(url => new MediaUrl(MediaType.Image, url)).ToList();
            var detail = new Detail(step.Title, step.Description, mediaUrls);

            await _detailRepository.AddAsync(detail, cancellationToken);
            var recipeDetail = new RecipeDetail(detail, step.Order);
            existingRecipe.AddStep(recipeDetail);
        }

        // Save all
        await _recipeRepository.UpdateAsync(existingRecipe, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return existingRecipe.Id;
    }


    public async Task DeleteRecipeAsync(Guid recipeId, Guid siteUserId, bool bypassOwnershipCheck, CancellationToken cancellationToken = default)
    {
        // check for existing recipe
        var existingRecipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (existingRecipe == null)
        {
            throw new RecipeDeletionException($"Recipe with ID {recipeId} not found.");
        }

        // check for ownership
        if (!bypassOwnershipCheck && existingRecipe.CreatedBy.Id != siteUserId)
        {
            throw new RecipeDeletionException($"Recipe with ID {recipeId} not owned by user {siteUserId}.");
        }

        var recipe = await _recipeRepository.GetRecipeDetailsByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new RecipeDeletionException($"Recipe with ID {recipeId} not found.");
        }

        recipe.SetVisible(false);

        await _recipeRepository.UpdateAsync(recipe, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<List<RecipeDto>> GetRandomRecipesAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        var recipes = await _recipeRepository.GetRandomRecipesAsync(count, cancellationToken);
        if (recipes == null || recipes.Count == 0)
        {
            return new List<RecipeDto>();
        }

        return recipes.Select(recipe => new RecipeDto
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
            TotalViews = recipe.TotalViews
        }).ToList();
    }
}