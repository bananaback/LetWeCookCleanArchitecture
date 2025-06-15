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

let isUploading = false;
let isImageSelected = false;

let isUploadingMedia = false;
let isMediaSelected = false;

// Initialize serving count
let servingsCount = 1;
const minServings = 1;
const maxServings = 10;

let pageSize = 8; // FIXED

// Cloudinary Configuration
const cloudinaryConfig = {
    cloud_name: 'dxclyqubm',
    upload_preset: 'letwecook_preset',
    sources: ['local', 'url', 'camera'],
    max_file_size: 10000000,
    client_allowed_formats: ["jpg", "jpeg", "png"]
};

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
            loadEditingRecipe();

        },
        error: function (xhr) {
            console.error("Error fetching ingredient summary:", {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText
            });

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: xhr.responseJSON?.message || 'Failed to load ingredient summary. Please try again later.',
                confirmButtonColor: '#dc3545'
            });
        }

    });
}

function fetchUnitEnums() {
    $.ajax({
        url: '/api/units',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // slice the Unknown unit
            data = data.filter(unit => unit !== 'Unknown');
            // Populate unit chips
            populateUnitChips(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching unit enums:', error);
        }
    });
}

// Fetch and render meal categories
function fetchMealCategoryEnums() {
    $.ajax({
        url: '/api/meal-categories',
        method: 'GET',
        success: function (data) {
            $('#meal-categories').empty(); // Clear previous chips if any

            data.forEach(function (category, index) {
                // Get color class by index or default gray if we run out
                var chipColor = colorPalette[index % colorPalette.length] || 'bg-gray-300 text-gray-700';

                var chipHtml = `
                    <div data-category="${category}" 
                         class="meal-category-chip px-5 py-2 rounded-full ${chipColor} font-semibold cursor-pointer select-none border-2 border-transparent hover:bg-opacity-80">
                        ${category}
                    </div>
                `;
                $('#meal-categories').append(chipHtml);
            });
        },
        error: function (err) {
            console.error('Failed to load meal categories:', err);
        }
    });
}

function fetchRecipeTags() {
    $.ajax({
        url: '/api/recipe-tags',
        method: 'GET',
        success: function (tags) {
            // Clear existing tags
            $('#recipe-tags-container').empty();

            // Loop and render each tag
            tags.forEach(tag => {
                let tagElement = `
                    <button class="tag-item px-3 py-1 bg-teal-100 text-teal-700 rounded-full text-sm hover:bg-teal-200 transition">
                        ${tag.name}
                    </button>
                `;
                $('#recipe-tags-container').append(tagElement);
            });
        },
        error: function (err) {
            console.error('Failed to fetch recipe tags:', err);
            $('#recipe-tags-container').html('<p class="text-red-500">Failed to load tags</p>');
        }
    });
}

// UI interactions
// Function to render emojis + number
function renderServings() {
    // Update number display
    $('#servings-count').text(servingsCount);

    // Render emojis
    let emojis = '';
    for (let i = 0; i < servingsCount; i++) {
        emojis += '👥';
    }
    $('#emoji-container').text(emojis);
}


function renderSelectedIngredients() {
    const container = $('#ingredient-items-container');
    container.empty(); // Clear existing items

    selectedIngredients.forEach(ingredient => {
        const overview = ingredientOverviews.find(i => i.id === ingredient.id);
        if (!overview) return; // Skip if no matching ingredient found

        const item = $(`
            <div class="flex justify-between items-center p-4 rounded-lg bg-yellow-100 gap-4">
                <div class="flex items-center gap-3">
                    <img src="${overview.coverImageUrl}" alt="${overview.name}" class="w-12 h-12 object-contain rounded shadow" />
                    <div class="font-medium text-teal-700">${overview.name} x${ingredient.quantity} (${ingredient.unit})</div>
                </div>
                <div class="text-red-500 font-semibold cursor-pointer hover:text-red-600 remove-ingredient" data-id="${ingredient.id}">
                    Remove
                </div>
            </div>
        `);

        container.append(item);
    });


    // Optional: Add remove click handler
    $('.remove-ingredient').on('click', function () {
        const idToRemove = $(this).data('id');
        selectedIngredients = selectedIngredients.filter(ing => ing.id !== idToRemove);
        console.log('selectedIngredients after removal:', selectedIngredients);
        renderSelectedIngredients();
    });
}


function populateUnitChips(units) {
    const container = $('#unit-selector');
    container.empty(); // Clear existing content if any
    units.forEach(unit => {
        const chip = $(`
            <div class="unit-chip select-none flex items-center justify-center px-4 py-2 rounded-full border border-teal-300 text-teal-600 cursor-pointer hover:bg-teal-100 transition duration-200 ease-in-out">
                ${unit}
            </div>
        `);
        container.append(chip);
    });
}


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

const colorPalette = [
    'bg-teal-500 text-gray-600',
    'bg-yellow-100 text-amber-600',
    'bg-rose-100 text-rose-600',
    'bg-lime-100 text-lime-600',
    'bg-amber-100 text-amber-600',
    'bg-indigo-100 text-indigo-600',
    'bg-purple-100 text-purple-600',
    'bg-pink-100 text-pink-600',
    'bg-green-100 text-green-600',
    'bg-blue-100 text-blue-600',
    'bg-red-100 text-red-600',
    'bg-cyan-100 text-cyan-600',
    'bg-emerald-100 text-emerald-600'
];

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
    renderSelectedIngredients();
}



function openCoverImageUploadWidget() {
    if (isUploading || isImageSelected) return; // Prevent if loading or image already selected
    isUploading = true;

    // Show loading spinner
    $('#cover-upload-box').html(`
        <div class="flex flex-col items-center justify-center gap-2 text-teal-500">
            <svg class="animate-spin h-8 w-8 text-teal-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"></path>
            </svg>
            <span class="font-semibold">Loading uploader...</span>
        </div>
    `);

    cloudinary.openUploadWidget(
        {
            cloudName: cloudinaryConfig.cloud_name,
            uploadPreset: cloudinaryConfig.upload_preset,
            sources: cloudinaryConfig.sources,
            maxFileSize: cloudinaryConfig.max_file_size,
            clientAllowedFormats: cloudinaryConfig.client_allowed_formats
        },
        function (error, result) {
            // ✅ Widget error, ignore
            if (error) {
                console.error('Widget error:', error);
                isUploading = false;
                return;
            }

            // ✅ User uploaded successfully
            if (result && result.event === "success") {
                console.log('Uploaded image URL:', result.info.secure_url);

                // Show uploaded image + remove button
                $('#cover-upload-box').html(`
                    <div class="relative w-full h-full">
                        <img src="${result.info.secure_url}" class="h-full w-full object-cover rounded-2xl" />
                        <button id="remove-cover-btn" class="absolute top-2 right-2 bg-red-500 text-white rounded-full p-1 hover:bg-red-600">
                            ✖️
                        </button>
                    </div>
                `);

                isImageSelected = true;
                isUploading = false;
                window.coverImageUrl = result.info.secure_url;
            }

            // ✅ User just closed (but no upload) ➡️ only act if we are uploading and no image selected yet
            else if (result && result.event === "close" && !isImageSelected) {
                console.log('Widget closed without selecting image');
                // Restore original upload box
                $('#cover-upload-box').html(`
                    <span class="text-teal-400 font-semibold cursor-pointer select-none">
                        Click to upload or drag image here
                    </span>
                `);
                isUploading = false;
            }

            // ❌ Other events like "queues-end" → ignore
        }
    );

}




$(document).ready(function () {
    fetchIngredientCategories();
    fetchIngredientOverviews();
    fetchUnitEnums();
    fetchMealCategoryEnums();
    fetchRecipeTags();
    renderServings();

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

    // Bind click
    $('#cover-upload-box').on('click', function () {
        openCoverImageUploadWidget();
    });

    // ✅ Delegate remove button click (since it’s dynamically added)
    $(document).on('click', '#remove-cover-btn', function (e) {
        e.stopPropagation(); // Prevent triggering upload on box click

        // Reset to original upload box
        $('#cover-upload-box').html(`
        <span class="text-teal-400 font-semibold cursor-pointer select-none">
            Click to upload or drag image here
        </span>
    `);

        isImageSelected = false;
        window.coverImageUrl = null;
    });

    // On click +
    $('#increase-btn').on('click', function () {
        if (servingsCount < maxServings) {
            servingsCount++;
            renderServings();
        }
    });

    // On click -
    $('#decrease-btn').on('click', function () {
        if (servingsCount > minServings) {
            servingsCount--;
            renderServings();
        }
    });

    // Listen for clicks on any difficulty chip
    $('.difficulty-chip').on('click', function () {
        // Remove selected state and styles from all chips
        $('.difficulty-chip').removeClass('selected').css({
            'border': '2px solid transparent', // Remove any border
            'background-color': '', // Remove any background-color styling
            'color': '', // Reset the text color
        });

        // Apply selected state to the clicked chip
        $(this).addClass('selected').css({
            'border': '2px solid #2b6cb0', // Blue border for selected chip
            'background-color': '#e0f2f7', // Light blue background for selected chip
            'color': '#2b6cb0', // Blue text color for selected chip
        });
    });


    // Event delegation for click handling
    $('#meal-categories').on('click', '.meal-category-chip', function () {
        // Remove selected state from all chips
        $('.meal-category-chip').removeClass('border-teal-500 bg-teal-50 text-teal-600 selected') // Remove 'selected' and styles
            .addClass('border-transparent');

        // Apply selected state to the clicked chip
        $(this).removeClass('border-transparent')
            .addClass('border-teal-500 bg-teal-50 text-teal-600 selected');
    });

    // Handle Add Step
    $('#add-step-btn').on('click', function () {
        let newStep = `
            <div class="step-item mb-2 p-4 rounded-2xl bg-yellow-50 border-2 border-teal-200 relative">
                <!-- Remove Step Button -->
                <button class="remove-step-btn absolute top-2 right-2 bg-red-500 rounded-full p-1 hover:bg-red-600">
                    <svg xmlns="http://www.w3.org/2000/svg" height="16px" viewBox="0 -960 960 960" width="16px" fill="#FFFFFF">
                        <path d="m256-200-56-56 224-224-224-224 56-56 224 224 224-224 56 56-224 224 224 224-56 56-224-224-224 224Z"/>
                    </svg>
                </button>

                <label class="block text-teal-500 font-semibold mb-2">Step Title</label>
                <input type="text" placeholder="Enter step title..."
                    class="w-full p-3 rounded-full border-2 border-teal-300 focus:outline-none focus:ring-2 focus:ring-teal-400 mb-4 bg-white" />

                <label class="block text-teal-500 font-semibold mb-2">Description</label>
                <textarea rows="3" placeholder="Describe this step..."
                    class="w-full p-3 rounded-2xl border-2 border-teal-300 focus:outline-none focus:ring-2 focus:ring-teal-400 mb-4 bg-white resize-none"></textarea>

                <label class="block text-teal-500 font-semibold mb-2">Media</label>
                <!-- Media Squares Container -->
                <div class="media-container flex gap-3 flex-wrap">
                    <!-- Initial Square -->
                    <div class="media-square w-20 h-20 rounded-lg border-2 border-teal-300 bg-white flex items-center justify-center cursor-pointer relative hover:bg-teal-50">
                        <span class="text-teal-300 text-2xl">+</span>
                    </div>
                </div>
            </div>`;
        $('#steps-container').append(newStep);
    });

    // Handle Remove Step (delegated)
    $('#steps-container').on('click', '.remove-step-btn', function () {
        $(this).closest('.step-item').remove();
    });

    // ✅ Handle Media Add (delegated)
    $('#steps-container').on('click', '.media-square', function () {
        let isPlusSquare = $(this).find('span').length > 0;

        if (isPlusSquare) {
            let $plusSquare = $(this);
            let $mediaContainer = $plusSquare.closest('.media-container');

            // Show loading spinner
            $plusSquare.html(`
            <svg class="animate-spin h-6 w-6 text-teal-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"></path>
            </svg>
        `).addClass('pointer-events-none');

            // Open Cloudinary widget
            cloudinary.openUploadWidget(
                {
                    cloudName: cloudinaryConfig.cloud_name,
                    uploadPreset: cloudinaryConfig.upload_preset,
                    sources: cloudinaryConfig.sources,
                    maxFileSize: cloudinaryConfig.max_file_size,
                    clientAllowedFormats: cloudinaryConfig.client_allowed_formats
                },
                function (error, result) {
                    if (error) {
                        console.error('Widget error:', error);
                        restorePlusSquare($plusSquare);
                        return;
                    }

                    if (result && result.event === "success") {
                        let imageUrl = result.info.secure_url;
                        console.log('Uploaded image URL:', imageUrl);

                        // Create media square (with hidden input for URL)
                        let newMediaSquare = `
                        <div class="media-square w-20 h-20 rounded-lg border-2 border-teal-300 bg-cover bg-center relative cursor-pointer"
                            style="background-image: url('${imageUrl}');">
                            <input type="hidden" name="stepMedia[]" value="${imageUrl}" />
                            <button class="remove-media-btn absolute -top-2 -right-2 bg-red-500 rounded-full p-1 hover:bg-red-600">
                                <svg xmlns="http://www.w3.org/2000/svg" height="16px" viewBox="0 -960 960-960" width="16px" fill="#FFFFFF">
                                    <path d="m256-200-56-56 224-224-224-224 56-56 224 224 224-224 56 56-224 224 224 224-56 56-224-224-224 224Z"/>
                                </svg>
                            </button>
                        </div>`;

                        // Insert before plus square
                        $plusSquare.before(newMediaSquare);

                        // Restore plus square
                        restorePlusSquare($plusSquare);
                    }
                    else if (result && result.event === "close") {
                        console.log('Widget closed without selecting image');
                        restorePlusSquare($plusSquare);
                    }
                }
            );
        }
    });

    // ✅ Restore plus square
    function restorePlusSquare($square) {
        $square.html('<span class="text-teal-300 text-2xl">+</span>').removeClass('pointer-events-none');
    }

    // Handle Remove Media Square (delegated)
    $('#steps-container').on('click', '.remove-media-btn', function (e) {
        e.stopPropagation(); // Prevent triggering parent click
        $(this).parent('.media-square').remove();
    });

    $('#recipe-tags-container').on('click', '.tag-item', function () {
        let isSelected = $(this).hasClass('selected');

        if (isSelected) {
            // Unselect: remove selected style
            $(this)
                .removeClass('selected')
                .removeClass('bg-teal-500 text-white')
                .addClass('bg-teal-100 text-teal-700');
        } else {
            // Select: add selected style
            $(this)
                .addClass('selected')
                .removeClass('bg-teal-100 text-teal-700')
                .addClass('bg-teal-500 text-white');
        }
    });


    // Listen to submit recipe button clicks
    $('#submit-recipe-btn').on('click', function () {
        let recipe = gatherRecipeInformation(); // Gather all recipe information
        console.log('Recipe data:', recipe); // Log the gathered recipe data

        if (validateRecipe(recipe)) {  // Only submit if validation passes
            submitRecipeToServer(recipe);
        }
    });

    recipeId = $('#recipe-id').text();
    console.log('Recipe ID:', recipeId); // Log the recipe ID

});

let recipeId = null; // This will be set when editing a recipe

function loadEditingRecipe() {
    $.ajax({
        url: '/api/recipe-preview/' + recipeId,
        method: 'GET',
        success: function (response) {
            console.log('Recipe preview data:', response);
            // Populate the form fields with the recipe data
            $('#recipe-name').val(response.name);
            $('#recipe-desc').val(response.description);
            $('#prepare-time').val(response.prepareTime);
            $('#cook-time').val(response.cookTime);

            // // Show uploaded image + remove button
            $('#cover-upload-box').html(`
                    <div class="relative w-full h-full">
                        <img src="${response.coverImage}" class="h-full w-full object-cover rounded-2xl" />
                        <button id="remove-cover-btn" class="absolute top-2 right-2 bg-red-500 text-white rounded-full p-1 hover:bg-red-600">
                            ✖️
                        </button>
                    </div>
                `);

            servingsCount = response.servings;
            renderServings();

            isUploading = false;
            isImageSelected = true;

            // First clear all selected styles
            $('.difficulty-chip').removeClass('selected').css({
                'border': '2px solid transparent',
                'background-color': '',
                'color': '',
            });

            $('.difficulty-chip').each(function () {
                if ($(this).text().trim().toLowerCase() === response.difficulty.trim().toLowerCase()) {
                    $(this).addClass('selected').css({
                        'border': '2px solid #2b6cb0',
                        'background-color': '#e0f2f7',
                        'color': '#2b6cb0',
                    });
                }
            });

            // First clear selected state from all chips
            $('.meal-category-chip').removeClass('border-teal-500 bg-teal-50 text-teal-600 selected')
                .addClass('border-transparent');

            // Then find and select the one matching response.mealCategory
            $('.meal-category-chip').each(function () {
                if ($(this).text().trim().toLowerCase() === response.mealCategory.trim().toLowerCase()) {
                    $(this).removeClass('border-transparent')
                        .addClass('border-teal-500 bg-teal-50 text-teal-600 selected');
                }
            });

            // First, unselect all tags
            $('.tag-item')
                .removeClass('selected bg-teal-500 text-white')
                .addClass('bg-teal-100 text-teal-700');

            // Then, select the ones from response.tags
            response.tags.forEach(function (tag) {
                $('.tag-item').each(function () {
                    if ($(this).text().trim().toLowerCase() === tag.trim().toLowerCase()) {
                        $(this)
                            .addClass('selected bg-teal-500 text-white')
                            .removeClass('bg-teal-100 text-teal-700');
                    }
                });
            });

            response.ingredients.forEach(function (ingredient) {
                // Add each ingredient to the selectedIngredients array
                let ing = {
                    id: ingredient.ingredientId,
                    quantity: ingredient.quantity,
                    unit: ingredient.unit
                };
                selectedIngredients.push(ing);
            });
            renderSelectedIngredients();

            // clear steps container
            $('#steps-container').empty();
            // sort steps by order
            response.steps.sort((a, b) => a.order - b.order);
            // Populate steps
            response.steps.forEach(function (step) {

                let newStep = `
                <div class="step-item mb-2 p-4 rounded-2xl bg-yellow-50 border-2 border-teal-200 relative">
                    <!-- Remove Step Button -->
                    <button class="remove-step-btn absolute top-2 right-2 bg-red-500 rounded-full p-1 hover:bg-red-600">
                        <svg xmlns="http://www.w3.org/2000/svg" height="16px" viewBox="0 -960 960 960" width="16px" fill="#FFFFFF">
                            <path d="m256-200-56-56 224-224-224-224 56-56 224 224 224-224 56 56-224 224 224 224-56 56-224-224-224 224Z"/>
                        </svg>
                    </button>

                    <label class="block text-teal-500 font-semibold mb-2">Step Title</label>
                    <input value="${step.title}" type="text" placeholder="Enter step title..."
                        class="w-full p-3 rounded-full border-2 border-teal-300 focus:outline-none focus:ring-2 focus:ring-teal-400 mb-4 bg-white" />

                    <label class="block text-teal-500 font-semibold mb-2">Description</label>
                    <textarea rows="3" placeholder="Describe this step..."
                        class="w-full p-3 rounded-2xl border-2 border-teal-300 focus:outline-none focus:ring-2 focus:ring-teal-400 mb-4 bg-white resize-none">${step.description}</textarea>

                    <label class="block text-teal-500 font-semibold mb-2">Media</label>
                    <!-- Media Squares Container -->
                    <div class="media-container flex gap-3 flex-wrap">
                        ${step.mediaUrls.map(function (mediaUrl) {
                    return `
                            <div class="media-square w-20 h-20 rounded-lg border-2 border-teal-300 bg-cover bg-center relative cursor-pointer hover:bg-teal-50"
                                style="background-image: url('${mediaUrl}');">
                                <input type="hidden" name="stepMedia[]" value="${mediaUrl}" />
                                <button class="remove-media-btn absolute -top-2 -right-2 bg-red-500 rounded-full p-1 hover:bg-red-600">
                                    <svg xmlns="http://www.w3.org/2000/svg" height="16px" viewBox="0 -960 960-960" width="16px" fill="#FFFFFF">
                                        <path d="m256-200-56-56 224-224-224-224 56-56 224 224 224-224 56 56-224 224 224 224-56 56-224-224-224 224Z"/>
                                    </svg>
                                </button>
                            </div>`;
                }).join('')}
                        <!-- Initial Square -->
                        <div class="media-square w-20 h-20 rounded-lg border-2 border-teal-300 bg-white flex items-center justify-center cursor-pointer relative hover:bg-teal-50">
                            <span class="text-teal-300 text-2xl">+</span>
                        </div>
                    </div>
                </div>`;
                $('#steps-container').append(newStep);
            });
        }
    });
}

function submitRecipeToServer(recipe) {
    console.log('Submitting recipe to server:', recipe);
    $.ajax({
        url: `/api/recipes/${recipeId}`, // Replace with actual recipe ID
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(recipe), // Your CreateRecipeRequest payload
        success: function (response) {
            Swal.fire({
                icon: 'info',
                title: 'Update Request Submitted',
                html: `Your request to update the recipe "<strong>${recipe.name}</strong>" has been submitted and is awaiting admin review.<br><br>
                           <a href="/UserPanel/Profile/Requests" style="color: #007BFF; text-decoration: underline;">View Your Request</a>`,
                customClass: {
                    confirmButton: 'swal-custom-btn'
                },
                didOpen: () => {
                    $('.swal-custom-btn').css({
                        'background-color': '#007BFF', // Blue
                        'color': '#FFFFFF'
                    });
                }
            });
        },
        error: function (xhr) {
            console.log("Error Response:", xhr.responseText);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: xhr.responseJSON?.message || 'Something went wrong while submitting your update request.',
                customClass: {
                    confirmButton: 'swal-custom-btn'
                },
                didOpen: () => {
                    $('.swal-custom-btn').css({
                        'background-color': '#dc3545', // Red
                        'color': '#FFFFFF'
                    });
                }
            });
        }
    });
}



function gatherRecipeInformation() {
    // Gather all the recipe data
    let recipe = {};

    // Recipe Name and Description
    recipe.name = $('#recipe-name').val().trim();
    recipe.description = $('#recipe-desc').val().trim();

    // Servings based on Emoji
    let emojiText = $('#emoji-container').text().trim();
    recipe.servings = Math.floor(emojiText.length / 2);  // Divide by 2 to get correct number of servings

    // Time details
    recipe.prepareTime = $('#prepare-time').val().trim();
    recipe.cookTime = $('#cook-time').val().trim();

    // Difficulty Level
    recipe.difficulty = $('#difficulty-levels .difficulty-chip.selected').text().trim();

    // Meal Category
    recipe.mealCategory = $('#meal-categories .meal-category-chip.selected').text().trim();

    // Selected Tags
    recipe.tags = $('#recipe-tags-container .tag-item.selected').map(function () {
        return $(this).text().trim();  // Clean up each tag's text
    }).get();

    // Selected Ingredients (from global list)
    recipe.ingredients = selectedIngredients || [];

    // Steps (Title, Description, Media)
    recipe.steps = [];
    $('#steps-container .step-item').each(function (index) { // index will auto count 0,1,2...
        let step = {};

        step.order = index + 1; // 1-based order
        step.title = $(this).find('input[type="text"]').val().trim();
        step.description = $(this).find('textarea').val().trim();

        // Media URLs
        step.mediaUrls = [];
        $(this).find('.media-square input[type="hidden"]').each(function () {
            step.mediaUrls.push($(this).val());
        });

        // Add step to the recipe's steps array
        recipe.steps.push(step);
    });


    // Retrieve the cover image URL if it exists or leave empty string
    let coverImageUrl = $('#cover-upload-box img').attr('src') || "";
    recipe.coverImage = coverImageUrl.trim(); // Ensure it's always present (even if empty)

    // Return the complete recipe object
    return recipe;
}


function validateRecipe(recipe) {
    // Validate recipe name
    if (!recipe.name.trim()) {
        Swal.fire({ icon: 'error', title: 'Missing Recipe Name', text: 'Please enter a name for the recipe.' });
        return false;
    }

    // Validate recipe description
    if (!recipe.description.trim()) {
        Swal.fire({ icon: 'error', title: 'Missing Recipe Description', text: 'Please enter a description for the recipe.' });
        return false;
    }

    // Validate servings
    if (isNaN(recipe.servings) || recipe.servings <= 0) {
        Swal.fire({ icon: 'error', title: 'Invalid Servings', text: 'Please enter a valid number for servings.' });
        return false;
    }

    // Validate prepare time
    if (isNaN(recipe.prepareTime) || recipe.prepareTime <= 0) {
        Swal.fire({ icon: 'error', title: 'Invalid Prepare Time', text: 'Please enter a valid prepare time.' });
        return false;
    }

    // Validate cook time
    if (isNaN(recipe.cookTime) || recipe.cookTime <= 0) {
        Swal.fire({ icon: 'error', title: 'Invalid Cook Time', text: 'Please enter a valid cook time.' });
        return false;
    }

    // Validate difficulty
    if (!recipe.difficulty.trim()) {
        Swal.fire({ icon: 'error', title: 'Missing Difficulty Level', text: 'Please select a difficulty level.' });
        return false;
    }

    // Validate meal category
    if (!recipe.mealCategory.trim()) {
        Swal.fire({ icon: 'error', title: 'Missing Meal Category', text: 'Please select a meal category.' });
        return false;
    }

    // Validate tags
    if (!Array.isArray(recipe.tags) || recipe.tags.length === 0) {
        Swal.fire({ icon: 'error', title: 'No Tags Selected', text: 'Please select at least one tag.' });
        return false;
    }

    // ✅ Validate ingredients
    if (!Array.isArray(recipe.ingredients) || recipe.ingredients.length === 0) {
        Swal.fire({ icon: 'error', title: 'No Ingredients Added', text: 'Please add at least one ingredient.' });
        return false;
    }

    for (let i = 0; i < recipe.ingredients.length; i++) {
        let ing = recipe.ingredients[i];
        if (!ing.id || !ing.id.trim()) {
            Swal.fire({ icon: 'error', title: 'Invalid Ingredient', text: `Ingredient ${i + 1} is missing its ID.` });
            return false;
        }
        if (isNaN(ing.quantity) || ing.quantity <= 0) {
            Swal.fire({ icon: 'error', title: 'Invalid Quantity', text: `Ingredient ${i + 1} must have a valid quantity.` });
            return false;
        }
        if (!ing.unit || !ing.unit.trim()) {
            Swal.fire({ icon: 'error', title: 'Missing Unit', text: `Ingredient ${i + 1} is missing its unit.` });
            return false;
        }
    }

    // ✅ Validate steps
    if (!Array.isArray(recipe.steps) || recipe.steps.length === 0) {
        Swal.fire({ icon: 'error', title: 'Missing Recipe Steps', text: 'Please add at least one step.' });
        return false;
    }

    for (let i = 0; i < recipe.steps.length; i++) {
        let step = recipe.steps[i];
        if (!step.title || !step.title.trim()) {
            Swal.fire({ icon: 'error', title: 'Missing Step Title', text: `Step ${i + 1} is missing its title.` });
            return false;
        }
        // Optional: Uncomment below if you want description required too
        // if (!step.description || !step.description.trim()) {
        //     Swal.fire({ icon: 'error', title: 'Missing Step Description', text: `Step ${i + 1} is missing its description.` });
        //     return false;
        // }
    }

    // ✅ Validate cover image
    if (!recipe.coverImage || !recipe.coverImage.trim()) {
        Swal.fire({ icon: 'error', title: 'Missing Cover Image', text: 'Please upload a cover image.' });
        return false;
    }

    // ✅ Passed all checks
    return true;
}
