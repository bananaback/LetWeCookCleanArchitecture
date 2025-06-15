using LetWeCook.Application.Dtos.RecipeRating;
using LetWeCook.Application.DTOs;
using LetWeCook.Application.Enums;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Application.RecipeRatings.Sorts;
using LetWeCook.Application.RecipeRatings.Specifications;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Services;

public class RecipeRatingService : IRecipeRatingService
{
    private readonly IRecipeRatingRepository _recipeRatingRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserInteractionRepository _userInteractionRepository;

    public RecipeRatingService(
        IRecipeRatingRepository recipeRatingRepository,
        IRecipeRepository recipeRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserInteractionRepository userInteractionRepository)
    {
        _recipeRatingRepository = recipeRatingRepository;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userInteractionRepository = userInteractionRepository;
    }

    public async Task<RecipeRatingDto> CreateRecipeRatingAsync(CreateRecipeRatingRequestDto request, CancellationToken cancellationToken = default)
    {
        // validate rating infor
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new CreateRecipeRatingException("Rating must be between 1 and 5.");
        }

        // check if recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe == null)
        {
            throw new CreateRecipeRatingException("Recipe not found.");
        }

        // check if user exists
        var user = await _userRepository.GetByIdAsync(request.SiteUserId, cancellationToken);
        if (user == null)
        {
            throw new CreateRecipeRatingException("User not found.");
        }

        // check if user has already rated the recipe, just update the rating
        var existingRating = await _recipeRatingRepository.GetByUserIdAndRecipeIdAsync(request.SiteUserId, request.RecipeId, cancellationToken);
        if (existingRating != null)
        {
            recipe.UpdateAverageRatingOnRatingUpdated(request.Rating, existingRating.Rating);

            existingRating.Rating = request.Rating;
            existingRating.Comment = request.Comment;
            existingRating.UpdatedAt = DateTime.UtcNow;

            await _recipeRatingRepository.UpdateAsync(existingRating, cancellationToken);
            await _recipeRepository.UpdateAsync(recipe, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);


            return new RecipeRatingDto
            {
                RecipeId = existingRating.RecipeId,
                UserId = existingRating.UserId,
                Rating = existingRating.Rating,
                Comment = existingRating.Comment,
                CreatedAt = existingRating.CreatedAt,
                UpdatedAt = existingRating.UpdatedAt
            };
        }


        var recipeRating = new RecipeRating(
            request.RecipeId,
            request.SiteUserId,
            request.Rating,
            request.Comment
        );

        var interactionRating = new UserInteraction(
            request.SiteUserId,
            request.RecipeId,
            "rating",
            request.Rating
        );

        var interactionComment = new UserInteraction(
            request.SiteUserId,
            request.RecipeId,
            "comment",
            request.Comment.Length
        );

        await _recipeRatingRepository.AddAsync(recipeRating, cancellationToken);
        await _userInteractionRepository.AddAsync(interactionRating, cancellationToken);
        await _userInteractionRepository.AddAsync(interactionComment, cancellationToken);

        recipe.UpdateAverageRatingOnNewRatingAdded(request.Rating);

        await _recipeRepository.UpdateAsync(recipe, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new RecipeRatingDto
        {
            RecipeId = recipeRating.RecipeId,
            UserId = recipeRating.UserId,
            Rating = recipeRating.Rating,
            Comment = recipeRating.Comment,
            CreatedAt = recipeRating.CreatedAt,
            UpdatedAt = recipeRating.UpdatedAt
        };
    }

    public async Task<PaginatedResult<RecipeRatingDto>> GetRecipeRatingsAsync(Guid recipeId, string sortBy, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var recipeRatingsQuery = _recipeRatingRepository.GetRecipeRatingsQueryableAsync(recipeId, cancellationToken);
        var specification = new RecipeRatingSpecification();
        if (sortBy == "Newest")
        {
            specification.AddSort(new RecipeRatingLatestSortFilter(SortDirection.Desc));
        }
        else if (sortBy == "Positive")
        {
            specification.AddSort(new RecipeRatingRatingSortFilter(SortDirection.Desc));
        }
        else if (sortBy == "Negative")
        {
            specification.AddSort(new RecipeRatingRatingSortFilter(SortDirection.Asc));
        }

        recipeRatingsQuery = specification.Apply(recipeRatingsQuery);

        var totalCount = await _recipeRatingRepository.CountAsync(recipeRatingsQuery, cancellationToken);

        var paginatedQuery = specification.ApplyPagination(recipeRatingsQuery, page, pageSize);

        var recipeRatings = await _recipeRatingRepository.QueryableToListAsync(paginatedQuery, cancellationToken);

        return new PaginatedResult<RecipeRatingDto>
        (
            recipeRatings.Select(r => new RecipeRatingDto
            {
                RecipeId = r.RecipeId,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                UserName = r.User.Profile == null ? "" : r.User.Profile.Name.FullName,
                UserProfileImage = r.User.Profile?.ProfilePic ?? ""
            }).ToList(),
            totalCount,
            page,
            pageSize
        );
    }

    public async Task<RecipeRatingDto> GetUserRatingForRecipeAsync(Guid recipeId, Guid siteUserId, CancellationToken cancellationToken = default)
    {
        // check if user exists
        var user = await _userRepository.GetByIdAsync(siteUserId, cancellationToken);
        if (user == null)
        {
            throw new CreateRecipeRatingException("User not found.");
        }

        // check if recipe exists
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe == null)
        {
            throw new CreateRecipeRatingException("Recipe not found.");
        }

        // get user rating for recipe
        var recipeRating = await _recipeRatingRepository.GetByUserIdAndRecipeIdAsync(siteUserId, recipeId, cancellationToken);
        if (recipeRating == null)
        {
            throw new CreateRecipeRatingException("User has not rated this recipe.");
        }

        return new RecipeRatingDto
        {
            RecipeId = recipeRating.RecipeId,
            UserId = recipeRating.UserId,
            Rating = recipeRating.Rating,
            Comment = recipeRating.Comment,
            CreatedAt = recipeRating.CreatedAt,
            UpdatedAt = recipeRating.UpdatedAt,
            UserName = recipeRating.User.Profile == null ? "" : recipeRating.User.Profile.Name.FullName,
            UserProfileImage = recipeRating.User.Profile?.ProfilePic ?? ""
        };
    }

    public async Task SeedRecipeRatingsAsync(int amount, CancellationToken cancellationToken = default)
    {
        // check if amount is valid
        if (amount <= 0)
        {
            throw new CreateRecipeRatingException("Amount must be greater than 0.");
        }

        // check if already enough amount of ratings
        var existingRatings = await _recipeRatingRepository.GetAllAsync(cancellationToken);
        if (existingRatings.Count >= amount)
        {
            return;
        }

        var ratingComments = new Dictionary<int, List<string>>
        {
            [1] = new List<string>
            {
                "Totally disappointed. The taste was bland.",
                "Followed the instructions exactly, but it came out terrible.",
                "Not sure what went wrong, but it was inedible.",
                "Way too salty and overcooked. Never again.",
                "My family didn’t enjoy it at all.",
                "The ingredients didn’t work well together.",
                "Too complicated for such a poor result.",
                "Cooking time was way off. Ended up burnt.",
                "Recipe looks good but just didn’t work.",
                "Sadly, this one’s a miss for me.",
                "Texture was strange and unappetizing.",
                "I double-checked everything—still a fail.",
                "This recipe ruined my evening.",
                "Even my dog wouldn’t eat it.",
                "Huge waste of ingredients.",
                "Burnt outside, raw inside. Terrible.",
                "Photos looked promising, but no.",
                "This was a disaster from the first step.",
                "Nothing like the pictures.",
                "Would give zero stars if I could.",
                "Complete mess. Don't try this.",
                "Tasted awful. Don’t know how this is rated.",
                "Prep steps are so unclear.",
                "It made my kitchen smell bad.",
                "Absolutely not recommended.",
                "Unbalanced flavors. No harmony at all.",
                "Takes forever and not worth the time.",
                "Needed so much fixing just to make it edible.",
                "Even with tweaks, still bad.",
                "Major letdown. Do not try."
            },

            [2] = new List<string>
            {
                "Not the worst, but needs major improvement.",
                "Texture was okay, but flavor lacked depth.",
                "Ingredients list was confusing.",
                "Had potential, but turned out dry.",
                "Kids wouldn’t touch it.",
                "Mediocre result after a lot of effort.",
                "Didn’t turn out as expected.",
                "Too much oil and not enough seasoning.",
                "Recipe needs clearer instructions.",
                "Disappointed—thought it would be better.",
                "Flavors didn’t blend well.",
                "Meh... I wouldn’t recommend it.",
                "Just average. Needs reworking.",
                "Some bites were okay, most were not.",
                "It didn’t really wow anyone.",
                "Overly greasy and not satisfying.",
                "Cooking time was way off.",
                "Sauce was watery and dull.",
                "I followed the recipe, still disappointing.",
                "Maybe for someone else, but not for me.",
                "Not worth the time spent.",
                "Had to add a ton of spices to make it work.",
                "Too much cleanup for the result.",
                "Some steps were vague and misleading.",
                "Expected more flavor.",
                "Undercooked even after adding 10 mins.",
                "Didn’t look appealing on the plate.",
                "Very basic. Not worth repeating.",
                "It felt like something was missing.",
                "Flavors clashed badly."
            },

            [3] = new List<string>
            {
                "It was alright, nothing special.",
                "Good enough for a quick meal, but not memorable.",
                "Average flavor, could use more seasoning.",
                "Worked fine, but I probably won’t make it again.",
                "Needed some tweaks to get it right.",
                "Decent recipe, just not my taste.",
                "Turned out okay after adjusting cook time.",
                "Fine for beginners, but a bit plain.",
                "Instructions could be clearer.",
                "Not bad, but I’ve had better.",
                "Gets the job done, but not exciting.",
                "Middle-of-the-road recipe.",
                "Filling, but lacked bold flavor.",
                "Okay in a pinch, but not great.",
                "Could be better with fresher ingredients.",
                "Tasted okay, looked messy.",
                "Nice effort, but lacks refinement.",
                "It’s a start, but not there yet.",
                "Usable base, I had to enhance it.",
                "Needed more depth and texture.",
                "I’ll try it again with my own twist.",
                "Passable, but didn’t wow anyone.",
                "Better with some cheese and herbs.",
                "Simple and plain.",
                "Served the purpose, nothing more.",
                "Mild flavor, good for picky eaters.",
                "You’ll need to spice it up a bit.",
                "Good for practice, not a keeper.",
                "Some steps felt unnecessary.",
                "It’s okay... once."
            },

            [4] = new List<string>
            {
                "Tasted great! Just a little more spice would be perfect.",
                "Really good! My family liked it a lot.",
                "Turned out well, will definitely make again.",
                "Solid recipe. Easy to follow and tasty.",
                "Nice balance of flavor and texture.",
                "Almost perfect! Added a few of my own tweaks.",
                "Great for weeknight dinner.",
                "Loved it, though I’d reduce the sugar next time.",
                "Very flavorful and easy to prepare.",
                "Surprisingly good! Will recommend it.",
                "Yummy! Just needed extra seasoning.",
                "Delicious and comforting.",
                "Pleasantly surprised how well it turned out.",
                "Simple and very effective.",
                "Tasted homemade in the best way.",
                "Friends enjoyed it too!",
                "Great go-to recipe with potential for variation.",
                "Quick and flavorful!",
                "Hits the spot. Family-approved.",
                "Impressive outcome with little effort.",
                "Great texture, rich flavors.",
                "My kids loved it.",
                "A little adjustment and it’s perfect.",
                "Tasted fresh and well-balanced.",
                "Great starting point for variations.",
                "Made it twice already!",
                "It’s in my regular rotation now.",
                "So good, but slightly salty.",
                "Tweak the timing and it’s a winner.",
                "Recommended with slight modifications."
            },

            [5] = new List<string>
            {
                "Absolutely amazing! Restaurant quality!",
                "Perfect in every way. A new favorite!",
                "So delicious and easy to follow!",
                "Everyone asked for seconds. Enough said.",
                "Loved every bite! Saved to my go-to list.",
                "Simple, clear, and mouthwatering results!",
                "Hands down the best I’ve tried!",
                "Nailed it! Tastes like it’s from a pro chef.",
                "Followed exactly and it was flawless.",
                "Can’t believe how good this turned out!",
                "My go-to recipe now. So good!",
                "Better than takeout!",
                "I made this for a party—huge hit!",
                "Tastes like it came from a gourmet kitchen.",
                "Five stars aren’t enough!",
                "Perfect balance of flavors.",
                "I’ll make this again and again.",
                "Family loved it, and so did I!",
                "Beautiful presentation and flavor.",
                "This is the kind of meal you crave.",
                "Amazing, even as leftovers!",
                "Cooked it once, now it's a staple.",
                "Super clear instructions. Perfect result!",
                "Cooked to perfection. Everyone asked for the recipe.",
                "Delicious, hearty, and so satisfying.",
                "Can’t wait to make this again!",
                "One of the best meals I’ve ever cooked.",
                "Everything about this worked beautifully.",
                "Flavor explosion! A keeper for sure.",
                "Instant classic. Thank you!"
            }
        };


        // get all recipes
        var recipes = _recipeRepository.GetAllAsync(cancellationToken).Result;

        // get all users
        var users = _userRepository.GetAllAsync(cancellationToken).Result;

        var starEmojisPool = new Dictionary<int, List<string>>
        {
            [1] = new List<string>
            {
                "⭐", "😡", "👎", "🤢", "🙄", "😤", "😠", "😞", "👎🏻", "👎🏽"
            },
            [2] = new List<string>
            {
                "⭐⭐", "😕", "🙁", "😬", "😒", "😓", "😑", "🫤", "👎", "🤷"
            },
            [3] = new List<string>
            {
                "⭐⭐⭐", "😐", "👌", "🤔", "🙂", "😶", "🆗", "🤷‍♂️", "🤷‍♀️", "👍"
            },
            [4] = new List<string>
            {
                "⭐⭐⭐⭐", "😊", "👍", "😄", "😃", "😋", "👌", "👏", "💯", "💖"
            },
            [5] = new List<string>
            {
                "⭐⭐⭐⭐⭐", "😍", "🔥", "🤩", "😻", "👏👏", "💥", "💎", "🌟", "🥰", "🎉"
            }
        };


        // pick random recipe and user
        var random = new Random();

        for (int i = 0; i < amount - existingRatings.Count; i++)
        {
            var recipe = recipes[random.Next(recipes.Count)];
            var user = users[random.Next(users.Count)];

            // check if user has already rated the recipe, if so overwrite the rating
            var existingRating = await _recipeRatingRepository.GetByUserIdAndRecipeIdAsync(user.Id, recipe.Id, cancellationToken);
            if (existingRating != null)
            {
                var ratingValue = random.Next(1, 6);
                recipe.UpdateAverageRatingOnRatingUpdated(ratingValue, existingRating.Rating);
                existingRating.Rating = ratingValue;
                var emojis = starEmojisPool[existingRating.Rating]
                    .OrderBy(x => random.Next())
                    .Take(random.Next(1, 4)) // pick 1 to 3 emojis
                    .ToList();

                var rawComment = ratingComments[existingRating.Rating][random.Next(ratingComments[existingRating.Rating].Count)];

                var decoratedComment = $"{rawComment} {string.Join(" ", emojis)}";

                existingRating.Comment = decoratedComment;

                existingRating.UpdatedAt = DateTime.UtcNow;

                await _recipeRatingRepository.UpdateAsync(existingRating, cancellationToken);
                await _recipeRepository.UpdateAsync(recipe, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                continue;
            }

            var rating = random.Next(1, 6);
            recipe.UpdateAverageRatingOnNewRatingAdded(rating);

            // get comment from the dictionary
            var commentList = ratingComments[rating];
            var comment = commentList[random.Next(commentList.Count)];

            var emojis2 = starEmojisPool[rating]
                .OrderBy(x => random.Next())
                .Take(random.Next(1, 4)) // pick 1 to 3 emojis
                .ToList();

            var decoratedComment2 = $"{comment} {string.Join(" ", emojis2)}";

            var recipeRating = new RecipeRating(recipe.Id, user.Id, rating, decoratedComment2);

            await _recipeRatingRepository.AddAsync(recipeRating, cancellationToken);
            await _recipeRepository.UpdateAsync(recipe, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
