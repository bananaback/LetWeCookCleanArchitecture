$(document).ready(function () {
    const donationId = $('#donation-id').text();

    if (!donationId) {
        Swal.fire({
            icon: 'error',
            title: 'Missing Donation ID',
            text: 'Donation ID is missing from URL.',
            confirmButtonColor: '#15803d'
        });
        return;
    }

    // Call GET /api/donations/donationId
    $.ajax({
        url: 'https://localhost:7212/api/donations/' + encodeURIComponent(donationId),
        method: 'GET',
        success: function (data) {
            populateDonationDetails(data);
        },
        error: function (xhr) {
            const errorMessage = xhr.responseJSON?.message || 'Failed to load donation details.';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage,
                confirmButtonColor: '#15803d'
            });
        }
    });
});

function populateDonationDetails(data) {
    // Amount
    $('#amount').text(`$${data.amount.toFixed(2)} (USD)`);

    // Recipe Name
    $('#recipe-name').text(data.recipeOverview.name);

    // Recipe Cover Image
    $('#recipe-cover').attr('src', data.recipeOverview.coverImageUrl);

    // Donator Name (or Anonymous)
    const donatorName = data.donatorProfileDto?.name || 'Anonymous';
    $('#donator-name').text(donatorName);

    // Author Name
    $('#author-name').text(data.authorProfileDto.name);

    // Author Picture (fallback if null)
    const authorPicUrl = data.authorProfileDto.profilePicUrl || 'https://via.placeholder.com/80';
    $('#author-pic').attr('src', authorPicUrl);

    // Author Bio (fallback)
    const authorBio = data.authorProfileDto.bio || 'Recipe creator at LetWeCook.';
    $('#author-bio').text(authorBio);

    // Message
    $('#donation-message').text(data.donateMessage);

    // Date
    const formattedDate = formatDateTime(data.createdAt);
    $('#created-at').text(formattedDate);
}

function formatDateTime(isoString) {
    const date = new Date(isoString);
    const options = { year: 'numeric', month: 'short', day: '2-digit', hour: '2-digit', minute: '2-digit' };
    return date.toLocaleString('en-US', options);
}