import { GoogleGenerativeAI } from "https://esm.sh/@google/generative-ai";

// Initialize the Google Generative AI client with the API key from .env
const genAI = new GoogleGenerativeAI("AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94");

let ingredient = {};


async function reviewIngredient() {
    try {
        const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

        const prompt = `
            You are an assistant reviewing a food ingredient submission for an online cooking app.

            Evaluate the ingredient based on:
            1. ✅ Name: Clear, real, and relevant food name (not spam or gibberish).
            2. ✅ Description: Meaningful, complete, not vague or AI-generated fluff.
            3. ✅ Category & Tags: Category is appropriate, and dietary flags (vegan, etc.) make sense.
            4. ✅ Nutrition: Calories, macros, and values look realistic.
            5. ✅ Details: Clear sections (title + description), not empty or repeated nonsense.
            6. ✅ No spam, offensive language, or irrelevant/unrelated info.

            Do **not** evaluate image URLs or their validity.

            Now review this ingredient JSON and return a structured evaluation:
            {
            "status": "Valid" | "Needs Fixes",
            "problems": [ "problem 1", "problem 2", ... ],
            "suggestions": [ "suggestion 1", "suggestion 2", ... ]
            }

            Ingredient JSON:
            \`\`\`json
            ${JSON.stringify(ingredient, null, 2)}
            \`\`\`
        `;

        const result = await model.generateContent(prompt);
        const response = await result.response;
        let text = response.text();

        const jsonMatch = text.match(/{[\s\S]*}/);
        if (!jsonMatch) {
            throw new Error("No valid JSON found in response");
        }

        text = jsonMatch[0]; // Extract JSON
        const review = JSON.parse(text);

        console.log("AI Review:\n", JSON.stringify(review, null, 2));

        return review;

    } catch (error) {
        console.error("Error during ingredient review:", error.message);
        return null;
    }
}


$(document).ready(function () {
    let ingredientCategoryName = "";
    let ingredientId = $("#ingredientId").text();
    console.log(ingredientId);
    // Fetch ingredient data from API
    $.ajax({
        url: `/api/ingredient/${ingredientId}/preview`,
        method: "GET",
        success: function (data) {
            console.log(data);
            ingredientCategoryName = data.categoryName;
            // Map API response to match the expected structure
            ingredient = {
                Id: data.id,
                Name: data.name,
                Description: data.description,
                CategoryName: data.categoryName,
                Calories: data.calories,
                Protein: data.protein,
                Carbohydrates: data.carbohydrates,
                Fats: data.fats,
                Sugars: data.sugars,
                Fiber: data.fiber,
                Sodium: data.sodium,
                IsVegetarian: data.isVegetarian,
                IsVegan: data.isVegan,
                IsGlutenFree: data.isGlutenFree,
                IsPescatarian: data.isPescatarian,
                CoverImageUrl: data.coverImageUrl,
                ExpirationDays: data.expirationDays,
                Details: data.details.map(detail => ({
                    Order: detail.order,
                    Title: detail.title,
                    Description: detail.description,
                    MediaUrls: detail.mediaUrls
                }))
            };

            // Populate fields with Tailwind animations
            $("#ingredient-image").attr("src", ingredient.CoverImageUrl).removeClass("scale-90 opacity-0");
            $("#ingredient-name").text(ingredient.Name).removeClass("translate-x-10 opacity-0");
            $("#ingredient-description").text(ingredient.Description).removeClass("translate-x-10 opacity-0");
            $("#category-name").text(ingredient.CategoryName);

            $("#calories").text(ingredient.Calories || "-");
            $("#protein").text(ingredient.Protein + "g" || "-");
            $("#carbohydrates").text(ingredient.Carbohydrates + "g" || "-");
            $("#fats").text(ingredient.Fats + "g" || "-");
            $("#sugars").text(ingredient.Sugars + "g" || "-");
            $("#fiber").text(ingredient.Fiber + "g" || "-");
            $("#sodium").text(ingredient.Sodium + "mg" || "-");

            $("#expiration-days").text(ingredient.ExpirationDays + " days" || "-");

            // Freshness bar logic (assuming 14 days max freshness)
            let maxDays = 14;
            let freshnessPercent = Math.min((ingredient.ExpirationDays / maxDays) * 100, 100);
            $("#freshness-bar div").css("width", freshnessPercent + "%");
            if (freshnessPercent < 30) $("#freshness-bar div").removeClass("bg-lime-500").addClass("bg-rose-500");
            else if (freshnessPercent < 60) $("#freshness-bar div").removeClass("bg-lime-500").addClass("bg-yellow-500");

            // Handle dietary preferences
            if (ingredient.IsVegetarian) $("#is-vegetarian").removeClass("hidden scale-75 opacity-0");
            if (ingredient.IsVegan) $("#is-vegan").removeClass("hidden scale-75 opacity-0");
            if (ingredient.IsGlutenFree) $("#is-gluten-free").removeClass("hidden scale-75 opacity-0");
            if (ingredient.IsPescatarian) $("#is-pescatarian").removeClass("hidden scale-75 opacity-0");

            // Populate Details with wider cards, including MediaUrls
            let detailsContainer = $("#details-container");
            ingredient.Details.forEach(detail => {
                // Build media HTML if MediaUrls exist
                let mediaHtml = '';
                if (detail.MediaUrls && detail.MediaUrls.length > 0) {
                    mediaHtml = '<div class="mt-4 flex flex-wrap gap-4">';
                    detail.MediaUrls.forEach(url => {
                        // Assuming URLs are images for now; adjust if other media types are needed
                        mediaHtml += `
                                <img src="${url}" class="cursor-pointer zoomable-img w-32 h-32 rounded-lg object-cover border-2 border-amber-200 shadow-md hover:shadow-lg transition-all duration-300" alt="${detail.Title} Media">
                            `;
                    });
                    mediaHtml += '</div>';
                }

                // Combine title, description, and media
                let detailHtml = `
                    <div class="bg-white p-6 rounded-2xl shadow-lg transform rotate-1 hover:rotate-0 transition-all duration-300 max-w-3xl mx-auto">
                        <h3 class="font-handwritten font-bold text-rose-700 text-2xl">${detail.Title}</h3>
                        <p class="text-rose-600 mt-2">${detail.Description}</p>
                        ${mediaHtml}
                    </div>`;
                detailsContainer.append(detailHtml);
            });

        },
        error: function (xhr, status, error) {
            if (xhr.status === 403) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Access Denied',
                    text: 'You do not have permission to view this ingredient.',
                    confirmButtonColor: '#3085d6', // Custom OK button color
                    background: '#fff3cd',         // Light warning background
                    color: '#856404',              // Text color
                    iconColor: '#ffc107',          // Warning icon color
                    confirmButtonText: 'Go Home'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = '/'; // Redirect to home
                    }
                });

            } else {
                console.error("Error fetching ingredient data:", error);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Failed to load ingredient data. Please try again later!'
                });
            }
        }

    });


    $(document).on('click', '.zoomable-img', function () {
        const imgSrc = $(this).attr('src');
        $('#modalImage').attr('src', imgSrc);
        $('#imageModal').removeClass('hidden');
    });

    $(document).on('click', '#modalOverlay, #closeModal', function () {
        $('#imageModal').addClass('hidden');
        $('#modalImage').attr('src', '');
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && !$('#imageModal').hasClass('hidden')) {
            $('#imageModal').addClass('hidden');
            $('#modalImage').attr('src', '');
        }
    });

    // Accept button
    $('.accept-btn').on('click', function () {
        const formattedMessage = collectEvaluationInputs();
        const newRefId = ingredientId; // Assuming ingredientId is defined elsewhere

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
        const newRefId = ingredientId; // Assuming ingredientId is defined elsewhere

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
            const review = await reviewIngredient();

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
                text: `An error occurred while reviewing the ingredient: ${error.message}`,
                confirmButtonText: 'OK'
            });
        }
    });

    // Helper function to format list with a dash before each item
    function formatListWithDashes(list) {
        return list.map(item => `- ${item}`).join('\n');
    }

});

function collectEvaluationInputs() {
    const adminResponse = $('#admin-response').val().trim();
    const problems = $('#problems').val().trim();
    const suggestions = $('#suggestions').val().trim();

    return `|${adminResponse}|${problems}|${suggestions}|`;
}
