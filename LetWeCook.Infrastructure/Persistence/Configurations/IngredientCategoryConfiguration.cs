using LetWeCook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LetWeCook.Infrastructure.Persistence.Configurations
{
    public class IngredientCategoryConfiguration : IEntityTypeConfiguration<IngredientCategory>
    {
        public void Configure(EntityTypeBuilder<IngredientCategory> builder)
        {
            builder.ToTable("ingredient_categories");

            builder.HasKey(ic => ic.Id);

            builder.Property(ic => ic.Id)
                .HasColumnName("id")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(ic => ic.Name)
                .HasColumnName("name")
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.Property(ic => ic.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(1024)")
                .IsRequired();

            // Seed data
            builder.HasData(
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000001"), "Meat", "All types of red and white meat"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000002"), "Poultry", "Chicken, turkey, duck, and other birds"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000003"), "Seafood", "Fish, shellfish, and other seafood"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000004"), "Eggs", "All types of eggs used in cooking"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000005"), "Dairy", "Milk, cheese, yogurt, and butter"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000006"), "Dairy alternatives", "Plant-based dairy substitutes like almond milk and soy milk"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000007"), "Vegetables", "Fresh, frozen, and canned vegetables"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000008"), "Fruits", "Fresh, dried, and preserved fruits"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000009"), "Grains", "Rice, wheat, oats, and other grains"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000A"), "Legumes", "Beans, lentils, chickpeas, and peas"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000B"), "Herbs", "Fresh and dried herbs like basil and parsley"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000C"), "Spices", "All types of spices such as cinnamon and cumin"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000D"), "Oils", "Cooking oils such as olive oil and vegetable oil"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000E"), "Sweeteners", "Sugar, honey, and artificial sweeteners"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-00000000000F"), "Beverages", "Non-alcoholic and alcoholic drinks"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000010"), "Condiments", "Sauces, dressings, and seasonings"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000011"), "Fermented foods", "Pickled and fermented ingredients like kimchi and miso"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000012"), "Nuts", "Almonds, cashews, walnuts, and other tree nuts"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000013"), "Seeds", "Chia seeds, flaxseeds, sunflower seeds, and others"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000014"), "Baking essentials", "Flour, yeast, baking powder, and cocoa powder"),
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000015"), "Broths and stocks", "Vegetable, chicken, and beef broths or stocks"),

                // Fallback category
                new IngredientCategory(new Guid("00000000-0000-0000-0000-000000000016"), "Uncategorized", "Fallback category for unspecified ingredients")
            );

        }
    }
}
