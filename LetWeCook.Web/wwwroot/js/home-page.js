

$(document).ready(function () {
    const $el = $('#fire-border');
    const shadows = [
        "0 0 8px 3px #ff4500, 0 0 15px 6px #ff8c00, 0 0 25px 10px #b22222, inset 0 0 40px 15px rgba(255,140,0,0.9)",
        "0 0 10px 4px #ff5a00, 0 0 18px 8px #ff9a1a, 0 0 28px 12px #d43b1a, inset 0 0 50px 20px rgba(255,165,0,1)",
        "0 0 12px 5px #ff6a00, 0 0 20px 9px #ffb330, 0 0 32px 15px #d4532a, inset 0 0 55px 25px rgba(255,180,20,1)"
    ];
    let idx = 0;

    setInterval(() => {
        $el.css('box-shadow', shadows[idx]);
        idx = (idx + 1) % shadows.length;
    }, 800);

    $.getJSON('/data/homepage.json', function (data) {
        const recipes = data.TrendingRecipes;
        const $container = $('#trending-recipe-cards');
        $container.empty(); // Clear existing content

        recipes.forEach(recipe => {
            const stars = '★'.repeat(Math.round(recipe.AverageRating)) +
                '☆'.repeat(5 - Math.round(recipe.AverageRating));

            const card = `
            <a href="/Cooking/Recipe/Details/${recipe.Id}" class="block focus:outline-none focus:ring-2 focus:ring-amber-300 focus:ring-offset-2 rounded-xl">
                <div class="relative bg-gradient-to-r from-white to-amber-100 w-[288px] max-w-full p-6 rounded-xl shadow-lg border border-amber-300 hover:shadow-2xl transition-shadow duration-300 flex flex-col">
                    <!-- Fixed-size image frame -->
                    <div class="w-full h-[160px] rounded-xl overflow-hidden mb-4 bg-gray-100">
                        <img src="${recipe.CoverMediaUrl}" alt="${recipe.Name}"
                             class="w-full h-full object-cover transition-all duration-300 hover:brightness-110" />
                    </div>

                    <h3 class="text-xl font-bold text-gray-900 mb-1 break-words line-clamp-2">${recipe.Name}</h3>
                    <p class="text-gray-700 text-sm mb-2 break-words line-clamp-2">${recipe.Description}</p>

                    <div class="flex items-center gap-2 mb-2">
                        <img src="${recipe.CreatedByProfilePic}" alt="Profile picture of ${recipe.CreatedBy}"
                             class="w-6 h-6 rounded-full" />
                        <span class="text-sm text-gray-600 italic">by ${recipe.CreatedBy}</span>
                    </div>

                    <div class="flex justify-between items-center text-sm text-gray-600 mb-2">
                        <span class="italic">${recipe.TotalTime} mins</span>
                        <span class="uppercase text-orange-700 font-medium">${recipe.DifficultyLevel}</span>
                    </div>

                    <div class="flex justify-between items-center">
                        <div class="text-yellow-500 text-lg" role="img" aria-label="Average rating: ${recipe.AverageRating} stars">
                            ${stars}
                        </div>
                        <div class="text-xs text-gray-500">${recipe.TotalRatings} ratings · ${recipe.TotalViews} views</div>
                    </div>
                </div>
            </a>
        `;

            $container.append(card);
        });

        const latestRecipes = data.LatestRecipes;
        const $latestRecipeContainer = $('#latest-recipe-cards');
        $latestRecipeContainer.empty(); // Clear existing content

        latestRecipes.forEach(recipe => {
            const stars = '★'.repeat(Math.round(recipe.AverageRating)) +
                '☆'.repeat(5 - Math.round(recipe.AverageRating));

            const card = `
            <a href="/Cooking/Recipe/Details/${recipe.Id}" class="block focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 rounded-xl">
                <div class="relative bg-gradient-to-r from-blue-50 to-white w-[288px] max-w-full p-6 rounded-xl shadow-lg border border-teal-300 hover:shadow-2xl transition-shadow duration-300 flex flex-col">
                    <!-- Fixed-size image frame -->
                    <div class="w-full h-[160px] rounded-xl overflow-hidden mb-4 bg-gray-100">
                        <img src="${recipe.CoverMediaUrl}" alt="${recipe.Name}"
                             class="w-full h-full object-cover transition-all duration-300 hover:brightness-110" />
                    </div>

                    <h3 class="text-xl font-bold text-gray-900 mb-1 break-words line-clamp-2">${recipe.Name}</h3>
                    <p class="text-gray-700 text-sm mb-2 break-words line-clamp-2">${recipe.Description}</p>

                    <div class="flex items-center gap-2 mb-2">
                        <img src="${recipe.CreatedByProfilePic}" alt="Profile picture of ${recipe.CreatedBy}"
                             class="w-6 h-6 rounded-full" />
                        <span class="text-sm text-gray-600 italic">by ${recipe.CreatedBy}</span>
                    </div>

                    <div class="flex justify-between items-center text-sm text-gray-600 mb-2">
                        <span class="italic">${recipe.TotalTime} mins</span>
                        <span class="uppercase text-teal-700 font-medium">${recipe.DifficultyLevel}</span>
                    </div>

                    <div class="flex justify-between items-center">
                        <div class="text-yellow-500 text-lg" role="img" aria-label="Average rating: ${recipe.AverageRating} stars">
                            ${stars}
                        </div>
                        <div class="text-xs text-gray-500">${recipe.TotalRatings} ratings · ${recipe.TotalViews} views</div>
                    </div>
                </div>
            </a>
        `;

            $latestRecipeContainer.append(card);
        });

        const topRatedRecipes = data.TopRatedRecipes;
        const $mostLovedContainer = $('#most-loved-recipes-container');
        $mostLovedContainer.empty(); // Clear existing content

        topRatedRecipes.forEach(topRatedRecipe => {
            const mostLovedStars = '★'.repeat(Math.round(topRatedRecipe.AverageRating)) +
                '☆'.repeat(5 - Math.round(topRatedRecipe.AverageRating));

            const topRatedCard = `
            <a href="/Cooking/Recipe/Details/${topRatedRecipe.Id}" class="block focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 rounded-xl">
                <div class="relative bg-gradient-to-r from-pink-50 to-white w-[288px] max-w-full p-6 rounded-xl shadow-lg border border-red-300 hover:shadow-2xl transition-shadow duration-300 flex flex-col">
                    <!-- Fixed-size image frame -->
                    <div class="w-full h-[160px] rounded-xl overflow-hidden mb-4 bg-gray-100">
                        <img src="${topRatedRecipe.CoverMediaUrl}" alt="${topRatedRecipe.Name}"
                             class="w-full h-full object-cover transition-all duration-300 hover:brightness-110" />
                    </div>

                    <h3 class="text-xl font-bold text-gray-900 mb-1 break-words line-clamp-2">${topRatedRecipe.Name}</h3>
                    <p class="text-gray-700 text-sm mb-2 break-words line-clamp-2">${topRatedRecipe.Description}</p>

                    <div class="flex items-center gap-2 mb-2">
                        <img src="${topRatedRecipe.CreatedByProfilePic}" alt="Profile picture of ${topRatedRecipe.CreatedBy}"
                             class="w-6 h-6 rounded-full" />
                        <span class="text-sm text-gray-600 italic">by ${topRatedRecipe.CreatedBy}</span>
                    </div>

                    <div class="flex justify-between items-center text-sm text-gray-600 mb-2">
                        <span class="italic">${topRatedRecipe.TotalTime} mins</span>
                        <span class="uppercase text-red-700 font-medium">${topRatedRecipe.DifficultyLevel}</span>
                    </div>

                    <div class="flex justify-between items-center">
                        <div class="text-yellow-500 text-lg" role="img" aria-label="Average rating: ${topRatedRecipe.AverageRating} stars">
                            ${mostLovedStars}
                        </div>
                        <div class="text-xs text-gray-500">${topRatedRecipe.TotalRatings} ratings · ${topRatedRecipe.TotalViews} views</div>
                    </div>
                </div>
            </a>
        `;

            $mostLovedContainer.append(topRatedCard);
        });

        const latestReviews = data.LatestReviews;
        const $latestReviewsContainer = $('#latest-reviews-container');
        $latestReviewsContainer.empty(); // Clear existing content

        latestReviews.forEach(review => {
            const reviewStars = '★'.repeat(review.Rating) +
                '☆'.repeat(5 - review.Rating);

            const reviewCard = `
            <a href="/Cooking/Recipe/Details/${review.RecipeId || '#'}" class="block focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 rounded-xl">
                <div class="bg-white w-80 max-w-full p-6 rounded-xl shadow-lg border border-teal-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center mb-4">
                        <img src="${review.UserProfilePic}" alt="Profile picture of ${review.UserName}"
                             class="w-12 h-12 rounded-full mr-4 border-2 border-teal-200" />
                        <div>
                            <h4 class="text-lg font-semibold text-gray-900">${review.UserName}</h4>
                            <div class="text-yellow-500 text-lg" role="img" aria-label="Rating: ${review.Rating} stars">
                                ${reviewStars}
                            </div>
                        </div>
                    </div>
                    <p class="text-sm text-gray-700 mb-3 italic">${review.Comment}</p>
                    <div class="text-sm text-gray-600">On: <span class="font-medium text-teal-600">${review.RecipeName}</span></div>
                </div>
            </a>
        `;

            $latestReviewsContainer.append(reviewCard);
        });

        const latestDonations = data.LatestDonations;
        const $recentDonationsContainer = $('#recent-donations-container');
        $recentDonationsContainer.empty(); // Clear existing content

        latestDonations.forEach(donation => {
            const donationCard = `
            <a href="/Cooking/Recipe/Details/${donation.RecipeId}" class="block focus:outline-none focus:ring-2 focus:ring-green-700 focus:ring-offset-2 rounded-xl">
                <div class="bg-white w-80 max-w-full p-6 rounded-xl shadow-lg border border-green-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center mb-4">
                        <img src="${donation.DonatorProfilePic || '/images/default-profile.jpg'}" alt="Profile picture of ${donation.DonatorName}"
                             class="w-12 h-12 rounded-full mr-4 border-2 border-green-200" />
                        <div>
                            <h4 class="text-lg font-semibold text-gray-900">${donation.DonatorName}</h4>
                            <div class="text-sm text-green-600 font-medium">Supporter</div>
                        </div>
                    </div>
                    <p class="text-sm text-gray-700 mb-3 italic">${donation.DonateMessage}</p>
                    <div class="text-sm font-semibold text-green-600">Donated: <span class="text-lg">$${donation.Amount.toFixed(2)}</span></div>
                    <div class="text-sm text-gray-600">On: <span class="font-medium text-green-600">${donation.RecipeName}</span></div>
                </div>
            </a>
        `;

            $recentDonationsContainer.append(donationCard);
        });
    }).fail(function () {
        console.error('Failed to fetch trending recipes.');
        $('#trending-recipe-cards').append('<p class="text-red-500">Failed to load recipes.</p>');
    });
});