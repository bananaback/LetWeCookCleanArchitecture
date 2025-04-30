using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedRecipeTagMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789012"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("23456789-2345-2345-2345-234567890123"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("34567890-3456-3456-3456-345678901234"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("45678901-4567-4567-4567-456789012345"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "recipe_tags",
                keyColumn: "id",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));
        }
    }
}
