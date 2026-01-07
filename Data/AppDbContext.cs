using Microsoft.EntityFrameworkCore;
using BusinessManagement.Api.Entities;

namespace BusinessManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryBatch> InventoryBatches { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Users table setup
        builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasConversion<string>(); // Keep this!
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Products table
        builder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(255);
            e.Property(x => x.Price);
        });

        // Inventory batches
        builder.Entity<InventoryBatch>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Product)
              .WithMany(p => p.InventoryBatches)
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // Shifts
        builder.Entity<Shift>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.StartedAt).IsRequired();
            e.Property(x => x.EndedAt);
            e.Property(x => x.IsActive).IsRequired();
        });

        // Sales
        builder.Entity<Sale>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TotalAmount);
        });

        // Sale items
        builder.Entity<SaleItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitPrice);
            e.HasOne(x => x.Sale)
              .WithMany(s => s.SaleItems)
              .HasForeignKey(x => x.SaleId)
              .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
              .WithMany(p => p.SaleItems)
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // Add some test data
        AddSeedData(builder);
    }

    private void AddSeedData(ModelBuilder builder)
    {
        // ID values
        var popcornGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var sodaGuid = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var nachosGuid = Guid.Parse("44444444-4444-4444-4444-444444444444");
        
        var popcornBatchGuid = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var sodaBatchGuid = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var nachosBatchGuid = Guid.Parse("77777777-7777-7777-7777-777777777777");

        // Create products
        var popcornItem = new Product
        {
            Id = popcornGuid,
            Name = "Popcorn",
            Price = 5.99m,
            IsActive = true
        };

        var sodaItem = new Product
        {
            Id = sodaGuid,
            Name = "Soda",
            Price = 3.49m,
            IsActive = true
        };

        var nachosItem = new Product
        {
            Id = nachosGuid,
            Name = "Nachos",
            Price = 6.99m,
            IsActive = true
        };

        // Create inventory
        var popcornInv = new InventoryBatch
        {
            Id = popcornBatchGuid,
            ProductId = popcornGuid,
            Quantity = 100,
            ExpirationDate = new DateTime(2024, 7, 1),
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var sodaInv = new InventoryBatch
        {
            Id = sodaBatchGuid,
            ProductId = sodaGuid,
            Quantity = 150,
            ExpirationDate = new DateTime(2025, 1, 1),
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var nachosInv = new InventoryBatch
        {
            Id = nachosBatchGuid,
            ProductId = nachosGuid,
            Quantity = 80,
            ExpirationDate = new DateTime(2024, 4, 1),
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Add to db
        builder.Entity<Product>().HasData(popcornItem, sodaItem, nachosItem);
        builder.Entity<InventoryBatch>().HasData(popcornInv, sodaInv, nachosInv);
    }
}