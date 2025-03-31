/** ========================== GLOBAL VARIABLES ========================== */
let ingredientCategories = [];
let selectedCategories = new Set();
let ingredientList = [];
let categoryHashMap = {};
let pagination = {
    totalItems: 0,
    itemsPerPage: parseInt($("#itemsPerPage").val()) || 9, // Default to 9
    currentPage: 1,
    totalPages: 1,
    minPage: 1,
    maxPage: 1
};

/** ========================== FETCH FUNCTIONS ========================== */

function fetchIngredientCategories() {
    $.ajax({
        url: '/api/ingredient-categories',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log("Categories fetched:", data);
            ingredientCategories = data;
            renderIngredientCategories(data);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching categories:", status, error);
        }
    });
}

function fetchIngredientsOverview() {
    $.ajax({
        url: '/api/ingredients/overview',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log("Ingredients fetched:", data);
            ingredientList = data;
            groupIngredientsByCategory();
            updatePagination();

        },
        error: function (xhr, status, error) {
            console.error("Error fetching ingredients:", status, error);
        }
    });
}

/** ========================== RENDER FUNCTIONS ========================== */

function renderIngredientCategories(categories) {
    const $categoryList = $('#categoryList');
    $categoryList.empty();

    $.each(categories, function (index, category) {
        const rotation = (index % 6) - 3; // Rotate between -3 and 2
        const buttonHtml = `
        <div class="relative group">
            <button class="category-btn px-5 py-2 bg-emerald-500 text-white rounded-full hover:bg-emerald-600 transition duration-300 shadow transform rotate-${rotation} hover:rotate-0"
                data-name="${category.name}">
                ${category.name}
            </button>
            <div class="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2 hidden group-hover:block bg-teal-700 text-white text-sm p-2 rounded-lg w-48 text-center shadow-md z-30 pointer-events-none">
                ${category.description || 'No description available'}
            </div>
        </div>`;
        $categoryList.append(buttonHtml);
    });
}

function groupIngredientsByCategory() {
    categoryHashMap = {}; // Reset

    ingredientList.forEach(ingredient => {
        const categoryName = ingredient.categoryName;
        if (!categoryHashMap[categoryName]) {
            categoryHashMap[categoryName] = [];
        }
        categoryHashMap[categoryName].push(ingredient);
    });

}

function updatePagination() {
    let ingredientPool = [];

    // Determine the ingredient pool based on selected categories
    if (selectedCategories.size === 0) {
        ingredientPool = ingredientList; // Use all ingredients
    } else {
        selectedCategories.forEach(category => {
            if (categoryHashMap[category]) {
                ingredientPool = ingredientPool.concat(categoryHashMap[category]);
            }
        });
    }

    // Update pagination based on the filtered pool
    pagination.totalItems = ingredientPool.length;
    pagination.totalPages = Math.max(1, Math.ceil(pagination.totalItems / pagination.itemsPerPage));
    pagination.currentPage = Math.min(pagination.currentPage, pagination.totalPages);
    pagination.maxPage = pagination.totalPages;

    console.log("Updated Pagination:", pagination);
    console.log("Current Ingredient Pool:", ingredientPool);
    updatePaginationButtons();
    updateResults();
}


/** ========================== EVENT LISTENERS ========================== */

function setupEventListeners() {
    $(document).on('click', '.category-btn', function () {
        const categoryName = $(this).data('name');

        if (selectedCategories.has(categoryName)) {
            selectedCategories.delete(categoryName);
            $(this).removeClass('bg-emerald-700').addClass('bg-emerald-500');
            $(`#selectedTags span[data-name="${categoryName}"]`).remove();
        } else {
            selectedCategories.add(categoryName);
            $(this).removeClass('bg-emerald-500').addClass('bg-emerald-700');
            addSelectedCategoryTag(categoryName);
        }
        updatePagination();

    });

    $(document).on('click', '.remove-tag', function () {
        const categoryName = $(this).parent().data('name');
        selectedCategories.delete(categoryName);
        $(`.category-btn[data-name="${categoryName}"]`).removeClass('bg-emerald-700').addClass('bg-emerald-500');
        $(this).parent().remove();
        updatePagination();
    });

    $("#itemsPerPage").on("change", function () {
        pagination.itemsPerPage = parseInt($(this).val());
        pagination.currentPage = 1; // Reset to first page
        updatePagination();
    });

    // 🔥 Go to Page Event Listener 🔥
    $("#goToPageBtn").on("click", function () {
        let page = parseInt($("#goToPageInput").val());

        // 🚨 Validate Input 🚨
        if (isNaN(page) || page < 1 || page > pagination.totalPages) {
            Swal.fire({
                icon: "warning",
                title: "Invalid Page Number!",
                text: `Please enter a number between 1 and ${pagination.totalPages}.`,
                footer: "<b>Try again with a valid page! 🚀</b>",
                showConfirmButton: true,
                confirmButtonText: "Got it! 👍",
                confirmButtonColor: "#22c55e", // Emerald green
                background: "#fef3c7", // Light amber background
                color: "#92400e", // Deep amber text
                toast: true,
                position: "top-end",
                timer: 3000,
                timerProgressBar: true
            });

        } else {
            // ✅ Update Page & BOOM 💥
            pagination.currentPage = page;
            updatePagination();
        }
    });

    // Support Enter key for input field
    $("#goToPageInput").on("keypress", function (event) {
        if (event.which === 13) { // Enter key
            $("#goToPageBtn").click();
        }
    });

    $("#searchInput").on("input", function () {
        const query = $(this).val().trim(); // Get the input value and trim whitespace

        console.log("Search Input Changed:", query);

        // Call your function here (e.g., fuzzy match, update results, etc.)
        if (query.length > 0) {
            performSearch(query); // Example function to handle search
        } else {
            clearSuggestions(); // Example function to clear results
        }
    });
}

/** ========================== HELPER FUNCTIONS ========================== */

function addSelectedCategoryTag(categoryName) {
    const randomRotation = Math.floor(Math.random() * 7) - 3;
    $('#selectedTags').append(`
        <span class="selected-tag bg-sky-100 text-sky-800 px-4 py-1 rounded-full flex items-center shadow-sm transform rotate-${randomRotation}"
            data-name="${categoryName}">
            ${categoryName}
            <button class="remove-tag ml-2 text-sky-600 hover:text-sky-900 font-bold transform -rotate-${randomRotation}">×</button>
        </span>`);
}

function updatePaginationButtons() {
    const $paginationContainer = $('.pagination-container'); // Adjust if needed
    $paginationContainer.empty(); // Clear existing buttons

    // Function to create a button with proper styles
    function createButton(text, isDisabled, targetPage) {
        return $('<button>')
            .text(text)
            .addClass('px-4 py-2 rounded-full transition duration-300 ' +
                (isDisabled
                    ? 'bg-gray-300 text-gray-500 opacity-50 cursor-not-allowed' // Disabled styles
                    : 'bg-amber-200 text-emerald-800 hover:bg-amber-300 hover:text-emerald-900 cursor-pointer')) // Enabled styles
            .prop('disabled', isDisabled)
            .click(() => {
                if (!isDisabled) {
                    pagination.currentPage = targetPage; // Update current page
                    updatePagination(); // Recalculate pagination and refresh UI
                }
            });
    }

    // Create buttons with correct enabled/disabled states
    const firstBtn = createButton('First', pagination.currentPage === 1, 1);
    const prevBtn = createButton('Prev', pagination.currentPage === 1, pagination.currentPage - 1);
    const currentPageDisplay = $('<span>')
        .text(`Page ${pagination.currentPage} of ${pagination.totalPages}`)
        .addClass('px-4 py-2 bg-emerald-500 text-white rounded-full font-bold shadow-md');

    const nextBtn = createButton('Next', pagination.currentPage === pagination.totalPages, pagination.currentPage + 1);
    const lastBtn = createButton('Last', pagination.currentPage === pagination.totalPages, pagination.totalPages);

    // Append buttons to pagination container
    $paginationContainer.append(firstBtn, prevBtn, currentPageDisplay, nextBtn, lastBtn);
}


function updateResults() {
    const $resultsContainer = $('#results-container'); // Adjust to your actual container ID
    $resultsContainer.empty(); // Clear previous results

    // Determine the pool of ingredients based on selected categories
    let pool = [];
    if (selectedCategories.size === 0) {
        pool = ingredientList; // No filter applied, use all ingredients
    } else {
        selectedCategories.forEach(category => {
            if (categoryHashMap[category]) {
                pool.push(...categoryHashMap[category]); // Add matching ingredients
            }
        });
    }

    // Apply pagination
    const startIdx = (pagination.currentPage - 1) * pagination.itemsPerPage;
    const paginatedResults = pool.slice(startIdx, startIdx + pagination.itemsPerPage);

    // Populate results
    if (paginatedResults.length === 0) {
        $resultsContainer.append('<p class="text-center text-gray-500 italic">No ingredients found.</p>');
    } else {
        const $grid = $('<div>').addClass('grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6');

        paginatedResults.forEach((ingredient, index) => {
            const rotation = (index % 6) - 3; // Random rotation effect
            const card = `
                <div class="bg-violet-100 p-6 rounded-2xl shadow-md border-t-2 border-violet-400 
                    hover:shadow-lg transition duration-300 transform rotate-${rotation} hover:rotate-0 
                    select-none cursor-pointer" onclick="window.location.href='https://localhost:7212/Cooking/Ingredient/Details/${ingredient.id}'">
                    
                    <img src="${ingredient.coverImageUrl}" alt="${ingredient.name}" 
                        class="w-full h-40 object-cover rounded-lg mb-4">

                    <h4 class="font-semibold text-violet-800 text-lg">${ingredient.name}</h4>
                    <p class="text-violet-600 text-sm">${ingredient.description || 'No description available'}</p>
                </div>`;
            $grid.append(card);
        });

        $resultsContainer.append($grid);
    }
}

function performSearch(query) {
    // Ensure query is a valid string and not just spaces
    if (typeof query !== "string" || query.trim().length === 0) {
        clearSuggestions();
        return;
    }

    let trimmedQuery = query.trim();

    // Generate a list with ID, Name, and Image
    let searchList = ingredientList.map(ingredient => ({
        id: ingredient.id,
        name: ingredient.name,
        image: ingredient.coverImageUrl
    }));

    // 🛑 If query is empty at this point, stop execution (extra safeguard)
    if (trimmedQuery === "") {
        clearSuggestions();
        return;
    }

    // Get fuzzy matched candidates (only names are passed to the matcher)
    let results = FuzzyMatcher.match(trimmedQuery, searchList.map(item => item.name), 12);

    // Filter out results with no matches
    let filteredResults = results.filter(result => result.matchScore > 0);

    // Map results back to the original searchList (ensuring ID and Image are included)
    let finalResults = filteredResults
        .map(result => {
            let ingredient = searchList.find(item => item.name.toLowerCase() === result.word.toLowerCase());
            if (!ingredient) return null; // Ignore undefined results

            return {
                id: ingredient.id,
                name: ingredient.name,
                image: ingredient.image,
                highlighted: result.highlighted
            };
        })
        .filter(item => item !== null); // Remove null values

    // Populate the UI with matched results
    populateSuggestions(finalResults);
}


function clearSuggestions() {
    console.log("Clearing suggestions...");
    const $container = $("#suggestions .grid");
    $container.empty(); // Clear previous suggestions

    // Add a placeholder message spanning 4 columns
    const $placeholder = $("<div>")
        .addClass("text-gray-500 text-center col-span-4 p-4")
        .text("Search results will be shown here...");

    $container.append($placeholder);
}


function populateSuggestions(suggestions) {
    console.log("Populating suggestions:", suggestions);
    const $container = $("#suggestions .grid");
    $container.empty(); // Clear previous suggestions

    suggestions.forEach((suggestion, index) => {
        const rotation = (index % 6) - 3; // Generate slight rotation effect

        // Create the suggestion wrapper (fixed size & clickable link)
        const $suggestion = $("<a>")
            .attr("href", `https://localhost:7212/Cooking/Ingredient/Details/${suggestion.id}`) // Dynamic route
            .addClass("flex flex-col items-center justify-center w-24 h-24 bg-white shadow-md rounded-lg p-2 transition duration-200 transform")
            .addClass(`rotate-${rotation} hover:rotate-0`) // Apply rotation
            .click(function (e) {
                console.log(`Navigating to: ${$(this).attr("href")}`);
            });

        // Create the image container (fixed size, ratio preserved)
        if (suggestion.image) {
            const $imgContainer = $("<div>")
                .addClass("w-16 h-16 flex items-center justify-center overflow-hidden rounded-full bg-gray-100");

            const $img = $("<img>")
                .attr("src", suggestion.image)
                .addClass("max-w-full max-h-full object-contain");

            $imgContainer.append($img);
            $suggestion.append($imgContainer);
        }

        // Create the text element with highlighted characters
        const $text = $("<div>").addClass("text-center mt-1 text-sm text-rose-700");

        // ✅ Ensure highlighted exists before using forEach
        if (Array.isArray(suggestion.highlighted)) {
            suggestion.highlighted.forEach(({ char, matched }) => {
                const span = $("<span>")
                    .text(char)
                    .addClass(matched ? "bg-green-300 px-1 rounded" : ""); // Green highlight for matches
                $text.append(span);
            });
        } else {
            // Fallback: Show name normally if highlighted is missing
            $text.text(suggestion.name);
        }

        $suggestion.append($text);
        $container.append($suggestion);
    });
}



window.getSelectedCategories = function () {
    return Array.from(selectedCategories);
};

/** ========================== DOCUMENT READY ========================== */

$(document).ready(function () {
    fetchIngredientCategories();
    fetchIngredientsOverview();
    clearSuggestions();
    setupEventListeners();
});
