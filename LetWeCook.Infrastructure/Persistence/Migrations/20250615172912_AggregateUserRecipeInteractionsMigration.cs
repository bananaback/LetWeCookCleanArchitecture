using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetWeCook.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AggregateUserRecipeInteractionsMigration : Migration
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
                        UserId,
                        RecipeId,
                        COUNT(CASE WHEN InteractionType = 'Like' THEN 1 END) AS LikeCount,
                        COUNT(CASE WHEN InteractionType = 'Dislike' THEN 1 END) AS DislikeCount,
                        AVG(CAST(Rating AS FLOAT)) AS AvgRating,
                        SUM(CommentLength) AS TotalCommentLength,
                        SUM(DonatedAmount) AS TotalDonated,
                        COUNT(CASE WHEN InteractionType = 'View' THEN 1 END) AS ViewsCount
                    FROM Interactions
                    GROUP BY UserId, RecipeId;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS AggregateUserRecipeInteractions");
        }
    }
}
