using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ingredients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_CreatedByUserId",
                table: "ingredients",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ingredients_site_users_CreatedByUserId",
                table: "ingredients",
                column: "CreatedByUserId",
                principalTable: "site_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ingredients_site_users_CreatedByUserId",
                table: "ingredients");

            migrationBuilder.DropIndex(
                name: "IX_ingredients_CreatedByUserId",
                table: "ingredients");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ingredients");
        }
    }
}
