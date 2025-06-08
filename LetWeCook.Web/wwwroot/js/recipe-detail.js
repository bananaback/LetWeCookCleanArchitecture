

let fetchedRecipe = {};

$(document).ready(function () {
    const recipeId = $('#recipeId').val();

    // Function to fetch and populate random recipes in the left panel
    function loadRandomRecipes() {
        $.ajax({
            url: '/api/recipes/random',
            type: 'GET',
            data: { count: 4 }, // Request 4 random recipes
            dataType: 'json',
            success: function (data) {
                const container = $('#recipesYouMayLike .space-y-4');
                container.empty(); // Clear any existing content

                if (data && data.length > 0) {
                    // Iterate through the recipes and create cards
                    $.each(data, function (index, recipe) {
                        const recipeCard = `
                            <a href="/Cooking/Recipe/Details/${recipe.id}" 
                               class="block bg-white rounded-xl shadow-md p-4 hover:shadow-lg hover:scale-105 transition-transform duration-300 border border-lime-200">
                                <img src="${recipe.coverImage || '/images/placeholder.jpg'}" 
                                     alt="${recipe.name}" 
                                     class="w-full h-40 object-cover rounded-lg mb-3">
                                <h3 class="text-lg font-semibold text-orange-600 truncate">${recipe.name}</h3>
                                <p class="text-sm text-gray-600 line-clamp-2">${recipe.description || 'No description available'}</p>
                                <div class="mt-2 flex items-center gap-2 text-gray-500 text-sm">
                                    <span>⭐ ${recipe.averageRating ? recipe.averageRating.toFixed(1) : 'N/A'}</span>
                                    <span>👁️ ${recipe.totalViews || 0} Views</span>
                                </div>
                            </a>
                        `;
                        container.append(recipeCard);
                    });
                } else {
                    // Display fallback message if no recipes are returned
                    container.append(`
                        <p class="text-gray-600 text-center">No recipes found. Try again later!</p>
                    `);
                }
            },
            error: function (xhr, status, error) {
                // Handle errors (e.g., 403 Forbidden or other issues)
                const container = $('#recipesYouMayLike .space-y-4');
                container.empty();
                container.append(`
                    <p class="text-red-600 text-center">Failed to load recipes. Please try again later.</p>
                `);
            }
        });
    }

    // Call the function to load recipes when the page loads
    loadRandomRecipes();

    // Function to fetch and populate personalized recipe suggestions in the right panel
    function loadRecipeSuggestions() {
        $.ajax({
            url: '/api/suggestions',
            type: 'GET',
            data: { count: 4 }, // Request 4 suggested recipes
            dataType: 'json',
            success: function (data) {
                const container = $('#recipesInCategory .space-y-4');
                container.empty(); // Clear any existing content

                if (data && data.length > 0) {
                    // Iterate through the recipes and create cards
                    $.each(data, function (index, recipe) {
                        const recipeCard = `
                            <a href="/Cooking/Recipe/Details/${recipe.id}" 
                               class="block bg-white rounded-xl shadow-md p-4 hover:shadow-lg hover:scale-105 transition-transform duration-300 border border-lime-200">
                                <img src="${recipe.coverImage || '/images/placeholder.jpg'}" 
                                     alt="${recipe.name}" 
                                     class="w-full h-40 object-cover rounded-lg mb-3">
                                <h3 class="text-lg font-semibold text-orange-600 truncate">${recipe.name}</h3>
                                <p class="text-sm text-gray-600 line-clamp-2">${recipe.description || 'No description available'}</p>
                                <div class="mt-2 flex items-center gap-2 text-gray-500 text-sm">
                                    <span>⭐ ${recipe.averageRating ? recipe.averageRating.toFixed(1) : 'N/A'}</span>
                                    <span>👁️ ${recipe.totalViews || 0} Views</span>
                                </div>
                            </a>
                        `;
                        container.append(recipeCard);
                    });
                } else {
                    // Display fallback message if no recipes are returned
                    container.append(`
                        <p class="text-gray-600 text-center">No suggested recipes found. Try again later!</p>
                    `);
                }
            },
            error: function (xhr, status, error) {
                // Handle errors (e.g., server issues)
                const container = $('#recipesInCategory .space-y-4');
                container.empty();
                container.append(`
                    <p class="text-red-600 text-center">Failed to load suggestions. Please try again later.</p>
                `);
            }
        });
    }

    // Call the function to load suggestions when the page loads
    loadRecipeSuggestions();

    $.ajax({
        url: `/api/recipe-details/${recipeId}`,
        method: 'GET',
        success: function (recipe) {

            fetchedRecipe = recipe; // Store the fetched recipe for later use
            // Cover & Basic
            $('#coverImage').attr('src', recipe.coverImage || 'https://via.placeholder.com/800x320?text=Recipe+Image');
            $('#recipeName').text(recipe.name || 'Untitled Recipe');
            $('#recipeDescription').text(recipe.description || 'No description available.');

            // Author Profile
            const author = recipe.authorProfile || {};
            $('#authorName').text(author.name || 'Anonymous');
            $('#authorBio').text(author.bio || 'No bio available.');
            if (author.profilePicUrl) {
                $('#authorProfilePic').attr('src', author.profilePicUrl).show();
            }
            const $socials = $('#authorSocials').empty();
            if (author.facebook) {
                $('<a></a>')
                    .attr('href', author.facebook)
                    .attr('target', '_blank')
                    .addClass('text-blue-600 hover:text-blue-800')
                    .html('<svg class="w-6 h-6" fill="currentColor" viewBox="0 0 24 24"><path d="M9 8h-3v4h3v12h5v-12h3.642l.358-4h-4v-1.667c0-.955.192-1.333 1.115-1.333h2.885v-5h-3.808c-3.596 0-5.192 1.583-5.192 4.615v3.385z"/></svg>')
                    .appendTo($socials);
            }
            if (author.instagram) {
                $('<a></a>')
                    .attr('href', author.instagram)
                    .attr('target', '_blank')
                    .addClass('text-pink-600 hover:text-pink-800')
                    .html('<svg class="w-6 h-6" fill="currentColor" viewBox="0 0 24 24"><path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.948-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/></svg>')
                    .appendTo($socials);
            }

            // Tags
            const $tags = $('#tagsContainer').empty();
            (recipe.tags || []).forEach((tag, index) => {
                $('<span></span>')
                    .addClass('bg-teal-100 text-teal-800 px-4 py-2 rounded-full text-sm font-semibold hover:bg-teal-600 hover:text-white transition opacity-0')
                    .text(tag)
                    .appendTo($tags)
                    .delay(100 * index).animate({ opacity: 1, marginLeft: 0 }, 300);
            });

            // Stats
            $('#prepareTime span:last').text((recipe.prepareTime ? recipe.prepareTime + ' min' : 'N/A'));
            $('#cookTime span:last').text((recipe.cookTime ? recipe.cookTime + ' min' : 'N/A'));
            $('#totalTime span:last').text((recipe.totalTime ? recipe.totalTime + ' min' : 'N/A'));
            $('#servings span:last').text(recipe.servings || 'N/A');
            $('#difficulty span:last').text(recipe.difficulty || 'N/A');
            $('#mealCategory span:last').text(recipe.mealCategory || 'N/A');

            // Animate stats cards
            $('.grid > div').each(function (index) {
                $(this).css({ opacity: 0 }).delay(150 * index).animate({ opacity: 1 }, 400);
            });

            // Ingredients
            const $ingredients = $('#ingredientsList').empty();
            (recipe.ingredients || []).forEach((ing, index) => {
                const id = ing.ingredientId || ''; // correct ID field
                const name = ing.ingredientName || 'Unnamed Ingredient';
                const quantity = ing.quantity || '';
                const unit = ing.unit || '';

                const $li = $('<li></li>')
                    .addClass('bg-amber-100 p-4 rounded-lg shadow-sm hover:bg-amber-400 hover:text-white hover:animate-pulse transition flex items-center gap-4 opacity-0 cursor-pointer');

                // Click only if valid id
                if (id) {
                    $li.on('click', () => {
                        window.location.href = `/Cooking/Ingredient/Details/${id}`;
                    });
                } else {
                    // If no valid id, remove pointer cursor and disable click
                    $li.css('cursor', 'default');
                }

                const $content = $('<span></span>').html(`${quantity} ${unit} <strong>${name}</strong>`);

                if (ing.coverImageUrl) {
                    $('<img>')
                        .attr('src', ing.coverImageUrl)
                        .addClass('w-12 h-12 object-cover rounded-lg')
                        .prependTo($li);
                }

                $li.append($content).appendTo($ingredients).delay(150 * index).animate({ opacity: 1 }, 400);
            });

            // Steps
            const $steps = $('#stepsList').empty();
            (recipe.steps || []).forEach((step, index) => {
                const $li = $('<li></li>').addClass('bg-yellow-50 border-l-8 border-red-500 p-5 rounded-lg shadow-md hover:bg-amber-100 transition space-y-3 hidden');
                $li.append(`<h3 class="text-2xl font-bold text-orange-700">Step ${index + 1}: ${step.title || 'Untitled'}</h3>`);
                $li.append(`<p class="text-gray-700">${step.description || 'No description.'}</p>`);

                if (step.mediaUrls && step.mediaUrls.length > 0) {
                    const $media = $('<div></div>').addClass('flex flex-wrap gap-4');
                    step.mediaUrls.forEach(url => {
                        $('<img>')
                            .attr('src', url)
                            .addClass('w-32 h-32 object-cover rounded-lg shadow-sm hover:scale-110 transition')
                            .appendTo($media);
                    });
                    $li.append($media);
                }

                $li.appendTo($steps).delay(200 * index).slideDown(400);
            });

            // Ratings
            let starsHtml = '';
            const rating = Math.round(recipe.averageRating || 0);
            for (let i = 1; i <= 5; i++) {
                starsHtml += i <= rating ? '★' : '☆';
            }
            $('#ratingStars').html(starsHtml).css({ display: 'none' }).fadeIn(600, function () {
                $(this).animate({ fontSize: '2.5rem' }, 200).animate({ fontSize: '2rem' }, 200);
            });

            // Views & Date
            $('#viewsCount').text(`Views: ${recipe.totalViews || 0}`);
            const date = recipe.createdAt ? new Date(recipe.createdAt).toLocaleDateString() : 'N/A';
            $('#createdDate').text(`Created: ${date}`);

            // Save Recipe Button
            $('#saveRecipe').on('click', function () {
                // Pulse effect
                $(this).animate({ scale: 1.2 }, 100).animate({ scale: 1 }, 100);
                Swal.fire({
                    title: 'Recipe Saved!',
                    text: 'This recipe has been added to your collection.',
                    icon: 'success',
                    confirmButtonColor: '#dc2626',
                });
                // TODO: Implement actual save functionality via API
            });

            // Show recipe
            $('#loading').hide();
            $('#recipeContainer').removeClass('opacity-0').fadeIn(800);
        },
        error: function () {
            $('#loading').text('Oops! Failed to load recipe. 😔').addClass('text-orange-700');
        }
    });

    // Show modal on donate button click
    $('#donateButton').on('click', function () {
        $('#donationModal').removeClass('hidden').addClass('flex');
    });

    // Hide modal on cancel button click
    $('#cancelDonate').on('click', function () {
        $('#donationModal').addClass('hidden').removeClass('flex');
        $('#donationForm')[0].reset(); // Reset form fields
    });

    // Handle form submission
    $('#donationForm').on('submit', function (e) {
        e.preventDefault();

        const recipeId = $('#formRecipeId').val();
        const amount = parseFloat($('#amount').val());
        const message = $('#message').val().trim();
        const $submitButton = $('#submitDonate');

        // Validate amount
        if (isNaN(amount) || amount < 1) {
            Swal.fire({
                icon: 'error',
                title: 'Invalid Amount',
                text: 'Please enter a valid amount (minimum $1.00).',
                confirmButtonColor: '#f97316' // Tailwind orange-500
            });
            return;
        }

        // Disable submit button
        $submitButton.prop('disabled', true).text('Processing...');

        // Assuming recipeId is a valid GUID string
        $.ajax({
            url: '/api/donation/' + encodeURIComponent(fetchedRecipe.id), // Pass recipeId as route parameter
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                amount: amount,
                currency: 'USD',
                donationMessage: message || null
            }),
            success: function (response) {
                if (response) {

                    window.location.href = "/Cooking/Donation/DonationConfirmation/" + response; // Redirect to PayPal approval URL
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Donation Error',
                        text: 'Failed to initiate donation.',
                        confirmButtonColor: '#f97316'
                    });
                }
            },
            error: function (xhr) {
                const errorMessage = xhr.responseJSON?.message || 'An error occurred while processing your donation.';
                Swal.fire({
                    icon: 'error',
                    title: 'Donation Error',
                    text: errorMessage,
                    confirmButtonColor: '#f97316'
                });
            },
            complete: function () {
                // Re-enable submit button
                $submitButton.prop('disabled', false).text('Submit');
            }
        });

    });

    // Fetch donations from API
    $.ajax({
        url: '/api/donations/recipe/' + recipeId,
        method: 'GET',
        success: function (data) {

            donations = data;
            if (donations.length > 0) {
                animateTicker();
            } else {
                $tickerText.text('No recent donations for this recipe.');
            }
        },
        error: function (xhr) {
            const errorMessage = xhr.responseJSON?.message || 'Failed to load donations.';
            $tickerText.text(errorMessage);
        }
    });




});

let donations = [];
// Donation ticker element
const $tickerText = $('#donation-text');
let currentIndex = 0;

// Function to create ticker sentence
function getTickerSentence(donation) {
    const donatorName = donation.donatorProfileDto?.name || 'Anonymous';
    const authorName = donation.authorProfileDto?.name || 'an author';
    const amount = donation.amount.toFixed(2);
    const message = donation.donateMessage || 'no message';
    return `${donatorName} donated $${amount} to support author ${authorName} with the message: "${message}"`;
}

// Function to animate ticker
function animateTicker() {
    // Get current donation
    const donation = donations[currentIndex];
    const sentence = getTickerSentence(donation);

    // Set initial text and position
    $tickerText.text(sentence);
    $tickerText.css({
        'position': 'relative',
        'left': '100%' // Start off-screen to the right
    });

    // Calculate animation duration based on text length (longer text = slower scroll)
    const duration = sentence.length * 300 + 1500;

    // Animate from right to left
    $tickerText.animate({
        left: '-100%' // Move off-screen to the left
    }, duration, 'linear', function () {
        // Move to next donation
        currentIndex = (currentIndex + 1) % donations.length;
        // Pause briefly before next animation
        setTimeout(animateTicker, 1000); // 1s pause between messages
    });
}