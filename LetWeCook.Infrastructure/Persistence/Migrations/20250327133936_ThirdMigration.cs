using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_site_users_CreatedByUserId",
                table: "ingredients");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "ingredients",
                newName: "created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_CreatedByUserId",
                table: "ingredients",
                newName: "IX_ingredients_created_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_site_users_created_by_user_id",
                table: "ingredients",
                column: "created_by_user_id",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_site_users_created_by_user_id",
                table: "ingredients");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "ingredients",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ingredients_created_by_user_id",
                table: "ingredients",
                newName: "IX_ingredients_CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_site_users_CreatedByUserId",
                table: "ingredients",
                column: "CreatedByUserId",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
