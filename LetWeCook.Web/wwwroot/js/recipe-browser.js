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
        $(this).toggleClass('selected').toggleClass('bg-amber-100').toggleClass('bg-amber-300');
        showSelectedDifficulty();
    });

    $('#category-container').on('click', '.category-chip', function () {
        $(this).toggleClass('selected').toggleClass('bg-amber-100').toggleClass('bg-amber-300');
        showSelectedCategory();
    });

    $('#tag-container').on('click', '.tag-chip', function () {
        $(this).toggleClass('selected').toggleClass('bg-amber-100').toggleClass('bg-amber-300');
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

            // Swal.fire({
            //     title: `${criteriaName} Sorting`,
            //     text: $(this).is(':checked') ? 'Sorting enabled' : 'Sorting disabled',
            //     icon: 'info',
            //     confirmButtonText: 'OK'
            // });
        });

        // Direction click (ascending/descending)
        $direction.on('click', function () {
            $(this).toggleClass('rotate-180');
            let direction = $(this).data('direction');
            let newDirection = direction === 'descending' ? 'ascending' : 'descending';
            $(this).data('direction', newDirection);

            Swal.fire({
                title: `${criteriaName} Sort Direction Changed`,
                text: `Sort direction changed to ${newDirection}`,
                icon: 'info',
                confirmButtonText: 'OK'
            });
        });
    });

    updateBrowsingOptions();

    $('#apply-filter-btn').on('click', function () {

        collectBrowsingOptions();
        updateBrowsingOptions();

        console.log('Browsing Options:', browsingOptions);

        // send ajax with request body browsingOptions to /api/recipes
        $.ajax({
            url: '/api/recipes-browser',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(browsingOptions),
            success: function (response) {
                // Handle success response
                console.log('Filtered recipes:', response);
                // You can update the UI with the filtered recipes here
            },
            error: function (error) {
                // Handle error response
                console.error('Error fetching filtered recipes:', error);
            }
        });
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
                    <div class="category-chip cursor-pointer px-2 py-1 text-xs bg-amber-100 border border-amber-300 rounded-full hover:bg-amber-200">
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
                    <div class="difficulty-chip cursor-pointer px-2 py-1 text-xs bg-amber-100 border border-amber-300 rounded-full hover:bg-amber-200">
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
                    <div class="tag-chip cursor-pointer px-2 py-1 text-xs bg-amber-100 border border-amber-300 rounded-full hover:bg-amber-200" data-id="${tag.id}">
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
});

function showSelectedDifficulty() {
    let selectedDifficulties = $('.difficulty-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');

    Swal.fire({
        title: 'Selected Difficulties',
        text: selectedDifficulties || 'None selected',
        icon: 'info',
        confirmButtonText: 'OK'
    });
}

function showSelectedCategory() {
    let selectedCategories = $('.category-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');

    // Swal.fire({
    //     title: 'Selected Categories',
    //     text: selectedCategories || 'None selected',
    //     icon: 'info',
    //     confirmButtonText: 'OK'
    // });
}

function showSelectedTag() {
    let selectedTags = $('.tag-chip.selected').map(function () {
        return $(this).text().trim();
    }).get().join(', ');

    // Swal.fire({
    //     title: 'Selected Tags',
    //     text: selectedTags || 'None selected',
    //     icon: 'info',
    //     confirmButtonText: 'OK'
    // });
}

function updateBrowsingOptions() {
    $('#browsing-options').text(JSON.stringify(browsingOptions, null, 4));
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

    browsingOptions.createdByUsername = $('#created-by-username').val().trim();

    browsingOptions.itemsPerPage = parseInt($('#items-per-page-select').val(), 10) || 10;

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
    itemsPerPage: 10,
    page: 1
};