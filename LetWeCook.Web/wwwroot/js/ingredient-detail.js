$(document).ready(function () {
    let ingredientCategoryName = "";
    let ingredientId = $("#ingredientId").text();
    console.log(ingredientId);
    // Fetch ingredient data from API
    $.ajax({
        url: `https://localhost:7212/api/ingredients/${ingredientId}`,
        method: "GET",
        success: function (data) {
            ingredientCategoryName = data.categoryName;
            // Map API response to match the expected structure
            let ingredient = {
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
                                <img src="${url}" class="zoomable-img w-32 h-32 rounded-lg object-cover border-2 border-amber-200 shadow-md hover:shadow-lg transition-all duration-300" alt="${detail.Title} Media">
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
            console.error("Error fetching ingredient data:", error);
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to load ingredient data. Please try again later!'
            });
        }
    });

    // Fetch random ingredients
    $.ajax({
        url: "/api/ingredients", // Use the correct endpoint
        method: "GET",
        data: { count: 5 }, // Pass count as query parameter
        success: function (data) {
            $("#loading-message").hide();
            data.forEach((ingredient, index) => {
                const tiltClass = index % 2 === 0 ? "-rotate-2" : "rotate-2"; // Alternate tilt

                $("#ingredients-container").append(`
                    <a href="https://localhost:7212/Cooking/Ingredient/Details/${ingredient.id}" class="block">
                        <div class="bg-white p-4 rounded-lg shadow-md flex items-center gap-3 transform ${tiltClass} hover:rotate-0 transition-all duration-300 cursor-pointer border border-amber-200 hover:border-rose-300 max-w-xs mx-auto">
                            <img src="${ingredient.coverImageUrl || 'https://via.placeholder.com/100'}"
                                class="w-1/4 h-1/4 rounded-md object-cover border border-lime-200 shadow-sm" alt="${ingredient.name}">
                            <div class="flex-1 min-w-0">
                                <h4 class="text-md font-bold text-rose-700">${ingredient.name}</h4>
                                <p class="text-rose-600 text-sm line-clamp-2">${ingredient.description || 'No description available'}</p>
                            </div>
                        </div>
                    </a>
                `);
            });
        },
        error: function () {
            $("#loading-message").text("Failed to load ingredients.");
        }
    });

    // Fetch category-based ingredients
    $.ajax({
        url: "/api/ingredients", // Use the same endpoint
        method: "GET",
        data: { category: ingredientCategoryName, count: 5 }, // Pass category and count as query parameters
        success: function (data) {
            $("#category-loading-message").hide();
            data.forEach((ingredient, index) => {
                const tiltClass = index % 2 === 0 ? "-rotate-2" : "rotate-2"; // Alternate tilt

                $("#category-ingredients-container").append(`
                    <a href="https://localhost:7212/Cooking/Ingredient/Details/${ingredient.id}" class="block">
                        <div class="bg-white p-4 rounded-lg shadow-md flex items-center gap-3 transform ${tiltClass} hover:rotate-0 transition-all duration-300 cursor-pointer border border-amber-200 hover:border-rose-300 max-w-xs mx-auto">
                            <img src="${ingredient.coverImageUrl || 'https://via.placeholder.com/100'}"
                                class="w-1/4 h-1/4 rounded-md object-cover border border-lime-200 shadow-sm" alt="${ingredient.name}">
                            <div class="flex-1 min-w-0">
                                <h4 class="text-md font-bold text-rose-700">${ingredient.name}</h4>
                                <p class="text-rose-600 text-sm line-clamp-2">${ingredient.description || 'No description available'}</p>
                            </div>
                        </div>
                    </a>
                `);
            });
        },
        error: function () {
            $("#category-loading-message").text("Failed to load category ingredients.");
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

});