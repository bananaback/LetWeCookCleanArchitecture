// global variables
let ingredientCategories = [];
let ingredientOverviews = [];
let filteredIngredients = [];
let paginatedIngredients = [];
let paginationObject = {};
let selectedCategoryId = 'all'; // default category
let selectedIngredientId = null; // default selected ingredient 
let selectedUnit = null; // default selected unit
let selectedIngredients = []; // default selected ingredients

let pageSize = 8; // FIXED

// Data processing function
function filteringIngredients() {
    const searchValue = $('#search-bar').val().toLowerCase();

    // Get the selected category ID from the clicked button
    $('.category-button').each(function () {
        if ($(this).hasClass('selected')) {
            selectedCategoryId = $(this).data('category-id');
            return false; // break the loop
        }
    });

    // Filter ingredients based on search value and selected category
    filteredIngredients = ingredientOverviews.filter(ingredient => {
        const matchesSearch = ingredient.name.toLowerCase().includes(searchValue);
        const matchesCategory = selectedCategoryId === 'all' || ingredient.categoryId === selectedCategoryId;
        return matchesSearch && matchesCategory;
    });

}

function paginatingIngredients() {
    const startIndex = (paginationObject.currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    paginatedIngredients = filteredIngredients.slice(startIndex, endIndex);
}


function updatePaginationObject() {
    const totalPages = Math.ceil(filteredIngredients.length / pageSize);
    paginationObject = {
        currentPage: 1,
        totalPages: totalPages,
        pageSize: pageSize,
        totalItems: filteredIngredients.length,
        firstPage: 1,
        lastPage: totalPages,
        hasNext: totalPages > 1,
        hasPrev: false
    };
}


// API calls
function fetchIngredientCategories() {
    $.ajax({
        url: '/api/ingredient-categories',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Add fixed "All" category
            data.unshift({ id: 'all', name: 'All' });
            ingredientCategories = data;
            populateIngredientCategories(); // Populate categories
        },
        error: function (xhr, status, error) {
            console.error('Error fetching ingredient categories:', error);
        }
    });
}

function fetchIngredientOverviews() {
    $.ajax({
        url: '/api/ingredients/overview',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            ingredientOverviews = data;
            filteringIngredients(); // Initial filtering
            updatePaginationObject(); // Initial pagination object update
            paginatingIngredients(); // Initial pagination
            updatePaginationButtons(); // Initial pagination button update
            populateIngredientList(paginatedIngredients); // Populate ingredient list
        },
        error: function (xhr, status, error) {
            console.error('Error fetching ingredient overviews:', error);
        }
    });
}

// UI interactions
function populateIngredientCategories() {
    container.empty(); // clear old buttons if any

    ingredientCategories.forEach(cat => {
        const color = colorThemes[Math.floor(Math.random() * colorThemes.length)];

        const button = $(`
            <div class="category-button select-none px-5 py-2 rounded-full bg-white border ${color.border} ${color.text} font-semibold cursor-pointer ${color.hover} transition"
                 data-category-id="${cat.id}"
                 data-bg="${color.bg}"
                 data-text="${color.text}"
                 data-hover="${color.hover}">
                ${cat.name}
            </div>
        `);

        container.append(button);
    });

    bindCategoryButtonEvents();

    // Auto-select "All" button
    $('.category-button').first().trigger('click');
}

function populateIngredientList(ingredientsToRender) {
    const colorClasses = [
        'text-teal-600',
        'text-amber-600',
        'text-rose-600',
        'text-lime-600'
    ];

    const container = $('#ingredient-list');
    container.empty();

    if (ingredientsToRender.length === 0) {
        container.append(`
            <div class="w-full text-center text-gray-500 py-10">
                No ingredients found.
            </div>
        `);
        return;
    }

    ingredientsToRender.forEach((ingredient, index) => {
        const colorClass = colorClasses[index % colorClasses.length];

        const ingredientCard = `
            <div class="ingredient-card w-full select-none h-48 bg-white p-4 rounded-2xl shadow-md hover:shadow-lg text-center cursor-pointer transition transform hover:-translate-y-1 flex flex-col justify-between"
                data-id="${ingredient.id}">
                <div class="w-full h-24 bg-gray-100 rounded-xl flex items-center justify-center overflow-hidden">
                    <img src="${ingredient.coverImageUrl}" alt="${ingredient.name}" class="max-h-full max-w-full object-contain">
                </div>
                <div class="mt-2 text-sm font-semibold ${colorClass} truncate" title="${ingredient.name}">
                    ${ingredient.name}
                </div>
            </div>
        `;

        container.append(ingredientCard);

    });
}


function updatePaginationButtons() {
    const container = $('#pagination-button-container');
    container.empty(); // Clear old buttons

    const {
        currentPage,
        totalPages,
        firstPage,
        lastPage,
        hasNext,
        hasPrev
    } = paginationObject;

    // Helper to create a button
    function createButton(label, page, isActive = false) {
        const baseClass = "px-4 py-2 rounded-lg font-medium transition select-none";
        if (isActive) {
            return $(`<div class="${baseClass} bg-teal-500 text-white">${label}</div>`);
        } else {
            return $(`<div class="${baseClass} bg-white border border-teal-300 text-teal-500 cursor-pointer hover:bg-teal-100">${label}</div>`)
                .click(() => {
                    console.log(`Page ${page} clicked`);
                    paginationObject.currentPage = page;
                    paginationObject.hasPrev = page > firstPage;
                    paginationObject.hasNext = page < lastPage;
                    filteringIngredients();
                    console.log('filteredIngredients:', filteredIngredients);
                    console.log(paginationObject);
                    paginatingIngredients();
                    updatePaginationButtons();
                    populateIngredientList(paginatedIngredients);
                });
        }
    }

    // Conditionally add First
    if (currentPage > firstPage + 0) {
        container.append(createButton("First", firstPage));
    }

    // Conditionally add Prev
    if (hasPrev) {
        container.append(createButton("Prev", currentPage - 1));
    }

    // Current Page (always rendered)
    container.append(createButton(currentPage, currentPage, true));

    // Conditionally add Next
    if (hasNext) {
        container.append(createButton("Next", currentPage + 1));
    }

    // Conditionally add Last
    if (currentPage < lastPage) {
        container.append(createButton("Last", lastPage));
    }
}

function updateSelectedIngredientPanel() {
    let panel = $('#selected-ingredient-panel');

    // Find the selected ingredient object from the list
    const ingredient = ingredientOverviews.find(i => i.id === selectedIngredientId);

    if (!ingredient) return;

    // Animate fade out first
    panel.stop(true, true).fadeOut(150, function () {
        // Update content
        $('#selected-ingredient-name').text(ingredient.name);
        $('#selected-ingredient-img')
            .attr('src', ingredient.coverImageUrl)
            .attr('alt', ingredient.name);

        // Optional: Add scale effect briefly
        panel.css({ transform: 'scale(0.97)' });

        // Animate fade in and scale back
        panel.fadeIn(150).css({ transform: 'scale(1)', transition: 'transform 0.2s ease' });
    });
}




function bindCategoryButtonEvents() {
    $('.category-button').off('click').on('click', function () {
        // Remove 'selected' class and reset background and text color
        $('.category-button').each(function () {
            $(this).removeClass('selected');
            $(this).removeClass($(this).data('bg'));   // remove colored background
            $(this).removeClass('text-white');          // remove white text
            $(this).addClass('bg-white');               // back to white bg
            $(this).addClass($(this).data('text'));      // restore original text color
        });

        // Add 'selected' class to clicked one
        $(this).addClass('selected');
        $(this).removeClass('bg-white');
        $(this).addClass($(this).data('bg'));    // set colored background
        $(this).removeClass($(this).data('text'));
        $(this).addClass('text-white');          // set text color white

        // Get the selected category ID from the clicked button
        selectedCategoryId = $(this).data('category-id');

        filteringIngredients(); // Filter ingredients based on search value
        updatePaginationObject(); // Update pagination object
        paginatingIngredients(); // Paginate filtered ingredients
        updatePaginationButtons(); // Update pagination buttons
        populateIngredientList(paginatedIngredients); // Populate ingredient list with paginated ingredients

    });
}

// Data
const colorThemes = [
    { bg: 'bg-teal-500', border: 'border-teal-300', text: 'text-teal-600', hover: 'hover:bg-teal-100' },
    { bg: 'bg-amber-500', border: 'border-amber-300', text: 'text-amber-600', hover: 'hover:bg-amber-100' },
    { bg: 'bg-rose-500', border: 'border-rose-300', text: 'text-rose-600', hover: 'hover:bg-rose-100' },
    { bg: 'bg-lime-500', border: 'border-lime-300', text: 'text-lime-600', hover: 'hover:bg-lime-100' },
    { bg: 'bg-indigo-500', border: 'border-indigo-300', text: 'text-indigo-600', hover: 'hover:bg-indigo-100' },
    { bg: 'bg-pink-500', border: 'border-pink-300', text: 'text-pink-600', hover: 'hover:bg-pink-100' },
    { bg: 'bg-orange-500', border: 'border-orange-300', text: 'text-orange-600', hover: 'hover:bg-orange-100' },
];
const container = $('#ingredient-category-container');

// Function to add the selected ingredient to the global list
function addSelectedIngredient() {
    // Retrieve the data from global variables and input fields
    const ingredientId = selectedIngredientId; // The selected ingredient ID
    const quantity = parseFloat($('#quantity-input').val()); // Quantity from input field
    const unit = selectedUnit; // The selected unit (e.g., Gram, Kilogram)

    // Check if the ingredient, quantity, and unit are valid
    if (!ingredientId || isNaN(quantity) || quantity <= 0 || !unit) {
        // If invalid data, return without adding
        return;  // Exit the function if validation fails
    }

    // Check if the ingredient is already in the selectedIngredients list
    const existingIngredientIndex = selectedIngredients.findIndex(i => i.id === ingredientId);

    if (existingIngredientIndex !== -1) {
        // If the ingredient already exists, update the quantity and unit
        selectedIngredients[existingIngredientIndex].quantity += quantity; // Add to existing quantity
        selectedIngredients[existingIngredientIndex].unit = unit; // Update the unit

        // Optionally, alert the user that the quantity has been updated
        console.log(`Updated ingredient with ID ${ingredientId}: New quantity is ${selectedIngredients[existingIngredientIndex].quantity}`);
    } else {
        // If the ingredient doesn't exist, create a new object and add it to the list
        const selectedIngredient = {
            id: ingredientId,
            quantity: quantity,
            unit: unit
        };

        selectedIngredients.push(selectedIngredient);
        console.log(`Added new ingredient with ID ${ingredientId}: Quantity is ${quantity}`);
    }

    // Optionally, clear the inputs after adding/updating (if needed)
    $('#quantity-input').val(''); // Clear quantity input
    $('.unit-chip').removeClass('bg-teal-500 text-white border-teal-600 ring-4 ring-teal-200');
    selectedUnit = null; // Clear the selected unit (you can reset unit selection here if needed)

    // You can log the selectedIngredients array for debugging purposes
    console.log(selectedIngredients);
}


$(document).ready(function () {
    fetchIngredientCategories();
    fetchIngredientOverviews();

    $('#search-bar').on('input', function () {
        const inputValue = $(this).val();
        filteringIngredients(); // Filter ingredients based on search value
        updatePaginationObject(); // Update pagination object
        paginatingIngredients(); // Paginate filtered ingredients
        updatePaginationButtons(); // Update pagination buttons
        populateIngredientList(paginatedIngredients); // Populate ingredient list with paginated ingredients

    });

    $(document).on('click', '.ingredient-card', function () {
        // Clear previous selections
        $('.ingredient-card').removeClass(
            'border-4 border-emerald-500 scale-105 ring-4 ring-emerald-200 bg-emerald-50'
        );

        // Apply highlight to selected card
        $(this).addClass(
            'border-4 border-emerald-500 scale-105 ring-4 ring-emerald-200 bg-emerald-50'
        );

        $('.unit-chip').removeClass('bg-teal-500 text-white border-teal-600 ring-4 ring-teal-200');
        selectedUnit = null; // Reset selected unit


        // Assign ingredient ID to global variable
        selectedIngredientId = $(this).data('id');
        console.log('Selected ingredient ID:', selectedIngredientId);
        updateSelectedIngredientPanel(); // Update selected ingredient panel
    });

    $(document).on('click', '.unit-chip', function () {
        // Remove previously applied highlight styles from all chips
        $('.unit-chip').removeClass('bg-teal-500 text-white border-teal-600 ring-4 ring-teal-200');

        // Add selected styles to the clicked chip
        $(this).addClass('bg-teal-500 text-white border-teal-600 ring-4 ring-teal-200');

        selectedUnit = $(this).text().trim();
        console.log('Selected unit:', selectedUnit);
    });

    // Event listener for the Add button click
    $('#add-ingredient-button').click(function () {
        // Get the quantity input value
        const quantity = $('#quantity-input').val();

        // Validate if quantity is entered
        if (quantity <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Invalid Quantity',
                text: 'Please enter a valid quantity.',
            });
            return;
        }

        if (selectedIngredientId && selectedUnit) {
            // Show success alert
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: `Added ${quantity} ${selectedUnit} of the selected ingredient.`,
            }).then((result) => {
                // This function runs after the user clicks "OK" on the success alert
                if (result.isConfirmed) {
                    // Clone the ingredient info
                    const clonedIngredientInfo = $('#selected-ingredient-panel').clone();
                    // Insert the cloned ingredient info somewhere in the DOM (e.g., body or another container)
                    $('body').append(clonedIngredientInfo);

                    // Add classes for animation
                    clonedIngredientInfo.css({
                        position: 'absolute',
                        top: $('#selected-ingredient-panel').offset().top,
                        left: $('#selected-ingredient-panel').offset().left,
                        opacity: 1,
                    });

                    clonedIngredientInfo.animate({
                        top: '+=300px',  // Moves down 100px (you can adjust this value)
                        opacity: 0       // Fade out
                    }, 500, function () {
                        // After the animation, remove the cloned element from the DOM
                        $(this).remove();
                    });

                    addSelectedIngredient(); // Add the selected ingredient to the global list
                }
            });
        } else {
            // If validation fails, show error alert
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Please select a valid ingredient and unit.',
            });
        }
    });


});
