using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

    CREATE PROCEDURE AggregateUserRecipeInteractions
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            user_id AS UserId,
            recipe_id AS RecipeId,
            COUNT(CASE WHEN interaction_type = 'like' THEN 1 END) AS LikeCount,
            COUNT(CASE WHEN interaction_type = 'dislike' THEN 1 END) AS DislikeCount,
            COUNT(CASE WHEN interaction_type = 'view' THEN 1 END) AS ViewsCount,

            ISNULL(CAST(AVG(CASE WHEN interaction_type = 'rating' THEN TRY_CAST(event_value AS REAL) END) AS REAL), 3) AS Rating,
            ISNULL(CAST(SUM(CASE WHEN interaction_type = 'comment' THEN TRY_CAST(event_value AS REAL) END) AS REAL), 0) AS CommentLength,
            ISNULL(CAST(SUM(CASE WHEN interaction_type = 'donate' THEN TRY_CAST(event_value AS REAL) END) AS REAL), 0) AS DonatedAmount

        FROM user_interactions
        GROUP BY user_id, recipe_id;
    END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS AggregateUserRecipeInteractions;");

        }
    }
}
