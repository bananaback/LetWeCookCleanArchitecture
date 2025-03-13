$(document).ready(function () {
    // Load profile data when the page loads
    loadUserProfile();

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
    });

    $(".save-btn").on("click", function () {
        const profileData = {
            profilePicture: $(".profile-picture").attr("src") || "",
            firstName: $(".first-name").val().trim(),
            lastName: $(".last-name").val().trim(),
            bio: $(".bio").val().trim() || null,
            birthDate: $(".birth-date").val(),
            gender: $(".gender-btn.bg-green-500").data("gender") || "",
            email: $(".email").val().trim(),
            phoneNumber: $(".phone-number").val().trim() || null,
            instagram: $(".instagram").val().trim() || null,
            facebook: $(".facebook").val().trim() || null,
            address: {
                houseNumber: $(".house-number").val().trim(),
                street: $(".street").val().trim(),
                ward: $(".ward").val().trim(),
                district: $(".district").val().trim(),
                city: $(".city").val().trim(),
                province: $(".province").val().trim()
            },
            dietaryPreferences: $(".peer:checked").map(function () {
                return $(this).next("span").text().trim();
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
        if (!profileData.address.city || profileData.address.city.length < 3 || profileData.address.city.length > 50) {
            errors.push("City must be between 3 and 50 characters.");
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
            }
        });
    });

    function loadUserProfile() {
        $.ajax({
            url: "/api/profile",
            method: "GET",
            success: function (data) {
                $(".profile-picture").attr("src", data.profilePicture || "/default-profile.png");
                $(".first-name").val(data.firstName || "");
                $(".last-name").val(data.lastName || "");
                $(".bio").val(data.bio || "");
                $(".birth-date").val(data.birthDate ? new Date(data.birthDate).toISOString().split('T')[0] : ""); // Ensure valid date format
                $(".email").val(data.email || "");
                $(".phone-number").val(data.phoneNumber || "");
                $(".instagram").val(data.instagram || "");
                $(".facebook").val(data.facebook || "");

                $(".house-number").val(data.address?.houseNumber || "");
                $(".street").val(data.address?.street || "");
                $(".ward").val(data.address?.ward || "");
                $(".district").val(data.address?.district || "");
                $(".city").val(data.address?.city || "");
                $(".province").val(data.address?.province || "");

                // Handle gender selection properly
                $(".gender-btn").removeClass("bg-green-500 text-white").addClass("bg-gray-200 text-gray-600");

                let selectedGender = data.gender && data.gender !== "Unspecified" ? data.gender : "Unspecified";
                $(".gender-btn[data-gender='" + selectedGender + "']")
                    .removeClass("bg-gray-200 text-gray-600")
                    .addClass("bg-green-500 text-white")
                    .find("input").prop("checked", true);

                // Handle dietary preferences
                $(".peer").prop("checked", false);
                if (data.dietaryPreferences) {
                    $(".peer").each(function () {
                        if (data.dietaryPreferences.includes($(this).next("span").text().trim())) {
                            $(this).prop("checked", true);
                        }
                    });
                }
            },
            error: function () {
                console.error("Failed to load profile data.");
            }
        });
    }

});
