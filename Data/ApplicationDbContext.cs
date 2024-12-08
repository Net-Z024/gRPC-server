using GrpcService1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<UserProduct> UserProducts { get; set; }
    public virtual DbSet<Chest> Chests { get; set; }
    public virtual DbSet<ChestProduct> ChestProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserProducts)
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserProduct>()
            .HasOne(up => up.Product)
            .WithMany(p => p.UserProducts)
            .HasForeignKey(up => up.ProductId);

        modelBuilder.Entity<ChestProduct>()
            .HasOne(cp => cp.Chest)
            .WithMany(c => c.PossibleProducts)
            .HasForeignKey(cp => cp.ChestId);

        modelBuilder.Entity<ChestProduct>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.ChestProducts)
            .HasForeignKey(cp => cp.ProductId);

        // Configure decimal properties
        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>()
            .Property(p => p.Value)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Chest>()
            .Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ChestProduct>()
            .Property(cp => cp.DropChance)
            .HasColumnType("decimal(5,4)");
    }
}