using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixDonationNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donation_recipes_RecipeId",
                table: "Donation");

            migrationBuilder.DropForeignKey(
                name: "FK_Donation_site_users_AuthorId",
                table: "Donation");

            migrationBuilder.DropForeignKey(
                name: "FK_Donation_site_users_DonatorId",
                table: "Donation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Donation",
                table: "Donation");

            migrationBuilder.RenameTable(
                name: "Donation",
                newName: "donations");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "donations",
                newName: "recipe_id");

            migrationBuilder.RenameColumn(
                name: "DonatorId",
                table: "donations",
                newName: "donator_id");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "donations",
                newName: "author_id");

            migrationBuilder.RenameIndex(
                name: "IX_Donation_RecipeId",
                table: "donations",
                newName: "IX_donations_recipe_id");

            migrationBuilder.RenameIndex(
                name: "IX_Donation_DonatorId",
                table: "donations",
                newName: "IX_donations_donator_id");

            migrationBuilder.RenameIndex(
                name: "IX_Donation_AuthorId",
                table: "donations",
                newName: "IX_donations_author_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_donations",
                table: "donations",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_donations_recipes_recipe_id",
                table: "donations",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_donations_site_users_author_id",
                table: "donations",
                column: "author_id",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_donations_site_users_donator_id",
                table: "donations",
                column: "donator_id",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donations_recipes_recipe_id",
                table: "donations");

            migrationBuilder.DropForeignKey(
                name: "FK_donations_site_users_author_id",
                table: "donations");

            migrationBuilder.DropForeignKey(
                name: "FK_donations_site_users_donator_id",
                table: "donations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_donations",
                table: "donations");

            migrationBuilder.RenameTable(
                name: "donations",
                newName: "Donation");

            migrationBuilder.RenameColumn(
                name: "recipe_id",
                table: "Donation",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "donator_id",
                table: "Donation",
                newName: "DonatorId");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "Donation",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_donations_recipe_id",
                table: "Donation",
                newName: "IX_Donation_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_donations_donator_id",
                table: "Donation",
                newName: "IX_Donation_DonatorId");

            migrationBuilder.RenameIndex(
                name: "IX_donations_author_id",
                table: "Donation",
                newName: "IX_Donation_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Donation",
                table: "Donation",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_recipes_RecipeId",
                table: "Donation",
                column: "RecipeId",
                principalTable: "recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_site_users_AuthorId",
                table: "Donation",
                column: "AuthorId",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_site_users_DonatorId",
                table: "Donation",
                column: "DonatorId",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
