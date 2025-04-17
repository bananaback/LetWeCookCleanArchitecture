using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientVisibleMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "created_by_user_id",
                table: "ingredients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "ingredients",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visible",
                table: "ingredients");

            migrationBuilder.AlterColumn<Guid>(
                name: "created_by_user_id",
                table: "ingredients",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
