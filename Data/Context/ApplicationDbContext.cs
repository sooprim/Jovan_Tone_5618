using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<StockImport> StockImports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Name)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<StockImport>(entity =>
        {
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");
            
            entity.Property(e => e.Categories)
                .HasMaxLength(1000);

            entity.Property(e => e.Status)
                .HasMaxLength(100);

            entity.Property(e => e.ProductName)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);
        });
    }
} 