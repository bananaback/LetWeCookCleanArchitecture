using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IngredientVisibleRequiredMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "visible",
                table: "ingredients",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "visible",
                table: "ingredients",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
