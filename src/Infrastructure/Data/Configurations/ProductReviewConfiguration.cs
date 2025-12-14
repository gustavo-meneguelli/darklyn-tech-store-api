using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.ProductId)
            .IsRequired();

        builder.Property(pr => pr.UserId)
            .IsRequired();

        builder.Property(pr => pr.Rating)
            .IsRequired();

        builder.Property(pr => pr.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(pr => pr.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        // Relacionamento com Product
        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com User
        builder.HasOne(pr => pr.User)
            .WithMany()
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único: um usuário só pode avaliar um produto uma vez
        builder.HasIndex(pr => new { pr.ProductId, pr.UserId })
            .IsUnique();

        // Soft Delete QueryFilter
        builder.HasQueryFilter(pr => !pr.IsDeleted);

        // Audit fields
        builder.Property(pr => pr.CreatedAt)
            .IsRequired();

        builder.Property(pr => pr.UpdatedAt);

        builder.Property(pr => pr.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
