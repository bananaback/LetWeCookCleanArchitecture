$(document).ready(function () {
    function formatRangeText(min, max, unitSymbol) {
        let left = `${min}`;
        let leftPadded = left.padStart(5, ' ');
        let right = `${max}`;
        return `${leftPadded} ${unitSymbol} - ${right} ${unitSymbol}`;
    }

    function initSlider(sliderId, amountId, min, max, values, unitSymbol, formatFunc) {
        $(sliderId).slider({
            range: true,
            min: min,
            max: max,
            values: values,
            slide: function (event, ui) {
                $(amountId).val(formatFunc(ui.values[0], ui.values[1], unitSymbol));
            }
        });

        let initialMin = $(sliderId).slider("values", 0);
        let initialMax = $(sliderId).slider("values", 1);
        $(amountId).val(formatFunc(initialMin, initialMax, unitSymbol));
    }

    // Init Servings slider
    initSlider(
        "#servings-slider-range",
        "#servings-amount",
        0,
        500,
        [0, 500],
        "👤",
        formatRangeText
    );

    // Init Prepare Time slider
    initSlider(
        "#prepare-slider-range",
        "#prepare-amount",
        0,
        500,
        [0, 500],
        "⏱️",
        formatRangeText
    );

    // Init Cook Time slider
    initSlider(
        "#cook-slider-range",
        "#cook-amount",
        0,
        500,
        [0, 500],
        "🔥",
        formatRangeText
    );

    // Init Rating slider
    initSlider(
        "#rating-slider-range",
        "#rating-amount",
        0,
        5,
        [0, 5],
        "⭐",
        formatRangeText
    );

    // Init Views slider
    initSlider(
        "#views-slider-range",
        "#views-amount",
        0,
        10000,
        [0, 10000],
        "👀",
        formatRangeText
    );

    $('#difficulty-container').on('click', '.difficulty-chip', function () {
        $(this).toggleClass('selected')
            .toggleClass('bg-blue-100')
            .toggleClass('bg-blue-300');
        showSelectedDifficulty();
    });

    $('#category-container').on('click', '.category-chip', function () {
        $(this).toggleClass('selected')
            .toggleClass('bg-blue-100')
            .toggleClass('bg-blue-300');
        showSelectedCategory();
    });

    $('#tag-container').on('click', '.tag-chip', function () {
        $(this).toggleClass('selected')
            .toggleClass('bg-blue-100')
            .toggleClass('bg-blue-300');
        showSelectedTag();
    });


    $('#created-at-min').on('change', function () {
        let minDate = $(this).val();
        let maxDate = $('#created-at-max').val();
        if (minDate && maxDate && new Date(minDate) > new Date(maxDate)) {
            Swal.fire({
                title: 'Invalid Date Range',
                text: 'The minimum date cannot be later than the maximum date.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            $(this).val('');
        }
    });


    $('#created-at-max').on('change', function () {
        let minDate = $('#created-at-min').val();
        let maxDate = $(this).val();
        if (minDate && maxDate && new Date(minDate) > new Date(maxDate)) {
            Swal.fire({
                title: 'Invalid Date Range',
                text: 'The maximum date cannot be earlier than the minimum date.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            $(this).val('');
        }
    });

    $('#updated-at-min').on('change', function () {
        let minDate = $(this).val();
        let maxDate = $('#updated-at-max').val();
        if (minDate && maxDate && new Date(minDate) > new Date(maxDate)) {
            Swal.fire({
                title: 'Invalid Date Range',
                text: 'The minimum date cannot be later than the maximum date.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            $(this).val('');
        }
    });

    $('#updated-at-max').on('change', function () {
        let minDate = $('#updated-at-min').val();
        let maxDate = $(this).val();
        if (minDate && maxDate && new Date(minDate) > new Date(maxDate)) {
            Swal.fire({
                title: 'Invalid Date Range',
                text: 'The maximum date cannot be earlier than the minimum date.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            $(this).val('');
        }
    });

    // Initialize all sort criteria
    $('.sort-criteria').each(function () {
        let $criteriaBlock = $(this);
        let $toggle = $criteriaBlock.find('.sort-toggle');
        let $direction = $criteriaBlock.find('.sort-direction');
        let criteriaName = $criteriaBlock.data('criteria');

        // Toggle switch change
        $toggle.on('change', function () {
            if ($(this).is(':checked')) {
                $direction.addClass('flex').removeClass('hidden');
            } else {
                $direction.addClass('hidden').removeClass('flex');
            }
            collectBrowsingOptions();
            loadRecipes(browsingOptions);
        });

        // Direction click (ascending/descending)
        $direction.on('click', function () {
            $(this).toggleClass('rotate-180');
            let direction = $(this).data('direction');
            let newDirection = direction === 'descending' ? 'ascending' : 'descending';
            $(this).data('direction', newDirection);
            collectBrowsingOptions();

            loadRecipes(browsingOptions);
        });
    });


    $('#apply-filter-btn').on('click', function () {

        collectBrowsingOptions();

        console.log('Browsing Options:', browsingOptions);

        loadRecipes(browsingOptions);
    });

    $.ajax({
        url: '/api/meal-category-enums',
        method: 'GET',
        success: function (data) {
            let container = $("#category-container");
            container.empty(); // Clear existing chips

            // slice the enum named UNKNOWN
            data = data.filter(function (category) {
                return category !== "Unknown";
            });

            data.forEach(function (category) {
                let chip = $(`
                    <div class="category-chip cursor-pointer px-2 py-1 text-xs bg-blue-100 border border-blue-300 rounded-full hover:bg-blue-200 transition ease-in-out duration-200">
                        ${category}
                    </div>
                `);

                container.append(chip);
            });


        },
        error: function (xhr, status, error) {
            console.error("Failed to fetch categories:", error);
        }
    });

    $.ajax({
        url: '/api/difficulty-enums',
        method: 'GET',
        success: function (data) {
            let container = $("#difficulty-container");
            container.empty(); // Clear existing chips
            // slice the enum named UNKNOWN
            data = data.filter(function (difficulty) {
                return difficulty !== "UNKNOWN";
            });

            data.forEach(function (difficulty) {
                let chip = $(`
                    <div class="difficulty-chip cursor-pointer px-2 py-1 text-xs bg-blue-100 border border-blue-300 rounded-full hover:bg-blue-200">
                        ${difficulty}
                    </div>
                `);
                container.append(chip);
            });
        },
        error: function (xhr, status, error) {
            console.error("Failed to fetch difficulty levels:", error);
        }
    });

    $.ajax({
        url: '/api/recipe-tags',
        method: 'GET',
        success: function (data) {
            let container = $("#tag-container");
            container.empty(); // Clear existing chips

            data.forEach(function (tag) {
                let chip = $(`
                    <div class="tag-chip cursor-pointer px-2 py-1 text-xs bg-blue-100 border border-blue-300 rounded-full hover:bg-blue-200 transition ease-in-out duration-200" data-id="${tag.id}">
                        ${tag.name}
                    </div>
                `);

                container.append(chip);
            });
        },
        error: function (xhr, status, error) {
            console.error("Failed to fetch tags:", error);
        }
    });

    loadRecipes(browsingOptions);

    // add event listener to pagination buttons
    $('#pagination-button-container').on('click', '.pagination-button', function () {
        let page = $(this).data('page');
        browsingOptions.page = page;
        loadRecipes(browsingOptions);
    });

    // add event listener to items per page select
    $('#items-per-page-select').on('change', function () {
        browsingOptions.itemsPerPage = parseInt($(this).val(), 10);
        browsingOptions.page = 1; // Reset to page 1 when items per page changes
        loadRecipes(browsingOptions);
    });

    // add event listener to delete recipe button
    $('#recipe-card-container').on('click', '.delete-recipe-btn', function () {
        let recipeId = $(this).data('id');
        let confirmation = false;
        Swal.fire({
            title: 'Are you sure?',
            text: "Do you really want to delete this recipe? This action cannot be undone.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                confirmation = true;
                deleteRecipe(recipeId);
            }
        });
        if (confirmation) {
            deleteRecipe(recipeId);
        }
    });
});

function deleteRecipe(recipeId) {
    // call the API to delete the recipe and reload the recipes
    $.ajax({
        url: `/api/recipes/${recipeId}`,
        type: 'DELETE',
        success: function (response) {
            console.log('Recipe deleted successfully:', response);
            // Reload the recipes after deletion
            loadRecipes(browsingOptions);
        },
        error: function (error) {
            console.error('Error deleting recipe:', error);
            Swal.fire({
                title: 'Error',
                text: 'Failed to delete the recipe. Please try again.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    });
}

function loadRecipes(browsingOptions) {
    console.log('Browsing Options:', browsingOptions);

    $.ajax({
        url: '/api/personal-recipes-browser',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(browsingOptions),
        success: function (response) {
            console.log('Filtered recipes:', response);

            let recipesHtml = $('#recipe-card-container');
            recipesHtml.empty(); // Clear existing cards

            // Handle zero recipes case
            if (response.items.length === 0) {
                recipesHtml.append(`
                    <div class="text-center text-gray-500 py-10 w-full col-span-3">
                        <p class="text-lg font-medium">😞 No recipes found matching your filters.</p>
                        <p class="text-sm mt-2">Try adjusting your search criteria or browse other categories!</p>
                    </div>
                `);
            }

            response.items.forEach(function (recipe) {
                recipesHtml.append(
                    `<div class="recipe-card bg-white rounded-lg shadow-md overflow-hidden border border-gray-200 w-64 transition-all duration-300 hover:border-orange-500 hover:shadow-lg">
                        <!-- Image -->
                        <div class="relative h-32 w-full">
                            <img src="${recipe.coverImage || '/img/default-recipe.jpg'}" alt="${recipe.name}" 
                                class="w-full h-full object-cover">
                            <div class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/60 to-transparent px-2 py-1">
                                <h3 class="text-white font-semibold text-sm truncate">${recipe.name}</h3>
                            </div>
                        </div>

                        <!-- Info -->
                        <div class="p-3 space-y-2">
                            <p class="text-gray-700 text-xs h-8 overflow-hidden truncate">${recipe.description}</p>

                            <div class="flex justify-between text-xs">
                                <span class="bg-green-100 text-green-700 px-1.5 py-0.5 rounded-full truncate">${recipe.mealCategory || 'Uncategorized'}</span>
                                <span class="bg-yellow-100 text-yellow-700 px-1.5 py-0.5 rounded-full truncate">${recipe.difficulty || 'Not specified'}</span>
                            </div>

                            <div class="space-y-0.5 text-xs text-gray-600">
                                <div class="flex justify-between"><span>🥣 Prep</span><span>${recipe.prepareTime}m</span></div>
                                <div class="flex justify-between"><span>🔥 Cook</span><span>${recipe.cookTime}m</span></div>
                                <div class="flex justify-between"><span>⏱️ Total</span><span>${recipe.totalTime}m</span></div>
                            </div>

                            <div class="flex justify-between text-xs text-gray-600">
                                <span>👥 ${recipe.servings}</span>
                                <span>👁️ ${recipe.totalViews}</span>
                            </div>

                            <div class="flex justify-between text-xs text-gray-600">
                                <span>⭐ ${recipe.averageRating.toFixed(1)}/5</span>
                                <span>(${recipe.totalRatings})</span>
                            </div>

                            <!-- Actions -->
                            <div class="flex flex-col space-y-1">
                                <a href="https://localhost:7212/Cooking/Recipe/Details/${recipe.id}" 
                                   class="block text-center bg-orange-500 hover:bg-orange-600 text-white py-1 rounded-md text-xs font-medium transition">
                                   🍽️ View
                                </a>
                                <a href="https://localhost:7212/Cooking/Recipe/Update/${recipe.id}"  class="edit-recipe-btn text-center bg-blue-500 hover:bg-blue-600 text-white py-1 rounded-md text-xs font-medium transition">✏️ Edit</a>
                                <button data-id="${recipe.id}" class="delete-recipe-btn bg-red-500 hover:bg-red-600 text-white py-1 rounded-md text-xs font-medium transition">🗑️ Delete</button>
                            </div>
                        </div>
                    </div>`
                );
            });

            // Pagination buttons
            let paginationButtonContainer = $("#pagination-button-container");
            paginationButtonContainer.empty(); // Clear existing pagination buttons
            let totalPages = Math.ceil(response.totalCount / response.pageSize);

            for (let i = 1; i <= totalPages; i++) {
                let button = $(`
                    <button class="pagination-button px-2 py-1 text-xs rounded-full hover:bg-blue-600 ${i === browsingOptions.page ? 'bg-blue-700 text-white' : 'bg-blue-500 text-white'}" data-page="${i}">
                        ${i}
                    </button>
                `);
                paginationButtonContainer.append(button);
            }
        },
        error: function (error) {
            console.error('Error fetching filtered recipes:', error);
        }
    });
}


function showSelectedDifficulty() {
    let selectedDifficulties = $('.difficulty-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');
}

function showSelectedCategory() {
    let selectedCategories = $('.category-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');

}

function showSelectedTag() {
    let selectedTags = $('.tag-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');

}


function getSliderRange(sliderId) {
    let min = $(sliderId).slider("values", 0);
    let max = $(sliderId).slider("values", 1);
    return { min, max };
}


function collectBrowsingOptions() {
    // Collect name search
    let nameSearch = $('#name-search').val().trim();
    browsingOptions.nameSearch.name = nameSearch;

    let textMatch = $('#text-match-option').val();
    browsingOptions.nameSearch.textMatch = textMatch;

    let selectedDifficulties = $('.difficulty-chip.selected').map(function () {
        return $(this).text().trim();
    }).get();

    browsingOptions.difficulties = selectedDifficulties;  // this will be [] when none selected


    let selectedCategories = $('.category-chip.selected').map(function () {
        return $(this).text().trim();
    }).get();

    browsingOptions.categories = selectedCategories;

    let selectedTags = $('.tag-chip.selected').map(function () {
        return $(this).text().trim();
    }).get();

    browsingOptions.tags = selectedTags;


    browsingOptions.servings = getSliderRange("#servings-slider-range");
    browsingOptions.prepareTime = getSliderRange("#prepare-slider-range");
    browsingOptions.cookTime = getSliderRange("#cook-slider-range");
    browsingOptions.rating = getSliderRange("#rating-slider-range");
    browsingOptions.views = getSliderRange("#views-slider-range");

    browsingOptions.createdAt.min = $('#created-at-min').val() || null;
    browsingOptions.createdAt.max = $('#created-at-max').val() || null;

    // check valid date range
    if (browsingOptions.createdAt.min && browsingOptions.createdAt.max && new Date(browsingOptions.createdAt.min) > new Date(browsingOptions.createdAt.max)) {
        Swal.fire({
            title: 'Invalid Date Range',
            text: 'The minimum date cannot be later than the maximum date.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        // reset the min date to empty
        browsingOptions.createdAt.min = null;
        // reset the max date to empty
        browsingOptions.createdAt.max = null;
        return;
    }

    browsingOptions.updatedAt.min = $('#updated-at-min').val() || null;
    browsingOptions.updatedAt.max = $('#updated-at-max').val() || null;

    // check valid date range
    if (browsingOptions.updatedAt.min && browsingOptions.updatedAt.max && new Date(browsingOptions.updatedAt.min) > new Date(browsingOptions.updatedAt.max)) {
        Swal.fire({
            title: 'Invalid Date Range',
            text: 'The minimum date cannot be later than the maximum date.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        // reset the min date to empty
        browsingOptions.updatedAt.min = null;
        // reset the max date to empty
        browsingOptions.updatedAt.max = null;
        return;
    }


    // check different of prev and current items per page, if different, reset page to 1
    if (browsingOptions.itemsPerPage !== parseInt($('#items-per-page-select').val(), 10) || browsingOptions.page !== 1) {
        browsingOptions.page = 1;
    }

    browsingOptions.itemsPerPage = parseInt($('#items-per-page-select').val(), 10) || 3;

    browsingOptions.sortOptions = [];
    $('.sort-criteria').each(function () {
        let $criteriaBlock = $(this);
        let $toggle = $criteriaBlock.find('.sort-toggle');
        let $direction = $criteriaBlock.find('.sort-direction');
        let criteriaName = $criteriaBlock.data('criteria');
        let isChecked = $toggle.is(':checked');
        let direction = $direction.data('direction');

        if (isChecked) {
            browsingOptions.sortOptions.push({
                criteria: criteriaName,
                direction: direction
            });
        }
    });
}

let browsingOptions = {
    nameSearch: {
        name: '',
        textMatch: 'exact'
    },
    difficulties: [],
    categories: [],
    tags: [],
    servings: {
        min: 0,
        max: 500
    },
    prepareTime: {
        min: 0,
        max: 300
    },
    cookTime: {
        min: 0,
        max: 300
    },
    rating: {
        min: 0,
        max: 5
    },
    views: {
        min: 0,
        max: 10000
    },
    createdAt: {
        min: null,
        max: null
    },
    updatedAt: {
        min: null,
        max: null
    },
    createdByUsername: '',
    sortOptions: [

    ],
    itemsPerPage: 3,
    page: 1
};