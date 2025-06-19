using LetWeCook.Application.DTOs.Donation;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Exceptions;

namespace LetWeCook.Application.Services;

public class DonationService : IDonationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDonationRepository _donationRepository;
    private readonly IUserInteractionRepository _userInteractionRepository;
    private readonly IPaymentService _paymentService;

    public DonationService(
        IUnitOfWork unitOfWork,
        IRecipeRepository recipeRepository,
        IUserRepository userRepository,
        IDonationRepository donationRepository,
        IPaymentService paymentService,
        IUserInteractionRepository userInteractionRepository)
    {
        _unitOfWork = unitOfWork;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _donationRepository = donationRepository;
        _paymentService = paymentService;
        _userInteractionRepository = userInteractionRepository;
    }

    public async Task SeedRecipeDonationsAsync(int amount, CancellationToken cancellationToken = default)
    {
        // check number of donations already exists
        var existingDonationsCount = await _donationRepository.GetTotalCountAsync(cancellationToken);
        if (existingDonationsCount >= amount)
        {
            Console.WriteLine("Donations already seeded.");
            return;
        }

        // define message pool
        var messages = new List<string>
{
    "Great recipe!",
    "I love this dish!",
    "Can't wait to try this!",
    "This looks amazing!",
    "Yum! I'm hungry now.",
    "Absolutely delicious!",
    "This recipe is a keeper.",
    "So easy and tasty!",
    "My family will love this.",
    "Perfect for dinner tonight.",
    "I’m bookmarking this one!",
    "Delicious and simple.",
    "A new favorite in my kitchen.",
    "The flavors are incredible.",
    "This looks mouthwatering!",
    "Can’t wait to make this again.",
    "Such a creative recipe!",
    "Love the combination of ingredients.",
    "This is comfort food at its best.",
    "So flavorful and satisfying.",
    "Easy to follow and tasty results.",
    "This will be my go-to recipe.",
    "Looks like a restaurant-quality dish!",
    "I’m drooling just looking at this.",
    "This recipe made my day!",
    "My taste buds thank you!",
    "Such a beautiful presentation.",
    "I’m definitely sharing this with friends.",
    "Yummy and healthy too!",
    "Made this for my family and they loved it.",
    "Quick, easy, and delicious!",
    "I’m adding this to my weekly meal plan.",
    "This recipe is pure magic.",
    "The perfect balance of flavors.",
    "So good, I ate seconds!",
    "This dish made me feel like a chef.",
    "I’m obsessed with this recipe!",
    "The best thing I’ve cooked all week.",
    "This is cooking perfection.",
    "I can't stop thinking about this dish.",
    "The aroma alone is irresistible.",
    "Such a refreshing twist!",
    "A must-try for everyone!",
    "This recipe never disappoints.",
    "I’m impressed by how tasty this is.",
    "A great way to impress guests.",
    "Comfort food done right.",
    "Totally delicious and satisfying.",
    "This recipe is a hit in my house.",
    "I can’t wait to make this again soon!",
    "So flavorful and easy to prepare.",
    "Perfect for meal prepping.",
    "I added my own twist and it was fantastic!",
    "The instructions were super clear.",
    "I love how versatile this dish is.",
    "Made this for a dinner party and it was a hit!",
    "So comforting and warm.",
    "A simple recipe with amazing results.",
    "The texture is just perfect.",
    "My kids actually ate this without complaining!",
    "Great for beginner cooks.",
    "I appreciate the healthy ingredients.",
    "This recipe brightened up my day.",
    "I’m definitely making this again next week.",
    "Such a satisfying meal.",
    "Perfect for any season.",
    "I couldn’t believe how easy this was.",
    "This recipe is a game-changer.",
    "Added this to my favorites list!",
    "I love recipes that come together quickly.",
    "Perfect for a cozy night in.",
    "I’m sharing this with all my friends.",
    "This is the best recipe I’ve found online.",
    "The flavors are perfectly balanced.",
    "Super delicious and filling.",
    "I love the combination of spices used.",
    "This recipe reminds me of home cooking.",
    "So fresh and vibrant.",
    "I made this for brunch and it was amazing.",
    "Great recipe for entertaining guests.",
    "I felt like a pro chef making this!",
    "The sauce was incredible.",
    "I used this as a base and created my own dish.",
    "I appreciate the detailed steps.",
    "This recipe is a lifesaver on busy days.",
    "Perfect comfort food after a long day.",
    "So good that I doubled the recipe!",
    "I love the crispy texture.",
    "This recipe made me fall in love with cooking.",
    "Easy ingredients and fantastic flavor.",
    "This dish is always a crowd-pleaser.",
    "A refreshing take on a classic.",
    "The seasoning is spot on.",
    "I felt so proud making this.",
    "Such a delightful surprise!",
    "I added this to my rotation.",
    "Perfect for potlucks.",
    "I can’t wait to try the dessert version!",
    "So many compliments after I served this.",
    "The presentation looks stunning.",
    "I love how adaptable this recipe is.",
    "Simple, quick, and delicious.",
    "I’ll be making this for holidays.",
    "My go-to recipe for weeknight dinners.",
    "This dish is pure comfort in a bowl.",
    "Easy, fast, and absolutely tasty!",
    "I appreciate the healthy twist.",
    "This is a family favorite now.",
    "I love recipes that use pantry staples.",
    "So satisfying and flavorful.",
    "I finally nailed the perfect version of this!",
    "This recipe brought back great memories.",
    "Highly recommend to everyone!",
    "The perfect combination of sweet and savory."
};


        // get all users in a pool
        var users = await _userRepository.GetAllAsync();
        if (users.Count == 0) throw new DonationException("No users found.", ErrorCode.DONATION_DONATOR_NOT_FOUND);
        // get all recipes in a pool
        var recipes = await _recipeRepository.GetAllAsync();
        if (recipes.Count == 0) throw new DonationException("No recipes found.", ErrorCode.DONATION_RECIPE_NOT_FOUND);


        // loop until amount is reached, get random user and recipe, create donation with random amount and random messages
        var random = new Random();
        while (existingDonationsCount < amount)
        {
            // get random user
            var donator = users[random.Next(users.Count)];
            // get random recipe
            var recipe = recipes[random.Next(recipes.Count)];
            var recipeWithAuthor = await _recipeRepository.GetRecipeWithAuthorByIdAsync(recipe.Id);
            if (recipeWithAuthor == null)
            {
                Console.WriteLine($"Recipe with ID {recipe.Id} not found. Skipping donation.");
                continue; // skip this iteration if recipe is not found
            }

            // generate random amount between 1 and 100
            decimal donationAmount = (decimal)(random.NextDouble() * 99 + 1);
            // get random message
            string donateMessage = messages[random.Next(messages.Count)];

            try
            {
                var donation = new Donation(
                    recipe.Id,
                    donator.Id,
                    recipeWithAuthor.CreatedBy.Id,
                    donationAmount,
                    "USD", // assuming USD for simplicity
                    donateMessage,
                    "Completed", // set status to Completed for seeding
                    "seeded-donation-with-no-approval-url" // no approval URL for seeded donations
                );
                await _donationRepository.AddAsync(donation, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                existingDonationsCount++;
                Console.WriteLine($"Seeded donation: {donation.Id} for recipe: {recipe.Name} with amount: {donationAmount:C}");
            }
            catch (DonationException ex)
            {
                Console.WriteLine($"Failed to create donation: {ex.Message}");
            }
        }
    }

    public async Task<(bool Success, string TransactionId, string CustomId, string Error)> CaptureDonationAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var captureResult = await _paymentService.CaptureDonationAsync(orderId);

        if (captureResult.Success)
        {
            // Success!
            string transactionId = captureResult.TransactionId;
            string customId = captureResult.CustomId;

            // conver customId to Guid
            if (!Guid.TryParse(customId, out Guid donationId))
            {
                return (false, null, null, "Invalid custom ID format.");
            }


            var donation = await _donationRepository.GetWithRecipeDonatorAndAuthorAsync(donationId, cancellationToken);



            if (donation == null)
            {
                Console.WriteLine($"Donation with ID {donationId} not found.");
                return (false, null, null, "Donation not found.");
            }

            donation.Status = "Completed";
            donation.TransactionId = transactionId;

            var interaction = new UserInteraction(
                donation.Donator.Id,
                donation.RecipeId,
                "donate",
                (float)donation.Amount
            );

            Console.WriteLine($"Donation ID: {donationId}");


            await _userInteractionRepository.AddAsync(interaction, cancellationToken);
            await _donationRepository.UpdateAsync(donation, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

        }
        else
        {
            // Failed
            string error = captureResult.Error;

            // Handle failure
        }

        return captureResult;
    }

    public async Task<Guid> CreateDonationAsync(Guid siteUserId, Guid recipeId, decimal amount, string currency, string donateMessage, CancellationToken cancellationToken = default)
    {
        // check recipe exists
        var recipe = await _recipeRepository.GetRecipeWithAuthorByIdAsync(recipeId);

        if (recipe == null)
        {
            var ex = new DonationException("Recipe not found.", ErrorCode.DONATION_RECIPE_NOT_FOUND);
            ex.AddContext("RecipeId", recipeId.ToString());
            throw ex;
        }

        var donator = await _userRepository.GetByIdAsync(siteUserId);
        if (donator == null)
        {
            var ex = new DonationException("Donator not found.", ErrorCode.DONATION_DONATOR_NOT_FOUND);
            ex.AddContext("DonatorId", siteUserId.ToString());
            throw ex;
        }

        var author = await _userRepository.GetByIdAsync(recipe.CreatedBy.Id);
        if (author == null)
        {
            var ex = new DonationException("Author not found.", ErrorCode.DONATION_AUTHOR_NOT_FOUND);
            ex.AddContext("AuthorId", recipe.CreatedBy.Id.ToString());
            throw ex;
        }

        // check if author profile exists then check PayPal email exists in profile
        var authorProfile = author.Profile;
        if (authorProfile == null)
        {
            var ex = new DonationException("Author profile not found.", ErrorCode.DONATION_AUTHOR_PROFILE_NOT_FOUND);
            ex.AddContext("AuthorId", recipe.CreatedBy.Id.ToString());
            throw ex;
        }

        if (string.IsNullOrEmpty(authorProfile.PayPalEmail))
        {
            var ex = new DonationException("Author PayPal email not found.", ErrorCode.DONATION_AUTHOR_PAYPAL_NOT_FOUND);
            ex.AddContext("AuthorId", recipe.CreatedBy.Id.ToString());
            throw ex;
        }

        // Create donation with pending status
        var donation = new Donation(
            recipeId,
            siteUserId,
            recipe.CreatedBy.Id,
            amount,
            currency,
            donateMessage,
            "Pending",
            ""
        );

        // Call IPaymentService to create PayPal order
        var approvalUrl = await _paymentService.CreateDonationOrderAsync(
            donation.Id,
            amount,
            currency,
            $"Donation for Recipe: {recipe.Name}",
            authorProfile.PayPalEmail);

        donation.SetApprovalUrl(approvalUrl);

        await _donationRepository.AddAsync(donation, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return donation.Id;
    }

    public async Task<List<DonationDetailDto>> GetCompletedDonationsByRecipeIdAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        var donations = await _donationRepository.GetCompletedDonationsByRecipeId(recipeId, cancellationToken);
        return donations.Select(donation => new DonationDetailDto
        {
            DonationId = donation.Id,
            AuthorId = donation.AuthorId,
            RecipeId = donation.RecipeId,

            AuthorProfileDto = new ProfileDto
            {
                Id = donation.Author.Id,
                Name = donation.Author.Profile?.Name.FullName ?? string.Empty,
                ProfilePicUrl = donation.Author.Profile?.ProfilePic,
                Bio = donation.Author.Profile?.Bio,
                Facebook = donation.Author.Profile?.Facebook,
                Instagram = donation.Author.Profile?.Instagram,
                PayPalEmail = donation.Author.Profile?.PayPalEmail
            },

            DonatorProfileDto = new ProfileDto
            {
                Id = donation.Donator.Id,
                Name = donation.Donator.Profile?.Name.FullName ?? string.Empty,
                ProfilePicUrl = donation.Donator.Profile?.ProfilePic,
                Bio = donation.Donator.Profile?.Bio,
                Facebook = donation.Donator.Profile?.Facebook,
                Instagram = donation.Donator.Profile?.Instagram,
                PayPalEmail = donation.Donator.Profile?.PayPalEmail
            },

            TransactionId = donation.TransactionId,
            Amount = donation.Amount,
            DonateMessage = donation.DonateMessage,
            Status = donation.Status,
            ApprovalUrl = donation.ApprovalUrl,
            CreatedAt = donation.CreatedAt,

            RecipeOverview = new RecipeOverviewDto
            {
                RecipeId = donation.Recipe.Id,
                Name = donation.Recipe.Name,
                CoverImageUrl = donation.Recipe.CoverMediaUrl?.Url ?? string.Empty
            }
        }).ToList();
    }

    public async Task<DonationDetailDto> GetDonationDetailsAsync(Guid donationId, CancellationToken cancellationToken)
    {
        var donation = await _donationRepository.GetDonationDetailsById(donationId, cancellationToken);

        if (donation == null)
        {
            var ex = new DonationRetrievalException($"Donation with id {donationId} not found", ErrorCode.DONATION_NOT_FOUND);
            ex.AddContext("DonationId", donationId.ToString());
            throw ex;
        }

        return new DonationDetailDto
        {
            DonationId = donation.Id,
            AuthorId = donation.AuthorId,
            RecipeId = donation.RecipeId,

            AuthorProfileDto = new ProfileDto
            {
                Id = donation.Author.Id,
                Name = donation.Author.Profile?.Name.FullName ?? string.Empty,
                ProfilePicUrl = donation.Author.Profile?.ProfilePic,
                Bio = donation.Author.Profile?.Bio,
                Facebook = donation.Author.Profile?.Facebook,
                Instagram = donation.Author.Profile?.Instagram,
                PayPalEmail = donation.Author.Profile?.PayPalEmail
            },

            DonatorProfileDto = new ProfileDto
            {
                Id = donation.Donator.Id,
                Name = donation.Donator.Profile?.Name.FullName ?? string.Empty,
                ProfilePicUrl = donation.Donator.Profile?.ProfilePic,
                Bio = donation.Donator.Profile?.Bio,
                Facebook = donation.Donator.Profile?.Facebook,
                Instagram = donation.Donator.Profile?.Instagram,
                PayPalEmail = donation.Donator.Profile?.PayPalEmail
            },

            TransactionId = donation.TransactionId,
            Amount = donation.Amount,
            DonateMessage = donation.DonateMessage,
            Status = donation.Status,
            ApprovalUrl = donation.ApprovalUrl,
            CreatedAt = donation.CreatedAt,

            RecipeOverview = new RecipeOverviewDto
            {
                RecipeId = donation.Recipe.Id,
                Name = donation.Recipe.Name,
                CoverImageUrl = donation.Recipe.CoverMediaUrl?.Url ?? string.Empty
            }
        };
    }

}
