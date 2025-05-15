using LetWeCook.Application.Dtos;
using LetWeCook.Application.Exceptions;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Common;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Domain.Events;
using LetWeCook.Domain.Exceptions;
using LetWeCook.Domain.ValueObjects;

namespace LetWeCook.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UserService(IIdentityService identityService, IUserRepository userRepository, IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
    }
    public async Task SeedUsersAsync(List<SeedUserDTO> seedUserDTOs, CancellationToken cancellationToken = default)
    {
        foreach (var seedUserDTO in seedUserDTOs)
        {
            if (string.IsNullOrEmpty(seedUserDTO.Email) || string.IsNullOrEmpty(seedUserDTO.Username) || string.IsNullOrEmpty(seedUserDTO.Password))
            {
                Console.WriteLine($"[SEED] Skipping user with missing info: {seedUserDTO.Username}");
                continue;
            }
            var existingAppUserIdByEmail = await _identityService.FindAppUserByEmailAsync(seedUserDTO.Email, cancellationToken);
            if (existingAppUserIdByEmail != null)
            {
                Console.WriteLine($"[SEED] User already exists by email: {seedUserDTO.Email}. Skipping.");
                continue;
            }

            var existingAppUserIdByUsername = await _identityService.FindAppUserByUsernameAsync(seedUserDTO.Username, cancellationToken);
            if (existingAppUserIdByUsername != null)
            {
                Console.WriteLine($"[SEED] User already exists by username: {seedUserDTO.Username}. Skipping.");
                continue;
            }

            var siteUser = new SiteUser(seedUserDTO.Email, false); // No token generation here

            try
            {
                await _userRepository.AddAsync(siteUser, cancellationToken);
                //await _unitOfWork.CommitAsync(cancellationToken);

                var identityCreated = await _identityService.CreateAppUserWithPasswordAsync(seedUserDTO.Email, seedUserDTO.Username, seedUserDTO.Password, siteUser.Id, true, cancellationToken);
                if (!identityCreated)
                {
                    Console.WriteLine($"[SEED] Failed to create identity user: {seedUserDTO.Username}");
                    continue;
                }
                await _unitOfWork.CommitAsync(cancellationToken);

                // log user creation
                var userSeededEvent = new UserSeededEvent(seedUserDTO.Email, seedUserDTO.Password, seedUserDTO.Username, seedUserDTO.IsAdmin);
                await _domainEventDispatcher.DispatchEventsAsync(new List<DomainEvent> { userSeededEvent }, cancellationToken);
                Console.WriteLine($"[SEED] User seeded: {seedUserDTO.Username}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SEED ERROR] {seedUserDTO.Username}: {ex.Message}");
            }
        }
    }

    public async Task SeedUserProfiles(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllWithProfileAsync(cancellationToken);
        var random = new Random();

        // Name pools
        string[] maleFirstNames = { "Liam", "Noah", "Mason", "James", "Lucas", "Leo", "Jack", "Elijah", "Ethan", "Minh", "An", "Huy", "Khanh" };
        string[] femaleFirstNames = { "Emma", "Olivia", "Ava", "Sophia", "Isabella", "Mia", "Amelia", "Zoe", "Aria", "Chloe", "Grace", "Linh" };

        string[] maleLastNames = { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Do", "Dang", "Vo", "Bui", "Smith", "Wilson", "Anderson" };
        string[] femaleLastNames = { "Nguyen", "Tran", "Le", "Pham", "Phan", "Walker", "Clark", "White", "Ngoc", "Taylor", "Brown" };

        // Profile fields
        string[] bios = {
            "Love to cook and share ideas.",
            "Foodie and travel lover.",
            "Home chef and proud!",
            "Passionate about recipes and culture.",
            "Exploring cuisines one dish at a time.",
            "Let’s cook together!",
            "I turn recipes into memories.",
            "Cooking is my therapy.",
            "Bringing flavors to life!",
            "Simplicity is the ultimate flavor.",
            "From grandma’s kitchen to yours.",
            "I believe food connects hearts.",
            "Spice is my love language.",
            "Weekends are for baking and bonding.",
            "Homemade meals, homemade smiles.",
            "Cultural explorer through cuisine.",
            "Inspired by tradition, driven by taste.",
            "New recipe every week, new joy every day.",
            "Cooking with soul and purpose.",
            "Let’s make your taste buds dance!",
            "Savory storyteller through food.",
            "Every dish tells a story.",
            "Mixing memories with ingredients.",
            "I find joy in seasoning and simmering.",
            "Fusion cooking fan – east meets west.",
            "Farm to table enthusiast.",
            "Life is short, eat dessert first!",
            "Comfort food is my superpower.",
            "Sharing joy one plate at a time.",
            "Chopping, stirring, and loving it.",
            "Recipe hoarder and flavor seeker.",
            "Making magic with my skillet.",
            "Bringing joy with every bite.",
            "Messy kitchen, happy heart.",
            "Feeding people is my love language.",
            "Experimenting with taste every day.",
            "Cooking is where I feel most alive.",
            "Food is art, and I'm the artist.",
            "Making every bite count.",
            "Seasoned with passion and purpose.",
            "Whisking up happiness daily.",
            "Curious cook, bold flavors.",
            "Life tastes better when shared.",
            "Trying something new every day in the kitchen.",
            "From scratch, with love.",
            "Flavors inspired by childhood memories.",
            "Always cooking, always learning.",
            "Master of leftovers and midnight snacks.",
            "Cooking is my canvas.",
            "Addicted to good food and good vibes."
        };


        string[] maleProfilePictures = {
        "/images/male/default1.jpg", "/images/male/default2.jpg", "/images/male/default3.jpg",
        "/images/male/default4.jpg", "/images/male/default5.jpg"
    };

        string[] femaleProfilePictures = {
        "/images/female/default1.jpg", "/images/female/default2.jpg", "/images/female/default3.jpg",
        "/images/female/default4.jpg", "/images/female/default5.jpg"
    };

        string[] fakeEmails = {
        "sample1@example.com", "testuser02@domain.com", "cooklover99@fake.com", "demo.account@fake.com",
        "john.doe@demo.net", "chef.life@mockmail.com", "kitchenqueen@demo.org", "mocked.user@fakemail.com"
    };

        string[] houseNumbers = { "12", "34A", "56B", "78", "90", "21C", "45", "3", "108", "210" };
        string[] streets = { "Nguyen Trai", "Le Loi", "Tran Hung Dao", "Vo Van Kiet", "Ly Thuong Kiet", "Pham Van Dong", "Hai Ba Trung" };
        string[] wards = { "Ward 1", "Ward 5", "Ward 7", "Ward 12", "Ward 3", "Ward 10" };
        string[] districts = { "District 1", "District 3", "District 5", "Binh Thanh", "Phu Nhuan", "Tan Binh", "Go Vap" };
        string[] provincesOrCities = { "Ho Chi Minh City", "Hanoi", "Da Nang", "Can Tho", "Hai Phong" };

        foreach (var user in users)
        {
            if (user.Profile != null) continue;

            var gender = random.Next(0, 2) == 0 ? Gender.Male : Gender.Female;

            string firstName = gender == Gender.Male
                ? maleFirstNames[random.Next(maleFirstNames.Length)]
                : femaleFirstNames[random.Next(femaleFirstNames.Length)];

            string lastName = gender == Gender.Male
                ? maleLastNames[random.Next(maleLastNames.Length)]
                : femaleLastNames[random.Next(femaleLastNames.Length)];

            var name = new Name(firstName, lastName);
            var birthDate = new DateTime(random.Next(1990, 2005), random.Next(1, 13), random.Next(1, 28));
            var email = fakeEmails[random.Next(fakeEmails.Length)];
            var bio = bios[random.Next(bios.Length)];

            var address = new Address(
                houseNumbers[random.Next(houseNumbers.Length)],
                streets[random.Next(streets.Length)],
                wards[random.Next(wards.Length)],
                districts[random.Next(districts.Length)],
                provincesOrCities[random.Next(provincesOrCities.Length)]
            );

            var facebook = $"https://facebook.com/{firstName.ToLower()}{lastName.ToLower()}";
            var instagram = $"https://instagram.com/{firstName.ToLower()}_{lastName.ToLower()}";
            var phoneNumber = $"09{random.Next(10000000, 99999999)}";
            var payPalEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@fakepaypal.com";

            var profilePicture = gender == Gender.Male
                ? maleProfilePictures[random.Next(maleProfilePictures.Length)]
                : femaleProfilePictures[random.Next(femaleProfilePictures.Length)];

            var newProfile = new UserProfile(
                name,
                birthDate,
                gender,
                email,
                address,
                bio,
                facebook,
                instagram,
                phoneNumber,
                payPalEmail,
                profilePicture
            )
            {
                UserId = user.Id
            };

            await _userProfileRepository.AddAsync(newProfile, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }

}
