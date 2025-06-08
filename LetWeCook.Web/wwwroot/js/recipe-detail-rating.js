let queryOptions = {
    sortBy: 'Newest', // Default value
    page: 1,
    pageSize: 5
};

$(document).ready(function () {
    let selectedRating = 0;

    $('#rating-star-container div').each(function (index) {
        $(this).on('mouseenter', function () {
            highlightStars(index + 1);
        }).on('mouseleave', function () {
            highlightStars(selectedRating);
        }).on('click', function () {
            selectedRating = index + 1;
            highlightStars(selectedRating);
            console.log('Selected rating:', selectedRating);
        });
    });

    function highlightStars(count) {
        $('#rating-star-container div').each(function (i) {
            $(this).css('opacity', i < count ? '1' : '0');
        });
    }

    loadRecipeRatings();
    loadUserRating();

    // Handle sort buttons
    $('.sort-button').on('click', function () {
        $('.sort-button').removeClass('selected border-blue-400 bg-blue-100 text-blue-700 hover:bg-blue-200')
            .addClass('border-gray-300 bg-gray-100 text-gray-700 hover:bg-gray-200');

        $(this).removeClass('border-gray-300 bg-gray-100 text-gray-700 hover:bg-gray-200')
            .addClass('selected border-blue-400 bg-blue-100 text-blue-700 hover:bg-blue-200');

        queryOptions.sortBy = $(this).text().trim();
        queryOptions.page = 1;
        loadRecipeRatings();
    });

    $('#items-per-page-select').on('change', function () {
        queryOptions.pageSize = $(this).val();
        queryOptions.page = 1;
        loadRecipeRatings();
    });

    // Handle pagination button clicks
    $('#pagination-buttons-container').on('click', 'button[data-page]', function () {
        const newPage = parseInt($(this).data('page'));
        if (!isNaN(newPage)) {
            queryOptions.page = newPage;
            loadRecipeRatings();
        }
    });

    $('#submit-rating-btn').on('click', function () {
        let message = $('#rating-message').val();
        const recipeId = $('#recipeId').val();

        // Validate rating
        if (!selectedRating || selectedRating < 1 || selectedRating > 5) {
            Swal.fire({
                icon: 'warning',
                title: 'Invalid Rating',
                text: 'Please select a rating between 1 and 5 stars.',
                confirmButtonText: 'OK',
                customClass: {
                    confirmButton: 'bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600'
                }
            });
            return;
        }

        const data = {
            recipeId: recipeId,
            rating: selectedRating,
            comment: message
        };

        // Show loading alert
        Swal.fire({
            title: 'Submitting Rating',
            text: 'Please wait...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            url: '/api/recipe-ratings',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                // Close loading alert
                Swal.close();

                // Show success alert
                Swal.fire({
                    icon: 'success',
                    title: 'Rating Submitted!',
                    text: 'Your rating has been successfully submitted.',
                    confirmButtonText: 'OK',
                    customClass: {
                        confirmButton: 'bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600'
                    }
                }).then(() => {
                    // Reload user rating and recipe ratings
                    loadUserRating();
                    loadRecipeRatings();
                    // Reset the form
                    $('#rating-message').val('');
                    selectedRating = 0;
                    highlightStars(selectedRating);
                });
            },
            error: function (error) {
                // Close loading alert
                Swal.close();

                // Show error alert
                let errorMessage = 'An error occurred while submitting your rating.';
                if (error.status === 400) {
                    errorMessage = 'Invalid rating data. Please check your input.';
                } else if (error.status === 401) {
                    errorMessage = 'Please log in to submit a rating.';
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Submission Failed',
                    text: errorMessage,
                    confirmButtonText: 'OK',
                    customClass: {
                        confirmButton: 'bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600'
                    }
                });
            }
        });
    });
});

async function getUserRating() {
    const recipeId = $('#recipeId').val();

    try {
        const data = await $.ajax({
            url: "/api/recipe-ratings/user/" + recipeId,
            type: 'GET'
        });
        return data;
    } catch (error) {
        console.error('Error fetching user rating:', error);
        throw error;
    }
}

function loadUserRating() {
    const $container = $('#user-rating-container');

    // Show loading state
    $container.html(`
        <div class="flex justify-center w-full items-center h-24">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
        </div>
    `);

    getUserRating()
        .then(data => {
            console.log('User rating data:', data);
            // Clear container
            $container.empty();

            // Generate star rating HTML
            let starsHtml = '';
            for (let i = 1; i <= 5; i++) {
                starsHtml += `
                    <svg class="w-5 h-5 inline-block ${i <= data.rating ? 'text-yellow-400' : 'text-gray-300'
                    }" fill="currentColor" viewBox="0 0 20 20">
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                    </svg>
                `;
            }

            // Render rating
            $container.html(`
                <div class="flex w-full items-start space-x-4">
                    <img src="${data.userProfileImage || '/default-avatar.png'}" 
                         alt="User profile" 
                         class="w-12 h-12 rounded-full object-cover">
                    <div class="flex-1">
                        <div class="flex items-center justify-between">
                            <div class="flex items-center space-x-2">
                                <span class="font-semibold text-gray-900">${data.userName}</span>
                                <div class="flex">${starsHtml}</div>
                            </div>
                            <span class="text-sm text-gray-500">
                                ${new Date(data.createdAt).toLocaleDateString()}
                            </span>
                        </div>
                        <p class="mt-2 text-gray-600">${data.comment == "" ? 'No comment yet.' : data.comment}</p>
                    </div>
                </div>
            `);
        })
        .catch(error => {
            console.error('Error loading user rating:', error);
            // Clear container
            $container.empty();

            // Handle 400 status (no rating found)
            if (error.status === 400) {
                $container.html(`
                    <div class="text-center text-gray-500 py-4">
                        No rating yet
                    </div>
                `);
            } else {
                // Handle other errors
                $container.html(`
                    <div class="text-center text-red-500 py-4">
                        Please login to see your rating.
                    </div>
                `);
            }
        });
}

async function getRecipeRatings() {
    const recipeId = $('#recipeId').val();

    try {
        const data = await $.ajax({
            url: `/api/recipe-ratings/${recipeId}`,
            type: 'GET',
            data: {
                sortBy: queryOptions.sortBy,
                page: queryOptions.page,
                pageSize: queryOptions.pageSize
            }
        });
        return data;
    } catch (error) {
        console.error('Error fetching recipe ratings:', error);
        throw error;
    }
}

async function loadRecipeRatings() {
    try {
        const data = await getRecipeRatings();
        renderPaginationButtons(data, queryOptions);
        console.log('Data:', data);

        renderRatingsList(data.items);
    } catch (error) {
        console.error('Error loading ratings:', error);
    }
}

function renderStars(starCount) {
    const filled = '★'.repeat(starCount);
    const empty = '☆'.repeat(5 - starCount);
    return filled + empty;
}

function timeAgo(dateString) {
    // Truncate microseconds if needed (e.g., from "2025-05-14T10:07:03.3902234" to "2025-05-14T10:07:03.390")
    const safeDateString = dateString.split('.')[0] + 'Z'; // Ensure it's ISO8601 with Zulu timezone
    const past = new Date(safeDateString);
    const now = new Date();

    const seconds = Math.floor((now - past) / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (isNaN(past.getTime())) return "Invalid date";

    if (days > 7) return past.toLocaleDateString(); // fallback
    if (days >= 1) return `${days} day${days > 1 ? 's' : ''} ago`;
    if (hours >= 1) return `${hours} hour${hours > 1 ? 's' : ''} ago`;
    if (minutes >= 1) return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
    return "Just now";
}



function renderRatingsList(ratings) {
    const $container = $('#rating-list');
    $container.empty();

    if (!ratings || ratings.length === 0) {
        $container.append(`<div class="text-gray-500 italic text-center py-8">No ratings yet.</div>`);
        return;
    }

    ratings.forEach(rating => {
        const stars = renderStars(rating.rating);
        const dateLabel = timeAgo(rating.updatedAt);
        const comment = rating.comment == "" ? 'No comment yet.' : rating.comment;
        const userName = rating.userName || 'Anonymous';
        const avatarUrl = rating.userProfileImage || '/images/default-avatar.png';

        const html = `
            <div class="p-4 bg-white rounded-xl shadow-md hover:shadow-lg transition-shadow mb-4">
                <div class="flex max-sm:flex-col sm:items-start justify-between gap-4 mb-3">
                    <div class="flex items-center gap-3">
                        <img src="${avatarUrl}" alt="${userName}'s avatar" class="w-10 h-10 rounded-full object-cover ring-1 ring-gray-200">
                        <div class="flex flex-col gap-1">
                            <div class="text-gray-800 font-medium text-sm">${userName}</div>
                            <div class="flex gap-1 text-yellow-500 text-base">${stars}</div>
                        </div>
                    </div>
                    <div class="text-gray-400 text-xs sm:text-sm max-sm:text-right">${dateLabel}</div>
                </div>
                <p class="text-gray-700 text-base leading-6">${comment}</p>
            </div>
        `;

        $container.append(html);
    });
}


function renderPaginationButtons(data, queryOptions) {
    const totalItems = data.totalCount;
    const pageSize = data.pageSize;
    const currentPage = data.pageNumber;
    const totalPages = Math.ceil(totalItems / pageSize);

    // update query options
    queryOptions.page = currentPage;
    queryOptions.pageSize = pageSize;

    const $container = $('#pagination-buttons-container');
    $container.empty();

    const createButton = (label, page, isDisabled, isActive = false) => {
        const baseClass = "px-3 py-1 text-sm rounded mx-1";
        const hoverClass = isDisabled ? "bg-gray-200 text-gray-400 cursor-not-allowed" : "bg-gray-100 hover:bg-gray-200 cursor-pointer";
        const activeClass = isActive ? "border border-blue-400 bg-blue-100 text-blue-700 shadow-sm" : "";
        return `<button class="${baseClass} ${hoverClass} ${activeClass}" data-page="${page}" ${isDisabled ? "disabled" : ""}>${label}</button>`;
    };

    const buttonsHtml = [
        createButton("« First", 1, currentPage === 1),
        createButton("‹ Prev", Math.max(currentPage - 1, 1), currentPage === 1),
        createButton(currentPage, currentPage, false, true),
        createButton("Next ›", Math.min(currentPage + 1, totalPages), currentPage >= totalPages),
        createButton("Last »", totalPages, currentPage >= totalPages)
    ];

    $container.html(buttonsHtml.join(''));
}
