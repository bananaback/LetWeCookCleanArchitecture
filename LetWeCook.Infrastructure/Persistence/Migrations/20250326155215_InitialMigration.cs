using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dietary_preferences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    color = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    emoji = table.Column<string>(type: "nvarchar(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dietary_preferences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "media_urls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    media_type = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(2048)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_urls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_removed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    date_joined = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    calories = table.Column<double>(type: "float", nullable: true),
                    protein = table.Column<double>(type: "float", nullable: true),
                    carbohydrates = table.Column<double>(type: "float", nullable: true),
                    fat = table.Column<double>(type: "float", nullable: true),
                    sugar = table.Column<double>(type: "float", nullable: true),
                    fiber = table.Column<double>(type: "float", nullable: true),
                    sodium = table.Column<double>(type: "float", nullable: true),
                    is_vegetarian = table.Column<bool>(type: "bit", nullable: false),
                    is_vegan = table.Column<bool>(type: "bit", nullable: false),
                    is_gluten_free = table.Column<bool>(type: "bit", nullable: false),
                    is_pescatarian = table.Column<bool>(type: "bit", nullable: false),
                    CoverImageUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    expiration_days = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.id);
                    table.ForeignKey(
                        name: "FK_ingredients_ingredient_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ingredient_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ingredients_media_urls_CoverImageUrlId",
                        column: x => x.CoverImageUrlId,
                        principalTable: "media_urls",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_site_users_SiteUserId",
                        column: x => x.SiteUserId,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    house_number = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    street = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ward = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    district = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    province_or_city = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    birth_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    gender = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    bio = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    facebook_url = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    instagram_url = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(15)", nullable: true),
                    profile_picture_url = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_profiles_site_users_user_id",
                        column: x => x.user_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_details_ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_dietary_preferences",
                columns: table => new
                {
                    user_profile_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dietary_preference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_dietary_preferences", x => new { x.user_profile_id, x.dietary_preference_id });
                    table.ForeignKey(
                        name: "FK_user_dietary_preferences_dietary_preferences_dietary_preference_id",
                        column: x => x.dietary_preference_id,
                        principalTable: "dietary_preferences",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_dietary_preferences_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detail_media_urls",
                columns: table => new
                {
                    DetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detail_media_urls", x => new { x.DetailId, x.MediaUrlId });
                    table.ForeignKey(
                        name: "FK_detail_media_urls_details_DetailId",
                        column: x => x.DetailId,
                        principalTable: "details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detail_media_urls_media_urls_MediaUrlId",
                        column: x => x.MediaUrlId,
                        principalTable: "media_urls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "dietary_preferences",
                columns: new[] { "id", "color", "description", "emoji", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "#808080", "No specific dietary preference", "❌", "None" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "#4CAF50", "Excludes meat, includes dairy and eggs", "🥦", "Vegetarian" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "#8BC34A", "Excludes all animal products", "🌱", "Vegan" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "#FF9800", "Excludes gluten-containing grains", "🚫🌾", "GlutenFree" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "#03A9F4", "Excludes meat but allows fish", "🐟", "Pescatarian" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "#FFEB3B", "Focuses on low-calorie meals", "🔥", "LowCalorie" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "#FF5722", "Emphasizes protein-rich foods", "💪", "HighProtein" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "#9C27B0", "Limits carbohydrate intake (Keto-friendly)", "🥩", "LowCarb" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "#2196F3", "Focuses on reducing fat intake", "🥗", "LowFat" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "#E91E63", "Reduces added sugar consumption", "🚫🍭", "LowSugar" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "#673AB7", "Emphasizes fiber-rich foods", "🌾", "HighFiber" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "#00BCD4", "Focuses on reducing salt intake", "🧂🚫", "LowSodium" }
                });

            migrationBuilder.InsertData(
                table: "ingredient_categories",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "All types of red and white meat", "Meat" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Chicken, turkey, duck, and other birds", "Poultry" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Fish, shellfish, and other seafood", "Seafood" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "All types of eggs used in cooking", "Eggs" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Milk, cheese, yogurt, and butter", "Dairy" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Plant-based dairy substitutes like almond milk and soy milk", "Dairy alternatives" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Fresh, frozen, and canned vegetables", "Vegetables" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Fresh, dried, and preserved fruits", "Fruits" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Rice, wheat, oats, and other grains", "Grains" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "Beans, lentils, chickpeas, and peas", "Legumes" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "Fresh and dried herbs like basil and parsley", "Herbs" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "All types of spices such as cinnamon and cumin", "Spices" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "Cooking oils such as olive oil and vegetable oil", "Oils" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "Sugar, honey, and artificial sweeteners", "Sweeteners" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "Non-alcoholic and alcoholic drinks", "Beverages" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Sauces, dressings, and seasonings", "Condiments" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Pickled and fermented ingredients like kimchi and miso", "Fermented foods" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Almonds, cashews, walnuts, and other tree nuts", "Nuts" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Chia seeds, flaxseeds, sunflower seeds, and others", "Seeds" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Flour, yeast, baking powder, and cocoa powder", "Baking essentials" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Vegetable, chicken, and beef broths or stocks", "Broths and stocks" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Fallback category for unspecified ingredients", "Uncategorized" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SiteUserId",
                table: "AspNetUsers",
                column: "SiteUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_detail_media_urls_MediaUrlId",
                table: "detail_media_urls",
                column: "MediaUrlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_details_IngredientId",
                table: "details",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_CategoryId",
                table: "ingredients",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_CoverImageUrlId",
                table: "ingredients",
                column: "CoverImageUrlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_dietary_preferences_dietary_preference_id",
                table: "user_dietary_preferences",
                column: "dietary_preference_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "detail_media_urls");

            migrationBuilder.DropTable(
                name: "user_dietary_preferences");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "details");

            migrationBuilder.DropTable(
                name: "dietary_preferences");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "site_users");

            migrationBuilder.DropTable(
                name: "ingredient_categories");

            migrationBuilder.DropTable(
                name: "media_urls");
        }
    }
}
