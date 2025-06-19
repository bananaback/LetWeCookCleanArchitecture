import { GoogleGenerativeAI } from "https://esm.sh/@google/generative-ai";

// Initialize the Google Generative AI client with the API key from .env
const genAI = new GoogleGenerativeAI("AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94");

let ingredientId = "";

function populateInputs(data) {
    $('#name').val(data.name);
    $('#category_id').val(data.categoryId);
    $('#description').val(data.description);

    // Populate Cover Image
    if (data.coverImageUrl) {
        $('#cover_image_url_id').val(data.coverImageUrl);
        $('#cover-preview').html(`
            <div class="relative inline-block">
                <img src="${data.coverImageUrl}" class="w-32 h-32 object-cover rounded-md">
                <button type="button" class="absolute top-0 right-0 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-cover">×</button>
            </div>
        `);

        $('.remove-cover').click(function () {
            $('#cover_image_url_id').val('');
            $('#cover-preview').empty();
        });
    } else {
        $('#cover_image_url_id').val('');
        $('#cover-preview').empty();
    }

    $('#calories').val(data.calories);
    $('#protein').val(data.protein);
    $('#carbohydrates').val(data.carbohydrates);
    $('#fats').val(data.fats);
    $('#sugars').val(data.sugars);
    $('#fiber').val(data.fiber);
    $('#sodium').val(data.sodium);

    $('#is_vegetarian').prop('checked', data.isVegetarian);
    $('#is_vegan').prop('checked', data.isVegan);
    $('#is_glutenFree').prop('checked', data.isGlutenFree);
    $('#is_pescatarian').prop('checked', data.isPescatarian);

    $('#cover_image_url_id').val(data.coverImageUrl);
    $('#expiration_days').val(data.expirationDays);

    populateDetails(data.details);
}

function populateDetails(details) {
    $('#details-container').empty();

    details
        .sort((a, b) => a.order - b.order)
        .forEach(detail => {
            const mediaHtml = detail.mediaUrls.map(url => `
                <div class="relative media-item" data-url="${url}">
                    <img src="${url}" class="w-24 h-24 object-cover rounded-md">
                    <button type="button" class="absolute top-0 right-0 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-media">×</button>
                </div>
            `).join('');

            const detailHtml = `
                <div class="detail-item draggable border border-teal-200 rounded-md p-4 mb-4 relative" draggable="true">
                    <div class="flex items-center justify-between mb-2">
                        <input type="text" name="detail_title[]" value="${detail.title || ''}" placeholder="Detail Title" 
                               class="w-full p-2 border border-teal-200 rounded-md focus:ring-2 focus:ring-teal-500">
                        <button type="button" class="ml-2 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-detail shrink-0">×</button>
                    </div>
                    <textarea name="detail_description[]" placeholder="Detail Description" rows="2"
                              class="w-full p-2 mb-2 border border-teal-200 rounded-md focus:ring-2 focus:ring-teal-500">${detail.description || ''}</textarea>
                    <button type="button" class="upload-media w-full p-2 bg-cream-200 border border-teal-300 rounded-md hover:bg-cream-300">
                        Upload Media
                    </button>
                    <div class="media-preview flex flex-wrap gap-2 mt-2">
                        ${mediaHtml}
                    </div>
                    <input type="hidden" name="detail_media[]" class="media-urls" value="${detail.mediaUrls.join(',')}">
                </div>
            `;

            $('#details-container').append(detailHtml);
        });
}



function fetchEditingIngredient() {
    // call the API to get the ingredient data with ajax
    $.ajax({
        url: `/api/ingredient/${ingredientId}/editing`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            populateInputs(data); // Populate the inputs with the data
            console.log("Ingredient Data:", data); // Log the ingredient data
        },
        error: function (xhr) {
            // check for 403 and route to home if so
            if (xhr.status === 403) {
                Swal.fire({
                    icon: 'error',
                    title: 'Unauthorized',
                    text: 'You are not authorized to view this ingredient.',
                    customClass: {
                        confirmButton: 'swal-custom-btn'
                    },
                    didOpen: () => {
                        $('.swal-custom-btn').css({
                            'background-color': '#dc3545', // Red
                            'color': '#FFFFFF' // White text
                        });
                    },
                }).then(() => {
                    window.location.href = '/Home'; // Redirect to home page
                });
            } else {
                console.error('Error fetching ingredient data:', xhr.responseText); // Log the error response
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Failed to fetch ingredient data.',
                    customClass: {
                        confirmButton: 'swal-custom-btn'
                    },
                    didOpen: () => {
                        $('.swal-custom-btn').css({
                            'background-color': '#dc3545', // Red
                            'color': '#FFFFFF' // White text
                        });
                    }
                });
            }
        }
    });
}

$(document).ready(function () {
    ingredientId = $('#ingredient-id').text();
    $('#ingredient-id').remove(); // Remove the element from the DOM

    fetchEditingIngredient();

    populateCategorySelect();

    let detailCount = 0;

    // Cloudinary Configuration
    const cloudinaryConfig = {
        cloud_name: 'dxclyqubm',
        upload_preset: 'letwecook_preset',
        sources: ['local', 'url', 'camera'],
        max_file_size: 10000000,
        client_allowed_formats: ["jpg", "jpeg", "png"]
    };

    // Cover Image Upload
    $('#upload-cover').click(function (event) {
        event.preventDefault();
        cloudinary.openUploadWidget({
            ...cloudinaryConfig,
            multiple: false
        }, function (error, result) {
            if (!error && result && result.event === "success") {
                const uploadedUrl = result.info.secure_url;
                $('#cover_image_url_id').val(uploadedUrl);
                $('#cover-preview').html(`
                                <div class="relative inline-block">
                                    <img src="${uploadedUrl}" class="w-32 h-32 object-cover rounded-md">
                                    <button type="button" class="absolute top-0 right-0 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-cover">×</button>
                                </div>
                            `);
                $('.remove-cover').click(function () {
                    $('#cover_image_url_id').val('');
                    $('#cover-preview').empty();
                });
            }
        });
    });

    // Add Detail
    $('#add-detail').click(function () {
        const detailHtml = `
            <div class="detail-item draggable border border-teal-200 rounded-md p-4 mb-4 relative" draggable="true">
                <div class="flex items-center justify-between mb-2">
                    <input type="text" name="detail_title[]" placeholder="Detail Title" 
                           class="w-full p-2 border border-teal-200 rounded-md focus:ring-2 focus:ring-teal-500">
                    <button type="button" class="ml-2 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-detail shrink-0">×</button>
                </div>
                <textarea name="detail_description[]" placeholder="Detail Description" rows="2"
                          class="w-full p-2 mb-2 border border-teal-200 rounded-md focus:ring-2 focus:ring-teal-500"></textarea>
                <button type="button" class="upload-media w-full p-2 bg-cream-200 border border-teal-300 rounded-md hover:bg-cream-300">
                    Upload Media
                </button>
                <div class="media-preview flex flex-wrap gap-2 mt-2"></div>
                <input type="hidden" name="detail_media[]" class="media-urls" value="">
            </div>
        `;
        $('#details-container').append(detailHtml);
    });

    $(document).on('click', '.remove-detail', function () {
        $(this).closest('.detail-item').remove();
    });

    // Media Upload for Details
    $(document).on('click', '.upload-media', function (event) {
        event.preventDefault();
        const $detailItem = $(this).closest('.detail-item');
        const $preview = $detailItem.find('.media-preview');
        const $mediaUrls = $detailItem.find('.media-urls');
        let currentUrls = $mediaUrls.val() ? $mediaUrls.val().split(',') : [];

        cloudinary.openUploadWidget({
            ...cloudinaryConfig,
            multiple: true
        }, function (error, results) {
            if (!error && results && results.event === "success") {
                $.each(results.info.files || [results.info], function (i, file) {
                    const uploadedUrl = file.secure_url;
                    currentUrls.push(uploadedUrl);
                    const previewHtml = `
                        <div class="relative media-item" data-url="${uploadedUrl}">
                            <img src="${uploadedUrl}" class="w-24 h-24 object-cover rounded-md">
                            <button type="button" class="absolute top-0 right-0 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-media">×</button>
                        </div>
                    `;
                    $preview.append(previewHtml);
                    $mediaUrls.val(currentUrls.join(','));
                });
            }
        });
    });

    // Remove Media
    $(document).on('click', '.remove-media', function () {
        const $mediaItem = $(this).closest('.media-item');
        const $detailItem = $mediaItem.closest('.detail-item');
        const $mediaUrls = $detailItem.find('.media-urls');
        const urlToRemove = $mediaItem.data('url');
        let currentUrls = $mediaUrls.val().split(',');

        currentUrls = currentUrls.filter(url => url !== urlToRemove);
        $mediaUrls.val(currentUrls.join(','));
        $mediaItem.remove();
    });

    // Auto-fill Nutrition Values with AI
    $('#auto-fill-nutrition').on('click', autoFillNutritionValues);

    $('#save-btn').on('click', async function () {
        const data = extractInputs();

        console.log("Ingredient Update Data:", data);

        if (!data) {
            return; // Stop execution if validation failed
        }


        // Ensure ingredientId is present
        if (!ingredientId) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ingredient ID is missing.',
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
            return;
        }

        // Call Gemini API to review the ingredient update
        const review = await reviewIngredient(data);

        if (!review) {
            // Handle API failure
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to evaluate the ingredient update. Please try again later.',
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
            return;
        }

        const { decision, explanation } = review;

        // Prepare request data with AcceptImmediately
        const requestData = {
            ...data,
            AcceptImmediately: decision === 'accept'
        };

        if (decision === 'reject') {
            // Show rejection alert
            Swal.fire({
                icon: 'error',
                title: 'Ingredient Update Rejected',
                html: `Your update for "<strong>${data.name}</strong>" was rejected.<br><br>Reason: ${explanation}`,
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
        } else {
            console.log("ingredientId:", ingredientId);
            // Call the endpoint for accept or needs review
            $.ajax({
                url: `/api/ingredient/${ingredientId}`,
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify(requestData),
                success: function (response) {
                    if (decision === 'accept') {
                        Swal.fire({
                            icon: 'success',
                            title: 'Ingredient Updated',
                            html: `Your update for "<strong>${data.name}</strong>" has been accepted!<br><br>Reason: ${explanation}`,
                            customClass: {
                                confirmButton: 'swal-custom-btn'
                            },
                            didOpen: () => {
                                $('.swal-custom-btn').css({
                                    'background-color': '#28a745', // Green
                                    'color': '#FFFFFF'
                                });
                            }
                        });
                    } else {
                        Swal.fire({
                            icon: 'info',
                            title: 'Update Request Submitted',
                            html: `Your update request for "<strong>${data.name}</strong>" has been submitted and is awaiting admin review.<br><br>Reason: ${explanation}<br><br>
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
                    }
                },
                error: function (xhr) {
                    console.log("Error Response:", xhr.responseText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseJSON?.message || 'Something went wrong while processing the ingredient update.',
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
    });

    $('#debug-btn').on('click', function () {
        $('#name').val('Testing apple' + Math.floor(Math.random() * 1000));
        $('#description').val('A test ingredient for debugging purposes.');
        // set default guid for category_id
        $('#category_id').val('00000000-0000-0000-0000-000000000008');
        const defaultUrl = 'https://th.bing.com/th/id/R.e7feee901a35fcb48b0ea3267298792f?rik=XMBDloAHyW5l5w&riu=http%3a%2f%2fimages6.fanpop.com%2fimage%2fphotos%2f34900000%2fApple-fruit-34914774-693-693.jpg&ehk=xkzD601LGQUIwWLmIjwDApmx6A7DM1foeMgDAPz4X7Q%3d&risl=&pid=ImgRaw&r=0';
        // set deafult value for cover image url
        $('#cover_image_url_id').val(defaultUrl);
        $('#cover-preview').html(`
                                <div class="relative inline-block">
                                    <img src="${defaultUrl}" class="w-32 h-32 object-cover rounded-md">
                                    <button type="button" class="absolute top-0 right-0 bg-red-500 text-white w-6 h-6 rounded-full flex items-center justify-center remove-cover">×</button>
                                </div>
                            `);
        $('.remove-cover').click(function () {
            $('#cover_image_url_id').val('');
            $('#cover-preview').empty();
        });
        $('#add-detail').click();
        $('.detail-item').each(function (index) {
            let $detail = $(this);
            let title = $detail.find('input[name="detail_title[]"]').val("Test detail")
            let description = $detail.find('textarea[name="detail_description[]"]').val("Test description");
        });
    });
});

async function reviewIngredient(ingredientData) {
    try {
        const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

        const prompt = `
You are an AI assistant tasked with evaluating ingredient submissions for a recipe app. Your goal is to assess each submission and return one of three decisions: "accept", "reject", or "needs review". Use the following criteria to evaluate the JSON submission provided below. Balance strictness and leniency to ensure high-quality submissions while flagging ambiguous cases for human review. Provide a brief explanation for your decision.

### Evaluation Criteria
1. **Completeness**:
   - Required fields (\`name\`, \`categoryId\`, \`description\`, \`nutritionValues\`, \`dietaryInfo\`, \`details\`) must be present and non-empty unless specified as optional.
   - \`expirationDays\` and \`mediaUrls\` in \`details\` are optional but should be considered for quality.
   - \`coverImage\` must be a valid URL (starts with "http" or "https").
2. **Quality**:
   - \`name\`, \`description\`, and \`details\` fields (\`title\`, \`description\`) must be meaningful, clear, and not gibberish (e.g., "dasdsadsad" is invalid).
   - Descriptions should be at least 10 characters and provide useful information.
   - \`details\` entries should have meaningful titles and descriptions, with at least one entry having a valid \`mediaUrls\` if provided.
3. **Consistency**:
   - \`nutritionValues\` (calories, protein, carbohydrates, fats, sugars, fiber, sodium) must be non-negative numbers and reasonable (e.g., calories < 1000 for a single ingredient).
   - \`dietaryInfo\` flags (\`isVegetarian\`, \`isVegan\`, \`isGlutenFree\`, \`isPescatarian\`) must be consistent (e.g., \`isVegan\` implies \`isVegetarian\%).
4. **Edge Cases**:
   - If a submission is mostly valid but has minor issues (e.g., vague description, missing optional fields, or slightly inconsistent dietary info), return "needs review".
   - Reject submissions with critical issues (e.g., missing required fields, gibberish text, invalid URLs, or illogical nutrition values).
   - Accept submissions that meet all criteria with no significant issues.

### Output Format
Return a JSON object with two fields:
- \`decision\`: One of "accept", "reject", or "needs review".
- \`explanation\`: A brief (1-2 sentences) explanation of the decision.

### Submission to Evaluate
${JSON.stringify(ingredientData)}

### Example Output
{
  "decision": "needs review",
  "explanation": "The description and details contain vague text ('dasdsadsad'), but other fields are valid, warranting human review."
}
`;

        const result = await model.generateContent(prompt);
        const response = await result.response;
        let text = response.text();

        // Extract JSON from response (handles cases where response includes extra text)
        const jsonMatch = text.match(/{[\s\S]*}/);
        if (!jsonMatch) {
            throw new Error("No valid JSON found in response");
        }

        text = jsonMatch[0];
        const reviewData = JSON.parse(text);

        // Validate the response structure
        if (!reviewData.decision || !reviewData.explanation) {
            throw new Error("Invalid response format: missing decision or explanation");
        }

        console.log("Review Data:\n", JSON.stringify(reviewData, null, 2));

        return reviewData; // Return { decision, explanation }

    } catch (error) {
        console.error("Error calling Gemini API for review:", error.message);
        return null; // Return null on error to handle gracefully
    }
}

async function autoFillNutritionValues() {
    const ingredientName = $('#name').val().trim();

    if (!ingredientName) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Ingredient',
            text: 'Please enter an ingredient name before fetching nutrition data.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }

    Swal.fire({
        title: 'Fetching Nutrition Data...',
        text: `Getting details for ${ingredientName}`,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        },
    });

    try {
        const nutritionData = await getNutrition(ingredientName);

        if (!nutritionData) {
            Swal.fire({
                icon: 'error',
                title: 'No Data Found',
                text: `Could not retrieve nutrition information for "${ingredientName}".`,
                customClass: {
                    confirmButton: 'swal-custom-btn'
                },
                didOpen: () => {
                    $('.swal-custom-btn').css({
                        'background-color': '#DC3545', // Red
                        'color': '#FFFFFF'
                    });
                }
            });
            return;
        }

        // Populate form fields correctly, preserving 0 values
        $('#calories').val(nutritionData.calories ?? '');
        $('#protein').val(nutritionData.protein ?? '');
        $('#carbohydrates').val(nutritionData.carbohydrates ?? '');
        $('#fats').val(nutritionData.fats ?? '');
        $('#sugars').val(nutritionData.sugars ?? '');
        $('#fiber').val(nutritionData.fiber ?? '');
        $('#sodium').val(nutritionData.sodium ?? '');

        // Update checkboxes
        $('#is_vegetarian').prop('checked', nutritionData.isVegetarian || false);
        $('#is_vegan').prop('checked', nutritionData.isVegan || false);
        $('#is_glutenFree').prop('checked', nutritionData.isGlutenFree || false);
        $('#is_pescatarian').prop('checked', nutritionData.isPescatarian || false);

        Swal.fire({
            icon: 'success',
            title: 'Nutrition Data Filled',
            text: `Nutrition details for "${ingredientName}" have been added.`,
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#28A745', // Green
                    'color': '#FFFFFF'
                });
            }
        });

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error Fetching Data',
            text: error.message || 'An unknown error occurred.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#DC3545', // Red
                    'color': '#FFFFFF'
                });
            }
        });
    }
}


async function getNutrition(ingredient) {
    try {
        const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

        const prompt = `
      Fill in the missing nutrition fields for this ingredient in concise JSON format. Use approximate values per 100g if not specified. Keep it structured and consistent.

      Ingredient: ${ingredient}

      Response format:
      {
        "calories": <int>,
        "protein": <float>,
        "carbohydrates": <float>,
        "fats": <float>,
        "sugars": <float>,
        "fiber": <float>,
        "sodium": <float>,
        "isVegetarian": <bool>,
        "isVegan": <bool>,
        "isGlutenFree": <bool>,
        "isPescatarian": <bool>
      }
    `;

        const result = await model.generateContent(prompt);
        const response = await result.response;
        let text = response.text();

        const jsonMatch = text.match(/{[\s\S]*}/);
        if (!jsonMatch) {
            throw new Error("No valid JSON found in response");
        }

        text = jsonMatch[0]; // Extract JSON
        const nutritionData = JSON.parse(text);

        console.log("Nutrition Data:\n", JSON.stringify(nutritionData, null, 2));

        return nutritionData; // ✅ Return parsed JSON

    } catch (error) {
        console.error("Error calling Gemini API:", error.message);
        return null; // ✅ Ensure function returns something on error
    }
}

function populateCategorySelect() {
    $.ajax({
        url: '/api/ingredient-categories',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            const $categorySelect = $('#category_id');
            $categorySelect.empty();
            $categorySelect.append('<option value="">Select Category</option>');

            $.each(data, function (index, category) {
                $categorySelect.append(`<option value="${category.id}">${category.name}</option>`);
            });
        },
        error: function (error) {
            console.error('Error fetching categories:', error);
        }
    });
}

function extractName() {
    return $('#name').val();
}

function extractCategory() {
    return $('#category_id').find(':selected').val();
}

function extractDescription() {
    return $('#description').val();
}

function extractNutritionValues() {
    let nutritionValues = {
        calories: parseFloat($('#calories').val()) || null,
        protein: parseFloat($('#protein').val()) || null,
        carbohydrates: parseFloat($('#carbohydrates').val()) || null,
        fats: parseFloat($('#fats').val()) || null,
        sugars: parseFloat($('#sugars').val()) || null,
        fiber: parseFloat($('#fiber').val()) || null,
        sodium: parseFloat($('#sodium').val()) || null,
    };
    return nutritionValues;
}


function extractDietaryInformation() {
    let dietaryInfo = {
        isVegetarian: $('#is_vegetarian').is(':checked'),
        isVegan: $('#is_vegan').is(':checked'),
        isGlutenFree: $('#is_glutenFree').is(':checked'),
        isPescatarian: $('#is_pescatarian').is(':checked'),
    };
    return dietaryInfo;
}

function extractCoverImage() {
    return $('#cover_image_url_id').val();
}

function extractExpirationDays() {
    return parseFloat($('#expiration_days').val());
}

function extractDetails() {
    let details = [];
    $('.detail-item').each(function (index) {
        let $detail = $(this);
        let title = $detail.find('input[name="detail_title[]"]').val().trim();
        let description = $detail.find('textarea[name="detail_description[]"]').val().trim();
        let mediaUrls = $detail.find('.media-urls').val() || ""; // Default to empty string if undefined
        let mediaArray = mediaUrls ? mediaUrls.split(',').filter(url => url.trim() !== "") : [];

        details.push({
            order: index, // Start from 0 for synchronization
            title: title,
            description: description,
            mediaUrls: mediaArray
        });
    });
    return details;
}

function extractInputs() {
    // Declare variables for each property
    const name = extractName();
    const categoryId = extractCategory();
    const description = extractDescription();
    const nutritionValues = extractNutritionValues();
    const dietaryInfo = extractDietaryInformation();
    const coverImage = extractCoverImage();
    const expirationDays = extractExpirationDays();
    const details = extractDetails();

    // Validate each property
    if (!name) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Name',
            text: 'Please enter a name for the ingredient.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }

    function isValidGuid(guid) {
        if (typeof guid !== "string") return false;

        let parts = guid.split("-");
        return (
            parts.length === 5 &&
            parts[0].length === 8 &&
            parts[1].length === 4 &&
            parts[2].length === 4 &&
            parts[3].length === 4 &&
            parts[4].length === 12
        );
    }

    if (!isValidGuid(categoryId)) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Category',
            text: 'Please select a category for the ingredient.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }

    if (!description) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Description',
            text: 'Please provide a description for the ingredient.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }

    if (!coverImage) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Cover Image',
            text: 'Please upload a cover image for the ingredient.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }

    if (details.length === 0 || details.some(d => !d.title && !d.description && d.mediaUrls.length === 0)) {
        Swal.fire({
            icon: 'warning',
            title: 'Incomplete Detail',
            text: 'Each detail must have at least a title, description, or media.',
            customClass: {
                confirmButton: 'swal-custom-btn'
            },
            didOpen: () => {
                $('.swal-custom-btn').css({
                    'background-color': '#007BFF', // Blue
                    'color': '#FFFFFF' // White text
                });
            }
        });
        return;
    }


    // Return a JSON object with all the extracted data
    return {
        name,
        categoryId,
        description,
        nutritionValues,
        dietaryInfo,
        coverImage,
        expirationDays,
        details
    };
}