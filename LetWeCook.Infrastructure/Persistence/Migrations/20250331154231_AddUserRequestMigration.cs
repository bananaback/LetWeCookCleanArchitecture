using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRequestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_ingredient_categories_CategoryId",
                table: "ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_media_urls_CoverImageUrlId",
                table: "ingredients");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrlId",
                table: "ingredients",
                newName: "cover_image_url_id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "ingredients",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_CoverImageUrlId",
                table: "ingredients",
                newName: "IX_ingredients_cover_image_url_id");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_CategoryId",
                table: "ingredients",
                newName: "IX_ingredients_category_id");

            migrationBuilder.CreateTable(
                name: "user_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    reference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_user_requests_created_by_user_id",
                table: "user_requests",
                column: "created_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_ingredient_categories_category_id",
                table: "ingredients",
                column: "category_id",
                principalTable: "ingredient_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_media_urls_cover_image_url_id",
                table: "ingredients",
                column: "cover_image_url_id",
                principalTable: "media_urls",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_ingredient_categories_category_id",
                table: "ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_media_urls_cover_image_url_id",
                table: "ingredients");

            migrationBuilder.DropTable(
                name: "user_requests");

            migrationBuilder.RenameColumn(
                name: "cover_image_url_id",
                table: "ingredients",
                newName: "CoverImageUrlId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "ingredients",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_cover_image_url_id",
                table: "ingredients",
                newName: "IX_ingredients_CoverImageUrlId");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_category_id",
                table: "ingredients",
                newName: "IX_ingredients_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_ingredient_categories_CategoryId",
                table: "ingredients",
                column: "CategoryId",
                principalTable: "ingredient_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_media_urls_CoverImageUrlId",
                table: "ingredients",
                column: "CoverImageUrlId",
                principalTable: "media_urls",
                principalColumn: "id");
        }
    }
}
