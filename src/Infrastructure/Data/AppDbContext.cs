using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft Delete: queries automáticas já filtram registros deletados
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);
        
        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);
        
        // Proteção de integridade: impede deletar categoria com produtos vinculados
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    // Intercepta SaveChanges para gerenciar timestamps e soft delete automaticamente
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Domain.Common.Entity>();

        foreach (var entry in entries)
        {
            // Novo registro
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.IsDeleted = false;
            }

            // Atualização
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            // Soft Delete: transforma DELETE em UPDATE
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
            
                entry.Entity.IsDeleted = true;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}