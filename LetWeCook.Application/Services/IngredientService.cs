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
    private readonly IMediaUrlRepository _mediaUrlRepository;
    private readonly IDetailRepository _detailRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;
    public IngredientService(
        IIngredientCategoryRepository ingredientCategoryRepository,
        IMediaUrlRepository mediaUrlRepository,
        IDetailRepository detailRepository,
        IIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork)
    {
        _ingredientCategoryRepository = ingredientCategoryRepository;
        _mediaUrlRepository = mediaUrlRepository;
        _detailRepository = detailRepository;
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task<IngredientDto> CreateIngredientAsync(CreateIngredientRequestDto request, CancellationToken cancellationToken)
    {
        // Create cover image (not yet saved)
        var coverImage = new MediaUrl(MediaType.Image, request.CoverImage);

        // Process media URLs for details but do not save yet
        var details = request.Details.Select(detail =>
        {
            var mediaUrls = detail.MediaUrls
                .Select(url => new MediaUrl(url.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? MediaType.Video : MediaType.Image, url))
                .ToList();

            return new Detail(detail.Title, detail.Description, mediaUrls, detail.Order);
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
            coverImage,
            request.ExpirationDays,
            details // ✅ Pass details in constructor, EF will handle saving them
        );

        await _ingredientRepository.AddAsync(ingredient, cancellationToken);

        // ✅ Commit all at once
        await _unitOfWork.CommitAsync(cancellationToken);

        // Return DTO
        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            CategoryId = ingredient.CategoryId,
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
            Details = ingredient.Details.Select(detail => new DetailDto
            {
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
            }).ToList()
        };
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
            Details = ingredient.Details.Select(detail => new DetailDto
            {
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
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
            Details = ingredient.Details.Select(detail => new DetailDto
            {
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
            }).ToList()
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
            Details = ingredient.Details.Select(detail => new DetailDto
            {
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
            }).ToList()
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
            Details = ingredient.Details.Select(detail => new DetailDto
            {
                Title = detail.Title,
                Description = detail.Description,
                MediaUrls = detail.MediaUrls.Select(mediaUrl => mediaUrl.Url).ToList(),
                Order = detail.Order
            }).ToList()
        }).ToList();
    }
}