let userRequests = [];

let filteredRequests = userRequests.slice(); // Initialize filtered requests with all user requests


let pagination = {
    totalItems: userRequests.length,
    itemsPerPage: $('#itemsPerPage').val() || 5, // Default to 5 if not set
    currentPage: 1,
    minPage: 1,
    maxPage: 1,
};

let statusFilter = "";
let sortingOption = "";
let requestTypeFilter = "";


$(document).ready(function () {
    $.ajax({
        url: '/api/requests',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            userRequests = response;
            console.log("User Requests:", userRequests);
            applyFilter();
            initializePagination();

            updatePaginationDisplay();
        },
        error: function (xhr, status, error) {
            console.error('API call failed:', error);
            initializePagination();

            updatePaginationDisplay();
        }
    });


    addEventListener();


});

function initializePagination() {
    pagination.totalItems = userRequests.length;
    pagination.itemsPerPage = 5; // Set the number of items per page
    pagination.currentPage = 1; // Start at the first page
    pagination.minPage = 1; // Minimum page number
    pagination.maxPage = Math.ceil(pagination.totalItems / pagination.itemsPerPage); // Calculate the maximum page number
}

function addEventListener() {
    // Click event for First button
    $('#first-page-btn').click(function () {
        console.log(`Before First Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
        if (pagination.currentPage > pagination.minPage) {
            pagination.currentPage = pagination.minPage;
            updatePaginationDisplay();
            updateRequestList(filteredRequests.slice((pagination.currentPage - 1) * pagination.itemsPerPage, pagination.currentPage * pagination.itemsPerPage)); // Update the request list with the filtered requests
        }
        console.log(`After First Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
    });

    // Click event for Prev button
    $('#prev-page-btn').click(function () {
        console.log(`Before Prev Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
        if (pagination.currentPage > pagination.minPage) {
            pagination.currentPage--;
            updatePaginationDisplay();
            updateRequestList(filteredRequests.slice((pagination.currentPage - 1) * pagination.itemsPerPage, pagination.currentPage * pagination.itemsPerPage)); // Update the request list with the filtered requests

        }
        console.log(`After Prev Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
    });

    // Click event for Next button
    $('#next-page-btn').click(function () {
        console.log(`Before Next Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
        if (pagination.currentPage < pagination.maxPage) {
            pagination.currentPage++;
            updatePaginationDisplay();
            updateRequestList(filteredRequests.slice((pagination.currentPage - 1) * pagination.itemsPerPage, pagination.currentPage * pagination.itemsPerPage)); // Update the request list with the filtered requests

        }
        console.log(`After Next Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
    });

    // Click event for Last button
    $('#last-page-btn').click(function () {
        console.log(`Before Last Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
        if (pagination.currentPage < pagination.maxPage) {
            pagination.currentPage = pagination.maxPage;
            updatePaginationDisplay();
            updateRequestList(filteredRequests.slice((pagination.currentPage - 1) * pagination.itemsPerPage, pagination.currentPage * pagination.itemsPerPage)); // Update the request list with the filtered requests

        }
        console.log(`After Last Page click - Current Page: ${pagination.currentPage}, Min Page: ${pagination.minPage}, Max Page: ${pagination.maxPage}`);
    });

    $('#itemsPerPage').change(function () {
        // Get the selected value
        const itemsPerPage = parseInt($(this).val());

        // Update the pagination object with the new items per page value
        pagination.itemsPerPage = itemsPerPage;

        // Recalculate the maxPage value based on the new items per page
        pagination.maxPage = Math.ceil(pagination.totalItems / pagination.itemsPerPage);

        // Reset the current page to 1 after changing items per page
        pagination.currentPage = 1;

        // Update the pagination display
        updatePaginationDisplay();

        collectSortingFilterPaginationData();
    });

    $('#status').change(function () {
        // Get the selected value
        const selectedStatus = $(this).val();

        statusFilter = selectedStatus; // Update the status filter
        collectSortingFilterPaginationData();
    });

    $('#sort').change(function () {
        // Get the selected value
        const selectedSort = $(this).val();

        sortingOption = selectedSort; // Update the sorting option
        collectSortingFilterPaginationData();
    });

    $('#requestType').change(function () {
        // Get the selected value
        const selectedRequestType = $(this).val();

        requestTypeFilter = selectedRequestType; // Update the request type filter
        collectSortingFilterPaginationData();
    });

    $('#goButton').on('click', function () {
        const inputVal = $('#goToPage').val().trim();

        if (!/^[1-9]\d*$/.test(inputVal)) {
            alert('Invalid page number!');
            return;
        }

        const pageNumber = parseInt(inputVal, 10);

        if (pageNumber < pagination.minPage || pageNumber > pagination.maxPage) {
            alert(`Page number must be between ${pagination.minPage} and ${pagination.maxPage}`);
            return;
        }

        // Passed all checks
        collectSortingFilterPaginationData();
    });




}

function collectSortingFilterPaginationData() {
    statusFilter = $('#status').val() || ""; // Get the selected status filter value
    sortingOption = $('#sort').val() || ""; // Get the selected sorting option value
    requestTypeFilter = $('#requestType').val() || ""; // Get the selected request type filter value

    // collect pagination data
    pagination.itemsPerPage = $('#itemsPerPage').val() || 5; // Get the selected items per page value
    pagination.currentPage = $('#goToPage').val() || 1; // Get the current page value
    pagination.minPage = 1; // Minimum page number
    pagination.maxPage = Math.ceil(userRequests.length / pagination.itemsPerPage); // Calculate the maximum page number

    applyFilter();
}

function applyFilter() {
    // Filter the user requests based on the selected status and request type
    filteredRequests = userRequests.filter(request => {
        let statusMatch = true;
        let typeMatch = true;

        if (statusFilter) {
            statusMatch = request.status === statusFilter;
        }

        if (requestTypeFilter) {
            typeMatch = request.type === requestTypeFilter;
        }

        return statusMatch && typeMatch;
    });

    // Sort the filtered requests based on the selected sorting option
    if (sortingOption) {
        filteredRequests.sort((a, b) => {
            if (sortingOption === 'DATE_REQUESTED') {
                return new Date(b.createdAt) - new Date(a.createdAt); // Sort by createdAt descending
            } else if (sortingOption === 'DATE_UPDATED') {
                return new Date(b.updatedAt) - new Date(a.updatedAt); // Sort by updatedAt descending
            }
            return 0; // No sorting applied
        });
    }

    console.log(pagination);
    console.log(filteredRequests);

    // Update pagination object and reset to page 1
    pagination.totalItems = filteredRequests.length;
    pagination.currentPage = 1; // Reset to first page after filtering
    pagination.maxPage = Math.ceil(pagination.totalItems / pagination.itemsPerPage); // Recalculate maxPage

    updatePaginationDisplay();
    updateRequestList(filteredRequests.slice((pagination.currentPage - 1) * pagination.itemsPerPage, pagination.currentPage * pagination.itemsPerPage)); // Update the request list with the filtered requests
}

const formatDate = (dateString) => {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-GB', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
    }).format(date);
}

const getBadgeColor = (type) => {
    switch (type) {
        case "CREATE_INGREDIENT": return "bg-emerald-500";
        case "UPDATE_INGREDIENT": return "bg-amber-500";
        case "CREATE_RECIPE": return "bg-blue-500";
        case "UPDATE_RECIPE": return "bg-purple-500";
        default: return "bg-gray-500";
    }
}

function getStatusColor(status) {
    switch (status) {
        case 'PENDING':
            return 'bg-yellow-500';
        case 'APPROVED':
            return 'bg-emerald-600';
        case 'REJECTED':
            return 'bg-red-500';
        default:
            return 'bg-gray-500';
    }
}


function updateRequestList(filteredRequests) {
    const $requestsList = $('#requestsList');
    $requestsList.empty();

    if (filteredRequests.length === 0) {
        $requestsList.append(`
            <div class="text-center text-gray-500 py-8">
                No requests found.
            </div>
        `);
        return;
    }

    // Sequential fetch + render
    (async () => {
        for (const request of filteredRequests) {
            let ingredientData = null;
            let recipeData = null;

            // ✅ Handle INGREDIENT data
            if (request.type === "CREATE_INGREDIENT" || request.type === "UPDATE_INGREDIENT") {
                try {
                    ingredientData = await $.ajax({
                        url: `/api/ingredient/${request.newReferenceId}/overview`,
                        method: 'GET',
                        dataType: 'json'
                    });
                } catch (err) {
                    console.warn('Failed to fetch ingredient data:', err);
                }
            }

            // ✅ Handle RECIPE data
            if (request.type === "CREATE_RECIPE" || request.type === "UPDATE_RECIPE") {
                try {
                    recipeData = await $.ajax({
                        url: `/api/recipe-overview/${request.newReferenceId}`,
                        method: 'GET',
                        dataType: 'json'
                    });
                    console.log(request);
                } catch (err) {
                    console.warn('Failed to fetch recipe data:', err);
                }
            }

            // ✅ Ingredient Section
            const ingredientSection = ingredientData ? `
                <div class="bg-emerald-50 px-6 py-4 grid grid-cols-3 gap-4 items-center">
                    <dt class="text-sm font-medium text-gray-700 col-span-1">Ingredient</dt>
                    <dd class="col-span-2 flex items-center gap-4">
                        <span class="text-sm text-gray-900 font-sans">${ingredientData.name}</span>
                        <div class="w-20 h-20 bg-gray-100 flex items-center justify-center rounded-md shadow overflow-hidden">
                            <img src="${ingredientData.coverImageUrl}" alt="Cover Image" class="object-contain w-full h-full">
                        </div>
                    </dd>
                </div>
            ` : '';

            // ✅ Recipe Section
            const recipeSection = recipeData ? `
                <div class="bg-cyan-50 px-6 py-4 grid grid-cols-3 gap-4 items-center">
                    <dt class="text-sm font-medium text-gray-700 col-span-1">Recipe</dt>
                    <dd class="col-span-2 flex items-center gap-4">
                        <span class="text-sm text-gray-900 font-sans">${recipeData.name}</span>
                        <div class="w-20 h-20 bg-gray-100 flex items-center justify-center rounded-md shadow overflow-hidden">
                            <img src="${recipeData.coverImage}" alt="Cover Image" class="object-contain w-full h-full">
                        </div>
                    </dd>
                </div>
            ` : '';

            // ✅ Response Message Handling (existing)
            let formattedResponseMessage = '';
            if (request.responseMessage) {
                const responseParts = request.responseMessage.split('|');
                const adminResponse = responseParts[1] || '';
                const problemsRaw = responseParts[2] || '';
                const suggestionsRaw = responseParts[3] || '';

                const parseList = (text, icon) =>
                    text
                        .split('-')
                        .map(item => item.trim())
                        .filter(item => item.length > 0)
                        .map(item => `<li class="flex items-start gap-2"><span>${icon}</span><span>${item}</span></li>`)
                        .join('');

                const problemsList = parseList(problemsRaw, '❌');
                const suggestionsList = parseList(suggestionsRaw, '✅');

                formattedResponseMessage = `
                    <div class="flex space-x-4 text-sm text-gray-700 font-sans">
                        <div class="flex-1">
                            <strong class="block text-emerald-700 mb-1">Admin Response:</strong>
                            <p class="text-gray-900 whitespace-pre-line">${adminResponse}</p>
                        </div>
                        <div class="flex-1">
                            <strong class="block text-red-700 mb-1">Problems:</strong>
                            <ul class="space-y-1">${problemsList}</ul>
                        </div>
                        <div class="flex-1">
                            <strong class="block text-green-700 mb-1">Suggestions:</strong>
                            <ul class="space-y-1">${suggestionsList}</ul>
                        </div>
                    </div>
                `;
            }

            // ✅ Card Render
            const card = `
                <div class="pt-4">
                    <div class="bg-white shadow-lg overflow-hidden rounded-xl border border-emerald-100 transform hover:shadow-xl transition duration-300">
                        <div class="px-6 py-4">
                            <h3 class="text-xl font-sans font-medium text-emerald-800 flex items-center gap-3">
                                <span class="inline-block px-3 py-1.5 text-sm tracking-wide font-semibold text-white ${getBadgeColor(request.type)} rounded-full">
                                    ${request.type.replaceAll('_', ' ')}
                                </span>
                            </h3>
                            <p class="mt-1 max-w-2xl">
                                <span class="inline-block px-2 py-0.5 text-xs font-medium text-white ${getStatusColor(request.status)} rounded">
                                    ${request.status}
                                </span>
                            </p>
                        </div>
                        <div class="border-t border-emerald-50">
                            <dl>
                                <div class="bg-emerald-50 px-6 py-4 grid grid-cols-3 gap-4">
                                    <dt class="text-sm font-medium text-gray-700">Date Requested</dt>
                                    <dd class="mt-1 text-sm text-gray-900 col-span-2 font-sans">${formatDate(request.createdAt)}</dd>
                                </div>
                                <div class="bg-white px-6 py-4 grid grid-cols-3 gap-4">
                                    <dt class="text-sm font-medium text-gray-700">Date Updated</dt>
                                    <dd class="mt-1 text-sm text-gray-900 col-span-2 font-sans">${formatDate(request.updatedAt) || '-'}</dd>
                                </div>
                                ${formattedResponseMessage ? `
                                    <div class="bg-emerald-50 px-6 py-4">
                                        <dt class="text-sm font-medium text-gray-700">Response Message</dt>
                                        <dd class="mt-1 text-sm text-gray-900">${formattedResponseMessage}</dd>
                                    </div>` : ""}
                                ${ingredientSection}
                                ${recipeSection}
                            </dl>
                        </div>
                        <div class="px-6 py-3 bg-white text-right">
                            ${ingredientData ? `
                                <button onclick="window.location.href='/Cooking/Ingredient/Preview/${request.newReferenceId}'"
                                    class="bg-cyan-600 hover:bg-cyan-700 text-white font-sans font-medium text-sm py-2 px-4 rounded-lg shadow-md transform hover:scale-105 transition duration-300">
                                    Preview Ingredient
                                </button>` : ''}
                            ${recipeData ? `
                                <button onclick="window.location.href='/Cooking/Recipe/Preview/${request.newReferenceId}'"
                                    class="bg-emerald-600 hover:bg-emerald-700 text-white font-sans font-medium text-sm py-2 px-4 rounded-lg shadow-md transform hover:scale-105 transition duration-300">
                                    Preview Recipe
                                </button>` : ''}
                        </div>
                    </div>
                </div>
            `;

            $requestsList.append(card);
        }
    })();
}



function updatePaginationDisplay() {
    // Logic to update buttons based on pagination state
    if (pagination.currentPage === pagination.minPage) {
        $('#first-page-btn').prop('disabled', true).removeClass('hover:bg-emerald-700 bg-emerald-600 text-white').addClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
        $('#prev-page-btn').prop('disabled', true).removeClass('hover:bg-emerald-700 bg-emerald-600 text-white').addClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
    } else {
        $('#first-page-btn').prop('disabled', false).addClass('hover:bg-emerald-700 bg-emerald-600 text-white').removeClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
        $('#prev-page-btn').prop('disabled', false).addClass('hover:bg-emerald-700 bg-emerald-600 text-white').removeClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
    }

    if (pagination.currentPage === pagination.maxPage) {
        $('#next-page-btn').prop('disabled', true).removeClass('hover:bg-emerald-700 bg-emerald-600 text-white').addClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
        $('#last-page-btn').prop('disabled', true).removeClass('hover:bg-emerald-700 bg-emerald-600 text-white').addClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
    } else {
        $('#next-page-btn').prop('disabled', false).addClass('hover:bg-emerald-700 bg-emerald-600 text-white').removeClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
        $('#last-page-btn').prop('disabled', false).addClass('hover:bg-emerald-700 bg-emerald-600 text-white').removeClass('bg-emerald-200 text-emerald-700 cursor-not-allowed');
    }

    // Always hard emerald for current page
    $('#current-page-btn').removeClass('bg-emerald-200 text-emerald-700 cursor-not-allowed').addClass('bg-emerald-600 hover:bg-emerald-700 text-white').prop('disabled', true);


    // Update current page display if needed
    $('#current-page-btn').text(pagination.currentPage);
}

