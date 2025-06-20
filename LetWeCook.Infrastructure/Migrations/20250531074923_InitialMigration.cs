﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetWeCook.Infrastructure.Migrations
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
                name: "details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_details", x => x.id);
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
                name: "recipe_tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_tags", x => x.id);
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
                name: "detail_media_urls",
                columns: table => new
                {
                    detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    media_url_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detail_media_urls", x => new { x.detail_id, x.media_url_id });
                    table.ForeignKey(
                        name: "FK_detail_media_urls_details_detail_id",
                        column: x => x.detail_id,
                        principalTable: "details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detail_media_urls_media_urls_media_url_id",
                        column: x => x.media_url_id,
                        principalTable: "media_urls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "ingredients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    visible = table.Column<bool>(type: "bit", nullable: false),
                    is_preview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    cover_image_url_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    expiration_days = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.id);
                    table.ForeignKey(
                        name: "FK_ingredients_ingredient_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "ingredient_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ingredients_media_urls_cover_image_url_id",
                        column: x => x.cover_image_url_id,
                        principalTable: "media_urls",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ingredients_site_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    recipe_count = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_collections", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_collections_site_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    servings = table.Column<int>(type: "int", nullable: false),
                    prepare_time = table.Column<int>(type: "int", nullable: false),
                    cook_time = table.Column<int>(type: "int", nullable: false),
                    difficulty_level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meal_category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cover_media_url_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    average_rating = table.Column<double>(type: "float", nullable: false),
                    total_ratings = table.Column<int>(type: "int", nullable: false),
                    total_views = table.Column<int>(type: "int", nullable: false),
                    is_visible = table.Column<bool>(type: "bit", nullable: false),
                    is_preview = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipes_media_urls_cover_media_url_id",
                        column: x => x.cover_media_url_id,
                        principalTable: "media_urls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipes_site_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                    paypal_email = table.Column<string>(type: "nvarchar(255)", nullable: true),
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
                name: "user_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    old_reference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    new_reference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    response_message = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_by_user_name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_requests_site_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "site_users",
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
                name: "ingredient_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    ingredient_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_ingredient_details_details_detail_id",
                        column: x => x.detail_id,
                        principalTable: "details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ingredient_details_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    donator_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    author_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    transaction_id = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    donate_message = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    approval_url = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donations", x => x.id);
                    table.ForeignKey(
                        name: "FK_donations_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_donations_site_users_author_id",
                        column: x => x.author_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_donations_site_users_donator_id",
                        column: x => x.donator_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_collection_items",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    collection_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    added_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_collection_items", x => new { x.recipe_id, x.collection_id });
                    table.ForeignKey(
                        name: "FK_recipe_collection_items_recipe_collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "recipe_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_collection_items_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_recipe_details_details_detail_id",
                        column: x => x.detail_id,
                        principalTable: "details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_details_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ingredient_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quantity = table.Column<double>(type: "float", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredients", x => new { x.recipe_id, x.ingredient_id });
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ratings",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ratings", x => new { x.recipe_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_recipe_ratings_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_ratings_site_users_user_id",
                        column: x => x.user_id,
                        principalTable: "site_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_tags_recipes",
                columns: table => new
                {
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    recipe_tag_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_tags_recipes", x => new { x.recipe_id, x.recipe_tag_id });
                    table.ForeignKey(
                        name: "FK_recipe_tags_recipes_recipe_tags_recipe_tag_id",
                        column: x => x.recipe_tag_id,
                        principalTable: "recipe_tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_tags_recipes_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suggestion_feedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    like_or_dislike = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suggestion_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_suggestion_feedbacks_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suggestion_feedbacks_site_users_user_id",
                        column: x => x.user_id,
                        principalTable: "site_users",
                        principalColumn: "id",
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

            migrationBuilder.InsertData(
                table: "recipe_tags",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Vietnamese" },
                    { new Guid("12345678-1234-1234-1234-123456789012"), "Brazilian" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Chinese" },
                    { new Guid("23456789-2345-2345-2345-234567890123"), "Caribbean" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Mexican" },
                    { new Guid("34567890-3456-3456-3456-345678901234"), "African" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Italian" },
                    { new Guid("45678901-4567-4567-4567-456789012345"), "German" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Indian" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Thai" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Japanese" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Korean" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "American" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "French" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Mediterranean" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Middle Eastern" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Spanish" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Greek" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Turkish" }
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
                name: "IX_detail_media_urls_media_url_id",
                table: "detail_media_urls",
                column: "media_url_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_donations_author_id",
                table: "donations",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_donations_donator_id",
                table: "donations",
                column: "donator_id");

            migrationBuilder.CreateIndex(
                name: "IX_donations_recipe_id",
                table: "donations",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_details_detail_id",
                table: "ingredient_details",
                column: "detail_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_details_ingredient_id",
                table: "ingredient_details",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_category_id",
                table: "ingredients",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_cover_image_url_id",
                table: "ingredients",
                column: "cover_image_url_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_created_by_user_id",
                table: "ingredients",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_collection_items_collection_id",
                table: "recipe_collection_items",
                column: "collection_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_collections_created_by_id",
                table: "recipe_collections",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_details_detail_id",
                table: "recipe_details",
                column: "detail_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_details_recipe_id",
                table: "recipe_details",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_ingredient_id",
                table: "recipe_ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ratings_user_id",
                table: "recipe_ratings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_tags_recipes_recipe_tag_id",
                table: "recipe_tags_recipes",
                column: "recipe_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_cover_media_url_id",
                table: "recipes",
                column: "cover_media_url_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipes_created_by_id",
                table: "recipes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_suggestion_feedbacks_recipe_id",
                table: "suggestion_feedbacks",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_suggestion_feedbacks_user_id",
                table: "suggestion_feedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_dietary_preferences_dietary_preference_id",
                table: "user_dietary_preferences",
                column: "dietary_preference_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_requests_created_by_user_id",
                table: "user_requests",
                column: "created_by_user_id");
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
                name: "donations");

            migrationBuilder.DropTable(
                name: "ingredient_details");

            migrationBuilder.DropTable(
                name: "recipe_collection_items");

            migrationBuilder.DropTable(
                name: "recipe_details");

            migrationBuilder.DropTable(
                name: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "recipe_ratings");

            migrationBuilder.DropTable(
                name: "recipe_tags_recipes");

            migrationBuilder.DropTable(
                name: "suggestion_feedbacks");

            migrationBuilder.DropTable(
                name: "user_dietary_preferences");

            migrationBuilder.DropTable(
                name: "user_requests");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "recipe_collections");

            migrationBuilder.DropTable(
                name: "details");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "recipe_tags");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "dietary_preferences");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "ingredient_categories");

            migrationBuilder.DropTable(
                name: "media_urls");

            migrationBuilder.DropTable(
                name: "site_users");
        }
    }
}
