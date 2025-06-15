function applyColorState($el) {
    const isSelected = $el.attr('data-selected') === 'true';
    const bg = isSelected ? $el.data('selected-bg') : $el.data('unselected-bg');
    const text = isSelected ? $el.data('selected-text') : $el.data('unselected-text');

    $el.css({
        'background-color': bg,
        'color': text
    });
}

let ingredientCategories = [];
let userIngredients = [];
let selectedCategories = [];
let searchText = '';

let pagination = {
    currentPage: 1,
    itemsPerPage: 12,
    totalItems: 0,
    get totalPages() {
        return Math.ceil(this.totalItems / this.itemsPerPage);
    },
    get hasPrev() {
        return this.currentPage > 1;
    },
    get hasNext() {
        return this.currentPage < this.totalPages;
    },
    goToPage(page) {
        this.currentPage = Math.max(1, Math.min(page, this.totalPages));
    },
    nextPage() {
        if (this.hasNext) this.currentPage++;
    },
    prevPage() {
        if (this.hasPrev) this.currentPage--;
    },
    reset() {
        this.currentPage = 1;
    }
};

function createCategoryElement(name) {
    const $el = $('<div></div>', {
        class: 'select-none category-option border-2 rounded-lg py-2 px-4 cursor-pointer transition-colors',
        text: name,
        'data-category': name,
        'data-selected': 'false',
        'data-selected-bg': '#22c55e',    // Unified selected background color
        'data-selected-text': '#ffffff',  // Unified selected text color
        'data-unselected-bg': '#ffffff',  // Unified unselected background
        'data-unselected-text': '#22c55e' // Unified unselected text
    });

    // Add toggle click handler
    $el.on('click', function () {
        const isSelected = $el.attr('data-selected') === 'true';
        $el.attr('data-selected', !isSelected);
        applyColorState($el);
        selectedCategories = getSelectedCategories();
        console.log("Collected user requirements:", collectUserRequirements());
    });

    applyColorState($el); // Initial color state
    return $el;
}

function fetchIngredientCategories() {
    $.ajax({
        url: '/api/ingredient-categories',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            ingredientCategories = data; // Store the fetched categories

            const $container = $('.option-container');
            $container.empty(); // Clear previous items

            data.forEach(category => {
                const $categoryEl = createCategoryElement(category.name); // Use actual field if it's different
                $container.append($categoryEl);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching categories:", status, error);
        }
    });
}

function fetchUserIngredients() {
    $.ajax({
        url: '/api/user/ingredients',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            userIngredients = data; // Store the fetched ingredients
            collectUserRequirements();
        },
        error: function (xhr, status, error) {
            console.error("Error fetching user ingredients:", {
                status: xhr.status,
                statusText: xhr.statusText,
                response: xhr.responseText,
                error: error
            });

            Swal.fire({
                icon: 'error',
                title: 'Failed to Load Ingredients',
                text: xhr.responseJSON?.message || 'Unable to fetch your saved ingredients. Please try again later.',
                confirmButtonColor: '#dc3545'
            });
        }

    });
}

function getSelectedCategories() {
    const selected = [];
    $('.category-option').each(function () {
        const $el = $(this);
        if ($el.attr('data-selected') === 'true') {
            selected.push($el.data('category'));
        }
    });
    return selected;
}

function collectUserRequirements() {
    const requirements = {
        categories: getSelectedCategories(),
        searchText: searchText,
        itemsPerPage: pagination.itemsPerPage
    };
    filterIngredients();
    return requirements;
}

function filterIngredients() {
    const filtered = userIngredients.filter(ingredient => {
        const nameMatch = searchText === '' || ingredient.name.toLowerCase().includes(searchText.toLowerCase());
        const categoryMatch = selectedCategories.length === 0 || selectedCategories.includes(ingredient.categoryName);
        return nameMatch && categoryMatch;
    });

    // Update pagination info
    pagination.totalItems = filtered.length;

    // Ensure current page is within bounds
    if (pagination.currentPage > pagination.totalPages) {
        pagination.currentPage = pagination.totalPages || 1;
    }

    // Slice the filtered list according to pagination
    const startIndex = (pagination.currentPage - 1) * pagination.itemsPerPage;
    const paginatedItems = filtered.slice(startIndex, startIndex + pagination.itemsPerPage);

    console.log(pagination);
    console.log("Filtered ingredients:", paginatedItems);

    updatePaginationControls();
    // Populate UI
    populateIngredientCards(paginatedItems);
}



function populateIngredientCards(paginatedIngredients) {
    const $container = $('#ingredientList'); // Replace with your actual container ID
    $container.empty(); // Clear previous cards

    if (paginatedIngredients.length === 0) {
        $container.append('<div class="text-gray-500">No ingredients found.</div>');
        return;
    }

    paginatedIngredients.forEach(ingredient => {
        const $card = $(`
            <div class="ingredient-card border rounded-lg p-4 shadow w-full flex flex-col gap-2">
                <!-- Name -->
                <h3 class="text-lg font-semibold mb-1 overflow-hidden text-ellipsis whitespace-nowrap">${ingredient.name}</h3>
                
                <!-- Description -->
                <p class="text-sm text-gray-500 mb-2 overflow-hidden text-ellipsis whitespace-nowrap flex-1">
                    ${ingredient.description}
                </p>
                
                <!-- Image -->
                <div class="w-full h-40 bg-gray-100 rounded-lg border flex items-center justify-center overflow-hidden">
                    <img src="${ingredient.coverImageUrl}" alt="${ingredient.name}"
                        class="max-h-full max-w-full object-contain" />
                </div>
        
                <!-- Hidden field for ingredient ID -->
                <input type="hidden" class="ingredient-id" value="${ingredient.id}" />
        
                <!-- Buttons: Edit and Delete -->
                <div class="mt-4 flex justify-between items-center">
                    <!-- Edit Button -->
                    <button class="edit-btn text-blue-600 hover:bg-blue-100 px-4 py-2 rounded-lg border focus:outline-none focus:ring-2 focus:ring-blue-400">
                        Edit
                    </button>
        
                  
                </div>
            </div>
        `);

        // Add event listener for Edit button
        $card.find('.edit-btn').on('click', function () {
            const ingredientId = $(this).closest('.ingredient-card').find('.ingredient-id').val();
            const editUrl = `/Cooking/Ingredient/Update/${ingredientId}`;
            window.location.href = editUrl; // Redirect to the Edit page
        });



        // Append the card to the container
        $container.append($card);
    });

}

function updatePaginationControls() {
    $('.current-page').text(`Page ${pagination.currentPage} of ${pagination.totalPages}`);

    // Enable/disable buttons based on pagination state
    $('.first-page').prop('disabled', !pagination.hasPrev);
    $('.prev-page').prop('disabled', !pagination.hasPrev);
    $('.next-page').prop('disabled', !pagination.hasNext);
    $('.last-page').prop('disabled', !pagination.hasNext);

    // Optionally, add visual feedback for disabled buttons
    ['.first-page', '.prev-page', '.next-page', '.last-page'].forEach(selector => {
        const $btn = $(selector);
        if ($btn.prop('disabled')) {
            $btn.addClass('opacity-50 cursor-not-allowed');
        } else {
            $btn.removeClass('opacity-50 cursor-not-allowed');
        }
    });
}


function setupPaginationListeners() {
    // First page button listener
    $('.first-page').on('click', function () {
        if (!pagination.hasPrev) {
            alert('You are already on the first page!');
            return;
        }
        pagination.goToPage(1);
        filterIngredients();
        updatePaginationControls();  // Optional: update page UI after change
    });

    // Previous page button listener
    $('.prev-page').on('click', function () {
        if (!pagination.hasPrev) {
            alert('No previous page available!');
            return;
        }
        pagination.prevPage();
        filterIngredients();
        updatePaginationControls();  // Optional: update page UI after change
    });

    // Next page button listener
    $('.next-page').on('click', function () {
        if (!pagination.hasNext) {
            alert('No next page available!');
            return;
        }
        pagination.nextPage();
        filterIngredients();
        updatePaginationControls();  // Optional: update page UI after change
    });

    // Last page button listener
    $('.last-page').on('click', function () {
        if (!pagination.hasNext) {
            alert('You are already on the last page!');
            return;
        }
        pagination.goToPage(pagination.totalPages);
        filterIngredients();
        updatePaginationControls();  // Optional: update page UI after change
    });
}



$(document).ready(function () {
    setupPaginationListeners();
    fetchIngredientCategories();
    fetchUserIngredients();
    $('#searchName').on('input', function () {
        const value = $(this).val().trim();

        searchText = value;
        console.log("Collected user requirements:", collectUserRequirements());
    });

    $('#itemsPerPage').on('change', function () {
        const selectedValue = $(this).val();
        pagination.itemsPerPage = parseInt(selectedValue, 10) || 12; // Default to 12 if invalid
        console.log("Collected user requirements:", collectUserRequirements());
    });


});
