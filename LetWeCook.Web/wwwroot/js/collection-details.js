function applyFilters() {
    const query = $('#search-input').val().trim();
    const sortBy = $('#sort').val();
    const itemsPerPage = parseInt($('#items-per-page').val(), 10);
    const pageNumber = currentPage || 1;


    const requestData = {
        searchTerm: query,
        collectionId: collectionId,
        sortBy: sortBy,
        pageNumber: pageNumber,
        pageSize: itemsPerPage,
        isAscending: isAscending
    };

    // log what is being sent
    console.log('Request data:', requestData);

    // check collectionId before making the request
    if (!collectionId) {
        console.error('Collection ID is not set. Cannot fetch collection items.');
        return;
    }

    $.ajax({
        url: `/api/collection/${requestData.collectionId}/items/query`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            console.log('Data received:', response);
            // Set the collection name in the heading
            if (response.items[0].collectionName) {
                $('#collection-title').text(`🍳 ${response.items[0].collectionName}`);
            }
            renderRecipes(response);
            renderPagination(response.pageNumber, Math.ceil(response.totalCount / response.pageSize), function (page) {
                currentPage = page;
                applyFilters(); // Reapply filters with new page
            });
        },
        error: function (xhr, status, error) {
            const err = xhr.responseJSON || {};

            console.error('Backend error:', err);

            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: err.message || 'Something went wrong. Please try again later.'
            });
        }
    });
}

function renderRecipes(data) {
    const container = $('#recipe-cards-container');
    container.empty(); // Clear previous cards

    if (!data.items || data.items.length === 0) {
        container.append('<p class="text-gray-500 col-span-full">No recipes found.</p>');
        return;
    }

    data.items.forEach(item => {
        const formattedDate = new Date(item.createdAt).toLocaleDateString('en-US', {
            year: 'numeric', month: 'short', day: 'numeric'
        });

        const card = `
    <div class="recipe-card bg-orange-50 border border-orange-200 shadow-sm hover:shadow-md transition p-4 flex flex-col"
         data-recipe-id="${item.recipeId}" data-collection-id="${collectionId}">
        <img src="${item.imageUrl}" alt="Recipe Cover"
            class="w-full h-40 object-cover rounded-md mb-3" />
        <h2 class="text-lg font-semibold text-orange-800">🍽️ ${item.recipeName}</h2>
        <p class="text-sm text-orange-700 mt-1">Date Added: <span class="font-medium">${formattedDate}</span></p>

        <div class="mt-auto flex gap-2 pt-4">
            <button class="view-details-btn flex-1 px-3 py-2 text-sm bg-orange-500 text-white rounded hover:bg-orange-600 transition">
                👁 View Details
            </button>
            <button class="remove-btn flex-1 px-3 py-2 text-sm bg-red-100 text-red-600 rounded hover:bg-red-200 transition">
                ❌ Remove
            </button>
        </div>
    </div>
`;


        container.append(card);
    });
}



let isAscending = true;
let currentPage = 1;
let collectionId = '';

$(document).ready(function () {
    collectionId = $('#collection-id').data('collection-id');
    applyFilters(); // Initial load
    console.log('Collection ID:', collectionId);
    $('#search-button').on('click', function (event) {
        event.preventDefault();
        currentPage = 1;
        applyFilters();
    });

    $('#sort').on('change', function () {
        applyFilters();
    });

    $('#items-per-page').on('change', function () {
        applyFilters();
    });

    $('#sort-direction-button').on('click', function () {
        isAscending = !isAscending;
        $('#sort-direction-icon').css('transform', isAscending ? 'rotate(0deg)' : 'rotate(180deg)');
        applyFilters();
    });

    $('#recipe-cards-container').on('click', '.view-details-btn', function () {
        const card = $(this).closest('.recipe-card');
        const recipeId = card.data('recipe-id');

        if (recipeId) {
            window.location.href = `/Cooking/Recipe/Details/${recipeId}`;
        } else {
            console.warn('No recipeId found on card');
        }
    });

    $('#recipe-cards-container').on('click', '.remove-btn', function () {
        const card = $(this).closest('.recipe-card');
        const recipeId = card.data('recipe-id');
        const collectionId = card.data('collection-id');

        if (!recipeId || !collectionId) {
            console.error('Missing IDs for deletion');
            Swal.fire({
                icon: 'error',
                title: 'Missing Data',
                text: 'Unable to identify the recipe or collection.',
            });
            return;
        }

        Swal.fire({
            title: 'Remove Recipe?',
            text: 'Are you sure you want to remove this recipe from the collection?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#aaa',
            confirmButtonText: 'Yes, remove it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/api/collection/${collectionId}/recipes/${recipeId}`,
                    method: 'DELETE',
                    success: function () {
                        Swal.fire({
                            icon: 'success',
                            title: 'Removed!',
                            text: 'Recipe removed successfully.',
                            timer: 1500,
                            showConfirmButton: false
                        });
                        applyFilters(); // Refresh the list
                    },
                    error: function (xhr) {
                        let message = "Failed to remove the recipe from the collection.";

                        if (xhr.responseJSON?.message) {
                            message = xhr.responseJSON.message;
                        }

                        console.error("Remove recipe error:", xhr.responseJSON); // Optional for debugging

                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: message,
                            confirmButtonText: "OK",
                            confirmButtonColor: "#d32f2f"
                        });
                    }
                });

            }
        });
    });


});

function renderPagination(currentPage, totalPages, onPageChange) {
    const container = document.getElementById('pagination');
    container.innerHTML = ''; // clear existing buttons

    function createButton(text, page, isActive = false, disabled = false) {
        const btn = document.createElement('button');
        btn.textContent = text;
        btn.className = `px-3 py-2 rounded-md ${isActive
            ? 'bg-orange-500 text-white font-semibold cursor-default'
            : 'bg-orange-100 text-orange-700 hover:bg-orange-200'
            }`;

        if (disabled) {
            btn.disabled = true;
            btn.classList.add('opacity-50', 'cursor-not-allowed');
        } else if (!isActive) {
            btn.addEventListener('click', () => onPageChange(page));
        }
        return btn;
    }

    // First button
    container.appendChild(createButton('⏮ First', 1, false, currentPage === 1));

    // Prev button
    container.appendChild(createButton('◀ Prev', currentPage - 1, false, currentPage === 1));

    // Show current page button as active
    container.appendChild(createButton(currentPage.toString(), currentPage, true));

    // Next button
    container.appendChild(createButton('Next ▶', currentPage + 1, false, currentPage === totalPages));

    // Last button
    container.appendChild(createButton('Last ⏭', totalPages, false, currentPage === totalPages));
}
