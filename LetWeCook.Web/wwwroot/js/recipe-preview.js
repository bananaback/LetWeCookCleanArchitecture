import { GoogleGenerativeAI } from "https://esm.sh/@google/generative-ai";

// Initialize the Google Generative AI client with the API key from .env
const genAI = new GoogleGenerativeAI("AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94");

let fetchedRecipe = {};

$(document).ready(function () {
    const recipeId = $('#recipeId').val();

    $.ajax({
        url: `/api/recipe-preview/${recipeId}`,
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
                const name = ing.ingredientName || 'Unnamed Ingredient';
                const quantity = ing.quantity || '';
                const unit = ing.unit || '';
                const $li = $('<li></li>')
                    .addClass('bg-amber-100 p-4 rounded-lg shadow-sm hover:bg-amber-400 hover:text-white hover:animate-pulse transition flex items-center gap-4 opacity-0');
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

    $('.review-ai-btn').on('click', async function () {
        const message = $('#admin-response').val().trim();
        console.log("🤖 Reviewing message with AI:", message);

        // Show loading message
        const swalLoading = Swal.fire({
            title: 'Reviewing...',
            text: 'Please wait while we review the ingredient.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading(); // Show the loading spinner
            }
        });

        try {
            // Perform the AI review
            const review = await reviewRecipe();

            // Close the loading alert and show success
            swalLoading.close();

            Swal.fire({
                icon: 'success',
                title: 'Review Completed',
                text: `Review status: ${review.status}`,
                confirmButtonText: 'OK',
                customClass: {
                    confirmButton: 'bg-rose-500 hover:bg-rose-600 text-white font-semibold rounded-xl px-6 py-2 focus:outline-none focus:ring-2 focus:ring-rose-300'
                },
                buttonsStyling: false // Disable default styling to apply Tailwind
            }).then(() => {
                // Populate the Problems and Suggestions textareas with formatted data
                $('#problems').val(formatListWithDashes(review.problems));
                $('#suggestions').val(formatListWithDashes(review.suggestions));
            });


        } catch (error) {
            // Close loading alert on error and show error message
            swalLoading.close();

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: `An error occurred while reviewing the recipe: ${error.message}`,
                confirmButtonText: 'OK'
            });
        }
    });

    // Accept button
    $('.accept-btn').on('click', function () {
        const formattedMessage = collectEvaluationInputs();
        const newRefId = fetchedRecipe.id;

        $.ajax({
            url: `/api/evaluation/accept/${newRefId}`,
            method: 'POST',
            data: { responseMessage: formattedMessage },
            success: function () {
                Swal.fire({
                    title: '✅ Accepted!',
                    text: 'The request has been successfully accepted.',
                    icon: 'success',
                    confirmButtonText: 'Got it!',
                    confirmButtonColor: '#28a745',
                    background: 'rgba(255, 255, 255, 0.8)',  // Transparent background
                    color: '#333',  // Text color to make it readable
                    backdrop: 'rgba(0, 0, 0, 0.3)',  // Slight dimming for the backdrop
                });
            },
            error: function (xhr) {
                const error = xhr.responseJSON?.error || 'Something went wrong';
                Swal.fire({
                    title: '❌ Oops!',
                    text: error,
                    icon: 'error',
                    confirmButtonText: 'Close',
                    confirmButtonColor: '#d33',
                    background: 'rgba(255, 255, 255, 0.9)',  // Transparent background
                    color: '#333',  // Text color for clarity
                    backdrop: 'rgba(0, 0, 0, 0.3)',  // Slight dimming for the backdrop
                });
            }
        });
    });

    // Reject button
    $('.reject-btn').on('click', function () {
        const formattedMessage = collectEvaluationInputs();
        const newRefId = fetchedRecipe.id;

        $.ajax({
            url: `/api/evaluation/reject/${newRefId}`,
            method: 'POST',
            data: { responseMessage: formattedMessage },
            success: function () {
                Swal.fire({
                    title: '🚫 Rejected!',
                    text: 'The request has been rejected as per your response.',
                    icon: 'warning',
                    confirmButtonText: 'Understood',
                    confirmButtonColor: '#dc3545',
                    background: 'rgba(255, 255, 255, 0.8)',  // Transparent background
                    color: '#333',  // Text color to make it readable
                    backdrop: 'rgba(0, 0, 0, 0.3)',  // Slight dimming for the backdrop
                });
            },
            error: function (xhr) {
                const error = xhr.responseJSON?.error || 'Something went wrong';
                Swal.fire({
                    title: '❌ Oops!',
                    text: error,
                    icon: 'error',
                    confirmButtonText: 'Close',
                    confirmButtonColor: '#d33',
                    background: 'rgba(255, 255, 255, 0.9)',  // Transparent background
                    color: '#333',  // Text color for clarity
                    backdrop: 'rgba(0, 0, 0, 0.3)',  // Slight dimming for the backdrop
                });
            }
        });
    });

});

async function reviewRecipe() {
    try {
        const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

        const prompt = `
            You are an assistant reviewing a recipe submission for an online cooking app.

            Evaluate the recipe based on:
            1. ✅ Name: Clear, real, and relevant recipe name (not spam or gibberish).
            2. ✅ Description: Meaningful, complete, not vague or AI-generated fluff.
            3. ✅ Recipe Details: Cooking times, servings, difficulty, and meal category are reasonable.
            4. ✅ Tags: Cuisine types and other tags are appropriate and consistent.
            5. ✅ Ingredients: Contains realistic ingredients with proper quantities and units.
            6. ✅ Steps: Instructions are clear, complete, and in logical order with appropriate titles and descriptions.
            7. ✅ Overall Recipe Quality: Recipe is complete, executable, and makes culinary sense.
            8. ✅ No spam, offensive language, or irrelevant/unrelated info.

            Do **not** evaluate image URLs or their validity.

            IMPORTANT: Your response MUST be ONLY a valid JSON object with exactly this format:
            {
              "status": "Valid" or "Needs Fixes",
              "problems": ["problem 1", "problem 2", ...],
              "suggestions": ["suggestion 1", "suggestion 2", ...]
            }

            If there are no problems, return an empty array for "problems".
            If there are no suggestions, return an empty array for "suggestions".
            Do not include any text before or after the JSON.
            Do not include any markdown formatting or code blocks.
            Return ONLY the raw JSON object.

            Recipe JSON:
            \`\`\`json
            ${JSON.stringify(fetchedRecipe, null, 2)}
            \`\`\`
        `;

        const result = await model.generateContent(prompt);
        const response = await result.response;
        let text = response.text();

        try {
            // Try parsing the response directly first
            const review = JSON.parse(text);

            // Validate that the response has the required fields
            if (typeof review !== 'object' || review === null) {
                throw new Error("Response is not an object");
            }

            if (!review.hasOwnProperty('status') ||
                !review.hasOwnProperty('problems') ||
                !review.hasOwnProperty('suggestions')) {
                throw new Error("Response missing required fields");
            }

            // Ensure status is valid
            if (review.status !== "Valid" && review.status !== "Needs Fixes") {
                review.status = "Needs Fixes"; // Default to needs fixes if invalid
            }

            // Ensure arrays are arrays
            if (!Array.isArray(review.problems)) {
                review.problems = [];
            }

            if (!Array.isArray(review.suggestions)) {
                review.suggestions = [];
            }

            return review;
        } catch (parseError) {
            // If direct parsing fails, try to extract JSON
            const jsonMatch = text.match(/{[\s\S]*}/);
            if (!jsonMatch) {
                // If no JSON found, return a default response
                console.error("No valid JSON found in response:", text);
                return {
                    status: "Needs Fixes",
                    problems: ["AI response format error"],
                    suggestions: ["Please try again"]
                };
            }

            try {
                const extractedJson = JSON.parse(jsonMatch[0]);

                // Validate and fix the extracted JSON
                if (!extractedJson.hasOwnProperty('status')) {
                    extractedJson.status = "Needs Fixes";
                } else if (extractedJson.status !== "Valid" && extractedJson.status !== "Needs Fixes") {
                    extractedJson.status = "Needs Fixes";
                }

                if (!extractedJson.hasOwnProperty('problems') || !Array.isArray(extractedJson.problems)) {
                    extractedJson.problems = [];
                }

                if (!extractedJson.hasOwnProperty('suggestions') || !Array.isArray(extractedJson.suggestions)) {
                    extractedJson.suggestions = [];
                }

                return extractedJson;
            } catch (extractError) {
                console.error("Error parsing extracted JSON:", extractError.message);
                return {
                    status: "Needs Fixes",
                    problems: ["AI response format error"],
                    suggestions: ["Please try again"]
                };
            }
        }

    } catch (error) {
        console.error("Error during recipe review:", error.message);
        return null;
    }
}

// Helper function to format list with a dash before each item
function formatListWithDashes(list) {
    return list.map(item => `- ${item}`).join('\n');
}

function collectEvaluationInputs() {
    const adminResponse = $('#admin-response').val().trim();
    const problems = $('#problems').val().trim();
    const suggestions = $('#suggestions').val().trim();

    return `|${adminResponse}|${problems}|${suggestions}|`;
}