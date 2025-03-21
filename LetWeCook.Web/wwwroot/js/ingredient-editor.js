import { GoogleGenerativeAI } from "https://esm.sh/@google/generative-ai";

// Initialize the Google Generative AI client with the API key from .env
const genAI = new GoogleGenerativeAI("AIzaSyCdBbtTfxOgYKBq7frKFmwOlOKSLDjxY94");

$(document).ready(function () {
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

});

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
