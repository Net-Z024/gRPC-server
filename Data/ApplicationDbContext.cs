﻿using GrpcService1.Models;
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
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<UserItem> UserItems { get; set; }
    public virtual DbSet<Chest?> Chests { get; set; }
    public virtual DbSet<ChestItem> ChestItems { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GamePlayer> GamePlayers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id); // Primary key

            entity.HasOne(g => g.host) // Relationship to the User hosting the game
                .WithMany()
                .HasForeignKey(g => g.hostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.chest) // Relationship to the Chest being used in the game
                .WithMany()
                .HasForeignKey(g => g.caseId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<GamePlayer>(entity =>
        {
            entity.HasKey(gp => gp.Id); // Primary key

            entity.HasOne(gp => gp.game) // Relationship to the User hosting the game
                .WithMany(g=> g.GamePlayers)
                .HasForeignKey(gp => gp.gameId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(gp => gp.user) // Relationship to the User
            .WithMany()
            .HasForeignKey(gp => gp.userId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.spinResultItem) // Relationship to the Chest being used in the game
                .WithMany()
                .HasForeignKey(g => g.spinResult)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<UserItem>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserItems)
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserItem>()
            .HasOne(up => up.Item)
            .WithMany(p => p.UserItems)
            .HasForeignKey(up => up.ItemId);

        modelBuilder.Entity<ChestItem>()
            .HasOne(cp => cp.Chest)
            .WithMany(c => c.PossibleItems)
            .HasForeignKey(cp => cp.ChestId);

        modelBuilder.Entity<ChestItem>()
            .HasOne(cp => cp.Item)
            .WithMany(p => p.ChestItems)
            .HasForeignKey(cp => cp.ItemId);

        // Configure decimal properties
        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasColumnType("decimal(18,2)");
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id); // Primary key
        
            entity.HasOne(u => u.IdentityUser)
                .WithMany() // No navigation property in IdentityUser
                .HasForeignKey(u => u.IdentityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Item>()
            .Property(p => p.Value)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Chest>()
            .Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ChestItem>()
            .Property(cp => cp.DropChance)
            .HasColumnType("decimal(5,4)");
    }
}