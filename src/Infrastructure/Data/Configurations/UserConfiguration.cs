using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired();

        // Campos de perfil
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.AvatarChoice)
            .IsRequired();

        builder.Property(u => u.HasCompletedFirstPurchaseReview)
            .IsRequired()
            .HasDefaultValue(false);

        // Propriedades calculadas - não mapeadas para o banco
        builder.Ignore(u => u.FullName);
        builder.Ignore(u => u.Initials);

        // Soft Delete QueryFilter
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Audit fields
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}


