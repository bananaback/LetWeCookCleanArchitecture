using LetWeCook.Application.Dtos.RecipeCollections;
using LetWeCook.Application.DTOs;
using LetWeCook.Application.DTOs.RecipeCollections;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Utilities;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Services;

public class RecipeCollectionService : ICollectionService
{
    private readonly IRecipeCollectionRepository _recipeCollectionRepository;
    private readonly IRecipeCollectionItemRepository _recipeCollectionItemRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeCollectionService(IRecipeCollectionRepository recipeCollectionRepository, IRecipeCollectionItemRepository recipeCollectionItemRepository, IRecipeRepository recipeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _recipeCollectionRepository = recipeCollectionRepository;
        _recipeCollectionItemRepository = recipeCollectionItemRepository;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddRecipeToCollectionAsync(Guid siteUserId, Guid recipeId, Guid collectionId, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionUpdateException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }
        // check if recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            var ex = new RecipeCollectionUpdateException("Recipe not found: " + recipeId, ErrorCode.RECIPE_COLLECTION_RECIPE_NOT_FOUND);
            ex.AddContext("RecipeId", recipeId.ToString());
            throw ex;
        }
        // check if collection exists
        var collection = await _recipeCollectionRepository.GetWithOwnerByIdAsync(collectionId, cancellationToken);
        if (collection == null)
        {
            var ex = new RecipeCollectionUpdateException("Collection not found: " + collectionId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("CollectionId", collectionId.ToString());
            throw ex;
        }

        // check if collection belongs to user
        if (collection.CreatedBy.Id != siteUserId)
        {
            var ex = new RecipeCollectionUpdateException("Collection does not belong to user: " + siteUserId, ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // check if recipe is already in collection
        if (collection.Recipes.Any(r => r.RecipeId == recipeId))
        {
            // short circuit only, resilient
            return;
        }

        // add recipe to collection
        collection.AddRecipe(recipe);

        await _recipeCollectionRepository.UpdateAsync(collection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task AddRecipeToNewCollectionAsync(Guid siteUserId, Guid recipeId, string collectionName, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionUpdateException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // check if recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            var ex = new RecipeCollectionUpdateException("Recipe not found: " + recipeId, ErrorCode.RECIPE_COLLECTION_RECIPE_NOT_FOUND);
            ex.AddContext("RecipeId", recipeId.ToString());
            throw ex;
        }

        // check if collection name is valid
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            var ex = new RecipeCollectionUpdateException("Collection name cannot be empty.", ErrorCode.RECIPE_COLLECTION_NAME_EMPTY);
            ex.AddContext("CollectionName", collectionName);
            throw ex;
        }
        if (collectionName.Length > 100)
        {
            var ex = new RecipeCollectionUpdateException("Collection name cannot exceed 100 characters.", ErrorCode.RECIPE_COLLECTION_NAME_TOO_LONG);
            ex.AddContext("CollectionName", collectionName);
            throw ex;
        }
        if (collectionName.Length < 3)
        {
            var ex = new RecipeCollectionUpdateException("Collection name must be at least 3 characters long.", ErrorCode.RECIPE_COLLECTION_NAME_TOO_SHORT);
            ex.AddContext("CollectionName", collectionName);
            throw ex;
        }
        // check if collection already exists with the same name
        bool collectionExists = await _recipeCollectionRepository.ExistsByNameAsync(siteUserId, collectionName, cancellationToken);
        if (collectionExists)
        {
            var ex = new RecipeCollectionUpdateException("Collection with the same name already exists.", ErrorCode.RECIPE_COLLECTION_NAME_EXISTS);
            ex.AddContext("CollectionName", collectionName);
            throw ex;
        }

        // create new collection
        var collection = new RecipeCollection(collectionName, user);
        collection.AddRecipe(recipe);
        await _recipeCollectionRepository.AddAsync(collection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<PaginatedResult<RecipeCollectionDto>> BrowseCollection(Guid siteUserId, RecipeCollectionQueryRequestDto request, CancellationToken cancellationToken = default)
    {
        // get all collection names for the user
        var collectionNames = await _recipeCollectionRepository.GetCollectionNamesAsync(siteUserId, cancellationToken);

        var matchedNames = new List<string>();
        if (request.SearchTerm == string.Empty)
        {
            matchedNames = collectionNames;
        }
        else
        {
            matchedNames = FuzzySearchUtil.FindTopMatches(request.SearchTerm, collectionNames, 10, 2);
        }

        var totalItems = matchedNames.Count;

        var recipeCollections = await _recipeCollectionRepository.GetRecipeCollectionByNamesAsync(
            siteUserId,
            matchedNames,
            cancellationToken);

        if (request.SortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
        {
            recipeCollections = request.IsAscending
                ? recipeCollections.OrderBy(c => c.CreatedAt).ToList()
                : recipeCollections.OrderByDescending(c => c.CreatedAt).ToList();
        }

        // sort the matched names based on the request
        if (request.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
        {
            recipeCollections = request.IsAscending
                ? recipeCollections.OrderBy(c => c.Name).ToList()
                : recipeCollections.OrderByDescending(c => c.Name).ToList();
        }

        // paginate the results
        var paginatedCollections = recipeCollections
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new RecipeCollectionDto
            {
                CollectionId = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                Items = c.Recipes.Select(i => new RecipeCollectionItemDto
                {
                    RecipeId = i.RecipeId,
                    CollectionId = i.CollectionId,
                    RecipeName = i.Recipe.Name,
                    ImageUrl = i.Recipe.CoverMediaUrl.Url,
                    CreatedAt = i.AddedAt
                }).ToList()
            })
            .ToList();

        return new PaginatedResult<RecipeCollectionDto>
        (
            paginatedCollections,
            totalItems,
            request.PageNumber,
            request.PageSize
        );
    }

    public async Task<PaginatedResult<RecipeCollectionItemDto>> BrowseCollectionItems(Guid siteUserId, RecipeCollectionItemQueryRequestDto request, CancellationToken cancellationToken = default)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionRetrievalException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }
        // check if collection exists
        var collection = await _recipeCollectionRepository.GetWithOwnerByIdAsync(request.CollectionId, cancellationToken);
        if (collection == null)
        {
            var ex = new RecipeCollectionRetrievalException("Collection not found: " + request.CollectionId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("CollectionId", request.CollectionId.ToString());
            throw ex;
        }
        // check if collection belongs to user
        if (collection.CreatedBy != null && collection.CreatedBy.Id != siteUserId)
        {
            var ex = new RecipeCollectionRetrievalException("Collection does not belong to user: " + siteUserId, ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            ex.AddContext("CollectionId", request.CollectionId.ToString());
            throw ex;
        }

        // get all recipe names for the user
        var recipeNames = await _recipeCollectionItemRepository.GetRecipeNamesAsync(request.CollectionId, cancellationToken);

        var matchedNames = new List<string>();
        if (request.SearchTerm == string.Empty)
        {
            matchedNames = recipeNames;
        }
        else
        {
            matchedNames = FuzzySearchUtil.FindTopMatches(request.SearchTerm, recipeNames, 10, 1, 2);
        }

        var totalItems = matchedNames.Count;

        var recipeCollectionItems = await _recipeCollectionItemRepository.GetItemsByNamesAsync(
            request.CollectionId,
            matchedNames,
            cancellationToken);

        if (request.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
        {
            recipeCollectionItems = request.IsAscending
                    ? recipeCollectionItems.OrderBy(c => c.AddedAt).ToList()
                    : recipeCollectionItems.OrderByDescending(c => c.AddedAt).ToList();
        }

        // sort the matched names based on the request
        if (request.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
        {
            recipeCollectionItems = request.IsAscending
                ? recipeCollectionItems.OrderBy(c => c.Recipe.Name).ToList()
                : recipeCollectionItems.OrderByDescending(c => c.Recipe.Name).ToList();
        }

        // paginate the results
        var paginatedCollectionItems = recipeCollectionItems
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new RecipeCollectionItemDto
            {
                CollectionId = i.CollectionId,
                CollectionName = collection.Name,
                RecipeId = i.RecipeId,
                CreatedAt = i.AddedAt,
                RecipeName = i.Recipe.Name,
                ImageUrl = i.Recipe.CoverMediaUrl.Url
            })
            .ToList();

        return new PaginatedResult<RecipeCollectionItemDto>
        (
            paginatedCollectionItems,
            totalItems,
            request.PageNumber,
            request.PageSize
        );
    }

    public async Task DeleteCollectionAsync(Guid siteUserId, Guid collectionId, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionUpdateException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // check if collection exists
        var collection = await _recipeCollectionRepository.GetWithOwnerByIdAsync(collectionId, cancellationToken);
        if (collection == null)
        {
            var ex = new RecipeCollectionUpdateException("Collection not found: " + collectionId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("CollectionId", collectionId.ToString());
            throw ex;
        }

        // check if collection belongs to user
        if (collection.CreatedBy.Id != siteUserId)
        {
            var ex = new RecipeCollectionUpdateException("Collection does not belong to user: " + siteUserId, ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // delete collection
        await _recipeCollectionRepository.RemoveAsync(collection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<List<RecipeCollectionDto>> GetRecipeCollectionsAsync(Guid siteUserId, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionRetrievalException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        var collections = await _recipeCollectionRepository.GetAllUserCollectionsAsync(siteUserId, cancellationToken);
        return collections.Select(c => new RecipeCollectionDto
        {
            CollectionId = c.Id,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            Items = c.Recipes.Select(i => new RecipeCollectionItemDto
            {
                RecipeId = i.RecipeId,
                CollectionId = i.CollectionId,
                RecipeName = i.Recipe.Name,
                ImageUrl = i.Recipe.CoverMediaUrl.Url,
                CreatedAt = i.AddedAt
            }).ToList()
        }).ToList();
    }

    public async Task RemoveRecipeFromCollectionAsync(Guid siteUserId, Guid recipeId, Guid collectionId, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionUpdateException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }
        // check if recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            var ex = new RecipeCollectionUpdateException("Recipe not found: " + recipeId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("RecipeId", recipeId.ToString());
            throw ex;
        }
        // check if collection exists
        var collection = await _recipeCollectionRepository.GetWithOwnerByIdAsync(collectionId, cancellationToken);
        if (collection == null)
        {
            var ex = new RecipeCollectionUpdateException("Collection not found: " + collectionId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("CollectionId", collectionId.ToString());
            throw ex;
        }
        // check if collection belongs to user
        if (collection.CreatedBy.Id != siteUserId)
        {
            var ex = new RecipeCollectionUpdateException("Collection does not belong to user: " + siteUserId, ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }
        // check if recipe is in collection
        var collectionItem = collection.Recipes.FirstOrDefault(i => i.RecipeId == recipeId);
        if (collectionItem == null)
        {
            var ex = new RecipeCollectionUpdateException("Recipe is not in collection: " + recipeId, ErrorCode.RECIPE_COLLECTION_RECIPE_NOT_FOUND);
            ex.AddContext("RecipeId", recipeId.ToString());
            throw ex;
        }
        // remove recipe from collection
        collection.RemoveRecipe(recipe);
        await _recipeCollectionRepository.UpdateAsync(collection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

    }

    public async Task UpdateCollectionNameAsync(Guid siteUserId, Guid collectionId, string newName, CancellationToken cancellationToken)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            var ex = new RecipeCollectionUpdateException("User not found: " + siteUserId, ErrorCode.RECIPE_COLLECTION_USER_NOT_FOUND);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // check if collection exists
        var collection = await _recipeCollectionRepository.GetWithOwnerByIdAsync(collectionId, cancellationToken);
        if (collection == null)
        {
            var ex = new RecipeCollectionUpdateException("Collection not found: " + collectionId, ErrorCode.RECIPE_COLLECTION_NOT_FOUND);
            ex.AddContext("CollectionId", collectionId.ToString());
            throw ex;
        }

        // check if collection belongs to user
        if (collection.CreatedBy.Id != siteUserId)
        {
            var ex = new RecipeCollectionUpdateException("Collection does not belong to user: " + siteUserId, ErrorCode.RECIPE_COLLECTION_NOT_BELONG_TO_USER);
            ex.AddContext("SiteUserId", siteUserId.ToString());
            throw ex;
        }

        // update collection name
        collection.UpdateName(newName);
        await _recipeCollectionRepository.UpdateAsync(collection, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

}