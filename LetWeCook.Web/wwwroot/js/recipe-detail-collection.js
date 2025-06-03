async function getCollections() {
    return $.ajax({
        url: '/api/collections',
        method: 'GET',
        contentType: 'application/json',
        dataType: 'json'
    });
}

async function addRecipeToCollection(request) {
    return $.ajax({
        url: `/api/collections`,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request)
    });
}


$(document).ready(function () {
    let collections = [];

    // Radio toggle logic
    $('input[name="collectionMode"]').on('change', function () {
        const mode = $(this).val();
        if (mode === 'existing') {
            $('#existingCollectionSection').removeClass('hidden');
            $('#newCollectionSection').addClass('hidden');
        } else {
            $('#newCollectionSection').removeClass('hidden');
            $('#existingCollectionSection').addClass('hidden');
        }
    });

    // Open modal
    $('#openModalBtn').on('click', async function () {
        $('#recipeModal').removeClass('hidden').addClass('flex');

        collections = await getCollections();
        console.log('Collections fetched:', collections);
        const $dropdown = $('#collections');
        $dropdown.empty();
        $dropdown.append('<option selected disabled>Select a collection</option>');
        collections.forEach(col => {
            $dropdown.append(`<option value="${col.collectionId}">${col.name}</option>`);
        });

        // Default mode
        $('#selectExisting').prop('checked', true).trigger('change');
        $('#newCollection').val('');
        $('#collections').val('');
    });

    // Close modal
    $('#closeModalBtn').on('click', function () {
        $('#recipeModal').addClass('hidden').removeClass('flex');
    });

    $('#recipeModal').on('click', function (e) {
        if ($(e.target).is('#recipeModal')) {
            $('#recipeModal').addClass('hidden').removeClass('flex');
        }
    });

    // Add to collection logic
    $('#addToCollectionBtn').on('click', async function () {
        const mode = $('input[name="collectionMode"]:checked').val();
        const recipeId = $('#recipeId').val(); // assume you pass recipeId as data attribute

        if (!recipeId) {
            console.error('No recipe ID provided.');
            return;
        }

        if (mode === 'existing') {
            const selectedId = $('#collections').val();
            if (!selectedId) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Please select a collection.',
                    toast: true,
                    position: 'top-end',
                    timer: 2000,
                    showConfirmButton: false
                });
                return;
            }

            const request = {
                isNewCollection: false,
                recipeId: recipeId,
                collectionId: selectedId,
                collectionName: ''
            };

            try {
                await addRecipeToCollection(request);
                $('#recipeModal').addClass('hidden').removeClass('flex');
                const selectedName = $('#collections option:selected').text();
                Swal.fire({
                    icon: 'success',
                    title: `Recipe saved to "${selectedName}"!`,
                    toast: true,
                    position: 'top-end',
                    timer: 2000,
                    showConfirmButton: false
                });
            } catch (err) {
                Swal.fire({
                    icon: 'error',
                    title: 'Failed to add recipe to collection.',
                    text: err.responseText || 'An error occurred.',
                });
            }

        } else if (mode === 'new') {
            const newName = $('#newCollection').val().trim();

            if (!newName) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Please enter a new collection name.',
                    toast: true,
                    position: 'top-end',
                    timer: 2000,
                    showConfirmButton: false
                });
                return;
            }

            const isDuplicate = collections.some(col => col.name.toLowerCase() === newName.toLowerCase());

            if (isDuplicate) {
                Swal.fire({
                    icon: 'error',
                    title: 'Collection already exists!',
                    toast: true,
                    position: 'top-end',
                    timer: 2000,
                    showConfirmButton: false
                });
                return;
            }

            const request = {
                isNewCollection: true,
                recipeId: recipeId,
                collectionId: '00000000-0000-0000-0000-000000000000', // placeholder GUID
                collectionName: newName
            };

            try {
                await addRecipeToCollection(request);
                $('#recipeModal').addClass('hidden').removeClass('flex');
                Swal.fire({
                    icon: 'success',
                    title: `Created "${newName}" and saved recipe!`,
                    toast: true,
                    position: 'top-end',
                    timer: 2000,
                    showConfirmButton: false
                });
            } catch (err) {
                Swal.fire({
                    icon: 'error',
                    title: 'Failed to create collection or save recipe.',
                    text: err.responseText || 'An error occurred.',
                });
            }
        }
    });

});