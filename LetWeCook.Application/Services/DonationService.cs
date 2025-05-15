using LetWeCook.Application.DTOs.Donation;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Services;

public class DonationService : IDonationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDonationRepository _donationRepository;
    private readonly IPaymentService _paymentService;
    private string _successUrl = "https://7b39-118-70-53-54.ngrok-free.app/api/donation/success";
    private string _cancelUrl = "https://7b39-118-70-53-54.ngrok-free.app/api/donation/cancel";

    public DonationService(
        IUnitOfWork unitOfWork,
        IRecipeRepository recipeRepository,
        IUserRepository userRepository,
        IDonationRepository donationRepository,
        IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
        _donationRepository = donationRepository;
        _paymentService = paymentService;
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

            Console.WriteLine($"Donation ID: {donationId}");

            var donation = await _donationRepository.GetByIdAsync(donationId, cancellationToken);

            if (donation == null)
            {
                return (false, null, null, "Donation not found.");
            }

            donation.Status = "Completed";
            donation.TransactionId = transactionId;

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
            throw new DonationException("Recipe not found.");
        }

        var donator = await _userRepository.GetByIdAsync(siteUserId);
        if (donator == null)
        {
            throw new DonationException("Donator not found.");
        }

        var author = await _userRepository.GetByIdAsync(recipe.CreatedBy.Id);
        if (author == null)
        {
            throw new DonationException("Author not found.");
        }

        // check if author profile exists then check PayPal email exists in profile
        var authorProfile = author.Profile;
        if (authorProfile == null)
        {
            throw new DonationException("Author profile not found.");
        }

        if (string.IsNullOrEmpty(authorProfile.PayPalEmail))
        {
            throw new DonationException("Author PayPal email not found.");
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
            authorProfile.PayPalEmail,
            _successUrl,
            _cancelUrl);

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
            throw new DonationRetrievalException($"Donation with id {donationId} not found");
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
