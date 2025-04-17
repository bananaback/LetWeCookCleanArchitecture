using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PreviewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "user_requests",
                newName: "new_reference_id");

            migrationBuilder.AddColumn<Guid>(
                name: "old_reference_id",
                table: "user_requests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_preview",
                table: "ingredients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "old_reference_id",
                table: "user_requests");

            migrationBuilder.DropColumn(
                name: "is_preview",
                table: "ingredients");

            migrationBuilder.RenameColumn(
                name: "new_reference_id",
                table: "user_requests",
                newName: "reference_id");
        }
    }
}
