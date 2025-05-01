$(document).ready(function () {
    // Load profile data when the page loads
    loadUserProfile();

    function loadDietaryPreferences(selectedPreferences = []) {
        $.ajax({
            url: "/api/dietary-preferences",
            method: "GET",
            success: function (data) {
                renderDietaryPreferences(data, selectedPreferences);
            },
            error: function () {
                console.error("Failed to load dietary preferences.");
            }
        });
    }

    function renderDietaryPreferences(preferences, selectedPreferences) {
        const container = $(".dietary-preferences-container");
        container.empty(); // Clear existing preferences

        preferences.forEach(pref => {
            const isChecked = selectedPreferences.includes(pref.name) ? "checked" : "";
            const preferenceHTML = `
                <label class="group relative flex cursor-pointer flex-col items-center rounded-xl p-5 shadow-md transition hover:scale-105 hover:shadow-lg" style="background-color: ${pref.color}">
                    <input type="checkbox" class="peer hidden" name="dietaryPreferences" value="${pref.name}" ${isChecked}>
                    <div class="absolute top-3 right-3 flex items-center justify-center h-6 w-6 rounded-full border-2 border-white peer-checked:bg-white peer-checked:border-gray-700">
                        <svg class="hidden peer-checked:block w-4 h-4 text-gray-700" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7"></path>
                        </svg>
                    </div>
                    <div class="mb-3 flex h-12 w-12 items-center justify-center text-3xl text-white">${pref.emoji}</div>
                    <span class="text-lg font-medium text-white">${pref.name}</span>
                    <p class="opacity-80 mt-1 text-center text-sm text-white">${pref.description}</p>
                </label>
            `;
            container.append(preferenceHTML);
        });
    }

    $("#btnUploadPhoto").click(function (event) {
        event.preventDefault();
        cloudinary.openUploadWidget({
            cloud_name: 'dxclyqubm',
            upload_preset: 'letwecook_preset',
            sources: ['local', 'url', 'camera'],
            multiple: false,
            max_file_size: 10000000,
            client_allowed_formats: ["jpg", "jpeg", "png"]
        }, function (error, result) {
            if (!error && result && result.event === "success") {
                let uploadedUrl = result.info.secure_url;
                $("#photoPreview, .profile-picture").attr("src", uploadedUrl);
            }
        });
    });

    $(".gender-btn").click(function () {
        $(".gender-btn").removeClass("bg-green-500 text-white").addClass("bg-gray-200 text-gray-600");
        $(this).removeClass("bg-gray-200 text-gray-600").addClass("bg-green-500 text-white");
        $(this).find("input").prop("checked", true);
        console.log("gender hey");
    });

    $(".save-btn").on("click", function () {
        let $btn = $(this);
        if ($btn.prop("disabled")) return; // Prevent multiple submissions

        $btn.prop("disabled", true); // Disable button to prevent multiple clicks

        const profileData = {
            profilePicture: ($(".profile-picture").attr("src").endsWith("/images/default-profile.jpg"))
                ? ""
                : $(".profile-picture").attr("src"),
            firstName: $(".first-name").val().trim(),
            lastName: $(".last-name").val().trim(),
            bio: $(".bio").val().trim() || null,
            birthDate: $(".birth-date").val(),
            gender: $(".gender-btn.bg-green-500 input").val() || "",
            email: $(".email").val().trim(),
            phoneNumber: $(".phone-number").val().trim() || null,
            paypalEmail: $(".paypal-email").val().trim() || null, // NEW PayPal email field
            instagram: $(".instagram").val().trim() || null,
            facebook: $(".facebook").val().trim() || null,
            address: {
                houseNumber: $(".house-number").val().trim(),
                street: $(".street").val().trim(),
                ward: $(".ward").val().trim(),
                district: $(".district").val().trim(),
                province: $(".province").val().trim()
            },
            dietaryPreferences: $(".peer:checked").map(function () {
                return $(this).val(); // Get the checkbox value instead of trying to extract text
            }).get()

        };

        let errors = [];

        if (!profileData.firstName || profileData.firstName.length < 2 || profileData.firstName.length > 50) {
            errors.push("First name must be between 2 and 50 characters.");
        }
        if (!profileData.lastName || profileData.lastName.length < 2 || profileData.lastName.length > 50) {
            errors.push("Last name must be between 2 and 50 characters.");
        }
        if (!profileData.birthDate || new Date(profileData.birthDate) >= new Date()) {
            errors.push("Please enter a valid birth date.");
        }
        if (!profileData.gender) {
            errors.push("Please select a gender.");
        }
        if (!profileData.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(profileData.email)) {
            errors.push("Please enter a valid email.");
        }
        if (profileData.phoneNumber && !/^\+?[0-9\s-]{7,20}$/.test(profileData.phoneNumber)) {
            errors.push("Please enter a valid phone number.");
        }
        if (profileData.paypalEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(profileData.paypalEmail)) {
            errors.push("Please enter a valid PayPal email.");
        }
        if (!profileData.address.houseNumber || profileData.address.houseNumber.length > 10) {
            errors.push("House number must not be empty and max 10 characters.");
        }
        if (!profileData.address.street || profileData.address.street.length < 5 || profileData.address.street.length > 100) {
            errors.push("Street must be between 5 and 100 characters.");
        }
        if (!profileData.address.ward || profileData.address.ward.length < 3 || profileData.address.ward.length > 50) {
            errors.push("Ward must be between 3 and 50 characters.");
        }
        if (!profileData.address.district || profileData.address.district.length < 3 || profileData.address.district.length > 50) {
            errors.push("District must be between 3 and 50 characters.");
        }
        if (!profileData.address.province || profileData.address.province.length < 3 || profileData.address.province.length > 50) {
            errors.push("Province must be between 3 and 50 characters.");
        }

        if (errors.length > 0) {
            Swal.fire({
                title: "Oops! Something went wrong",
                icon: "warning",
                html: `<ul style="text-align: left; font-size: 14px; color: #d32f2f; padding: 0; list-style-type: none;">
                    ${errors.map(error => `<li>• ${error}</li>`).join("")}
                </ul>`,
                confirmButtonText: "Try Again",
                confirmButtonColor: "#d32f2f",
                background: "#fff5f5",
            });
            $btn.prop("disabled", false); // Re-enable button if validation fails
            return;
        }

        $.ajax({
            url: "/api/profile",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(profileData),
            success: function (response) {
                Swal.fire({
                    title: "Profile Updated!",
                    text: "Your profile information has been successfully saved.",
                    icon: "success",
                    confirmButtonText: "Great!",
                    confirmButtonColor: "#4CAF50",
                    background: "#F1F8E9",
                });
            },
            error: function (xhr) {
                let message = "An error occurred while updating your profile.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    message = xhr.responseJSON.message;
                }
                Swal.fire({
                    title: "Error!",
                    text: message,
                    icon: "error",
                    confirmButtonText: "Try Again",
                    confirmButtonColor: "#d32f2f",
                });
            },
            complete: function () {
                $btn.prop("disabled", false); // Re-enable button after request completes
            }
        });
    });


    function loadUserProfile() {
        $.ajax({
            url: "/api/profile",
            method: "GET",
            success: function (data) {
                console.log(data);
                $(".profile-picture").attr("src", data.profilePic || "/images/default-profile.jpg");
                $(".first-name").val(data.firstName || "");
                $(".last-name").val(data.lastName || "");
                $(".bio").val(data.bio || "");
                $(".birth-date").val(data.birthDate ? new Date(data.birthDate).toISOString().split('T')[0] : ""); // Ensure valid date format
                $(".email").val(data.email || "");
                $(".phone-number").val(data.phoneNumber || "");
                $(".paypal-email").val(data.payPalEmail || ""); // Load PayPal email

                $(".instagram").val(data.instagram || "");
                $(".facebook").val(data.facebook || "");

                $(".house-number").val(data.houseNumber || "");
                $(".street").val(data.street || "");
                $(".ward").val(data.ward || "");
                $(".district").val(data.district || "");
                $(".province").val(data.provinceOrCity || "");

                // Handle gender selection properly
                $(".gender-btn").removeClass("bg-green-500 text-white").addClass("bg-gray-200 text-gray-600");


                // Ensure selectedGender is valid
                let selectedGender = data.gender ? data.gender.toLowerCase() : "unspecified";

                // Select the correct button based on gender value
                if (selectedGender === "male") {
                    $(".gender-btn.male")
                        .removeClass("bg-gray-200 text-gray-600")
                        .addClass("bg-green-500 text-white")
                        .find("input")
                        .prop("checked", true);
                } else if (selectedGender === "female") {
                    $(".gender-btn.female")
                        .removeClass("bg-gray-200 text-gray-600")
                        .addClass("bg-green-500 text-white")
                        .find("input")
                        .prop("checked", true);
                }

                loadDietaryPreferences(data.dietaryPreferences);
            },
            error: function () {
                console.error("Failed to load profile data.");
            }
        });
    }

});
