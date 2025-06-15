$(document).ready(function () {
    let isAscending = true;

    $('#sortToggle').click(function () {
        isAscending = !isAscending;
        $('#sortArrow').toggleClass('rotate-180', !isAscending);
        // Now you can call your fetch with updated isAscending
        fetchCollections(1, isAscending);
    });

    // Refetch when sort or page size changes
    $('#sortSelect, #pageSizeSelect').on('change', function () {
        fetchCollections(1, isAscending);
    });

    $('#searchBtn').on('click', function () {
        fetchCollections(1, isAscending);
    });

    $('#closeEditModal, #cancelEditBtn').on('click', closeEditModal);

    // Attach event listeners after rendering
    $(document).on('click', '.rename-btn', function () {
        const collectionId = $(this).data('id');
        const currentName = $(this).closest('.group').find('h2').text().trim();
        openEditModal(collectionId, currentName);
    });

    $('#editCollectionForm').on('submit', function (e) {
        e.preventDefault();

        const collectionId = $('#editCollectionId').val();
        const newName = $('#editCollectionName').val().trim();

        if (!newName) {
            Swal.fire({
                icon: 'warning',
                title: 'Invalid Name',
                text: 'Please enter a valid name.',
            });
            return;
        }


        $.ajax({
            url: `/api/collection/${collectionId}`, // no /rename
            method: 'PUT',                           // use PUT as your API expects
            contentType: 'application/json',
            data: JSON.stringify(newName),          // just a raw string, not { name: newName }
            success: function () {
                closeEditModal();
                fetchCollections(); // Re-render cards
                Swal.fire({
                    icon: 'success',
                    title: 'Renamed!',
                    text: 'Collection name updated successfully.',
                    timer: 1500,
                    showConfirmButton: false
                });
            },

            error: function (xhr) {
                let message = "Failed to rename the collection.";

                // Try to extract backend error message if available
                if (xhr.responseJSON?.message) {
                    message = xhr.responseJSON.message;
                }

                console.error("Rename collection error:", xhr.responseJSON); // Optional for dev/debugging

                Swal.fire({
                    icon: 'error',
                    title: 'Rename Failed',
                    text: message,
                    confirmButtonText: "OK",
                    confirmButtonColor: "#d32f2f",
                });
            }

        });
    });

    $(document).on('click', '.delete-btn', function () {
        const collectionId = $(this).data('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "This will permanently delete the collection!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/api/collection/${collectionId}`,
                    method: 'DELETE',
                    success: function () {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            text: 'Collection has been deleted.',
                            timer: 1500,
                            showConfirmButton: false
                        });
                        fetchCollections(); // re-fetch and update UI
                    },
                    error: function (xhr) {
                        let message = "An error occurred while deleting the collection.";

                        if (xhr.responseJSON?.message) {
                            message = xhr.responseJSON.message;
                        }

                        console.error("Delete collection error:", xhr.responseJSON); // Optional for debugging

                        Swal.fire({
                            icon: 'error',
                            title: 'Delete Failed',
                            text: message,
                            confirmButtonText: "OK",
                            confirmButtonColor: "#d32f2f"
                        });
                    }

                });
            }
        });
    });

    $(document).on('click', '.view-details-btn', function () {
        const collectionId = $(this).data('id');
        // For example, redirect to details page:
        window.location.href = `/UserPanel/Collection/CollectionDetails?collectionId=${collectionId}`;

    });



    fetchCollections();
});

function renderCollections(collections) {
    const $container = $('#collectionContainer');
    $container.empty();

    collections.forEach(collection => {
        const items = collection.items || [];
        const previewImages = items.slice(0, 5); // Up to 5 visible images
        const hasMore = items.length > 5;

        const imageElements = previewImages.map(item => `
            <img src="${item.imageUrl}" alt="${item.recipeName}" 
                 class="w-full aspect-square object-cover rounded-lg" />
        `);

        // Add "+ more" image if there are more than 5 items
        if (hasMore) {
            const moreImage = items[5];
            imageElements.push(`
                <div class="relative rounded-lg overflow-hidden">
                    <img src="${moreImage.imageUrl}" alt="${moreImage.recipeName}" 
                         class="w-full aspect-square object-cover blur-sm" />
                    <div class="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center">
                        <span class="text-white font-semibold text-lg">+${items.length - 5} more</span>
                    </div>
                </div>
            `);
        }

        const cardHtml = `
    <div class="bg-white shadow-lg border border-gray-100 rounded-2xl overflow-hidden group hover:shadow-xl transition">

        <!-- Top Bar -->
        <div class="flex items-center justify-between px-5 pt-5 pb-2">
            <h2 class="text-xl font-semibold text-gray-800 truncate">${collection.name}</h2>
            <div class="flex gap-2 opacity-0 group-hover:opacity-100 transition">
                <!-- View Details -->
                <button class="p-1.5 hover:bg-blue-100 rounded-full view-details-btn" data-id="${collection.collectionId}" title="View Details">
                    <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path stroke-linecap="round" stroke-linejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.477 0 8.268 2.943 9.542 7-1.274 4.057-5.065 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                </button>

                <!-- Rename -->
                <button class="p-1.5 hover:bg-orange-100 rounded-full rename-btn" data-id="${collection.collectionId}" title="Rename">
                    <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" stroke-width="2"
                        viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round"
                            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" />
                    </svg>
                </button>

                <!-- Delete -->
                <button class="p-1.5 hover:bg-red-100 rounded-full delete-btn" data-id="${collection.collectionId}" title="Delete">
                    <svg class="w-5 h-5 text-red-500" fill="none" stroke="currentColor" stroke-width="2"
                        viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
            </div>
        </div>

        <!-- Image Grid -->
        <div class="grid grid-cols-3 gap-1 p-5">
            ${imageElements.join('')}
        </div>
    </div>
`;


        $container.append(cardHtml);
    });
}


function fetchCollections(page = 1, isAscending = true) {
    const searchTerm = $('#searchInput').val().trim().replace(/\s+/g, ' ');
    const sortBy = $('#sortSelect').val();
    const pageSize = parseInt($('#pageSizeSelect').val());

    $.ajax({
        url: '/api/collections/query',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            searchTerm: searchTerm,
            sortBy: sortBy,
            pageNumber: page,
            pageSize: pageSize,
            isAscending: isAscending
        }),
        success: function (response) {
            console.log('Fetched collections:', response);
            const totalPages = Math.max(1, Math.ceil(response.totalCount / response.pageSize));
            renderPagination(response.pageNumber, totalPages);
            renderCollections(response.items);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to fetch collections: ' + xhr.responseText,
            });
        }

    });
}

function renderPagination(currentPage, totalPages) {
    const $container = $('#pagination');
    $container.empty();

    const createButton = (label, isCurrent, disabled, onClick) => {
        const $btn = $('<button>')
            .addClass('px-3 py-1.5 text-sm border rounded-xl transition')
            .toggleClass('bg-white text-black border-gray-300 hover:bg-orange-100', !isCurrent && !disabled)
            .toggleClass('bg-orange-500 text-white cursor-default', isCurrent)
            .toggleClass('opacity-50 cursor-not-allowed', disabled)
            .html(label)
            .prop('disabled', disabled);

        if (!disabled && typeof onClick === 'function') {
            $btn.on('click', onClick);
        }

        return $btn;
    };

    // First
    const isFirstDisabled = currentPage <= 1;
    $container.append(createButton('&laquo; First', false, isFirstDisabled, () => fetchCollections(1)));

    // Prev
    $container.append(createButton('&lsaquo; Prev', false, isFirstDisabled, () => fetchCollections(currentPage - 1)));

    // Current Page
    $container.append(createButton(currentPage, true, true)); // Always disabled and styled

    // Next
    const isLastDisabled = currentPage >= totalPages;
    $container.append(createButton('Next &rsaquo;', false, isLastDisabled, () => fetchCollections(currentPage + 1)));

    // Last
    $container.append(createButton('Last &raquo;', false, isLastDisabled, () => fetchCollections(totalPages)));
}

function openEditModal(collectionId, currentName) {
    $('#editCollectionId').val(collectionId);
    $('#editCollectionName').val(currentName);

    const $modal = $('#editModal');
    $modal.removeClass('hidden').addClass('flex items-center justify-center');
}


function closeEditModal() {
    $('#editModal').addClass('hidden').removeClass('flex items-center justify-center');
}


$('#closeEditModal, #cancelEditBtn').on('click', closeEditModal);
