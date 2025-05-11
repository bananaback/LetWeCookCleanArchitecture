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
}