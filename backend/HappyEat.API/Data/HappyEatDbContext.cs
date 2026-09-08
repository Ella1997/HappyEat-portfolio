using System;
using System.Collections.Generic;
using HappyEat.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyEat.API.Data;

public partial class HappyEatDbContext : DbContext
{
    public HappyEatDbContext()
    {
    }

    public HappyEatDbContext(DbContextOptions<HappyEatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BodyRecord> BodyRecords { get; set; }

    public virtual DbSet<Drink> Drinks { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodImage> FoodImages { get; set; }

    public virtual DbSet<FoodRecord> FoodRecords { get; set; }

    public virtual DbSet<FoodRecordItem> FoodRecordItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGoal> UserGoals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BodyRecord>(entity =>
        {
            entity.Property(e => e.BodyRecordId).HasColumnName("BodyRecordID");
            entity.Property(e => e.ActivityLevel).HasColumnType("decimal(4, 3)");
            entity.Property(e => e.BodyFat).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.HipSize).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MuscleMass).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.NeckSize).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VisceralFat).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.WaistSize).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.BodyRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BodyRecords_Users");
        });

        modelBuilder.Entity<Drink>(entity =>
        {
            entity.ToTable("Drink");

            entity.Property(e => e.DrinkId).HasColumnName("DrinkID");
            entity.Property(e => e.Calories).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Carbs).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DrinkCategory).HasMaxLength(50);
            entity.Property(e => e.DrinkName).HasMaxLength(50);
            entity.Property(e => e.Fat).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Protein).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Size).HasMaxLength(10);
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.ToTable("Food");

            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.Calories).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Carbs).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Fat).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FoodCategory).HasMaxLength(50);
            entity.Property(e => e.FoodName).HasMaxLength(50);
            entity.Property(e => e.Protein).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<FoodImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.ToTable("FoodImage");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.UploadTime).HasDefaultValueSql("(getdate())", "DF_FoodImage_UploadTime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.FoodImages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoodImage_Users");
        });

        modelBuilder.Entity<FoodRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.HasIndex(e => e.ImageId, "UX_FoodRecords_ImageId")
                .IsUnique()
                .HasFilter("([ImageId] IS NOT NULL)");

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.MealType).HasMaxLength(50);
            entity.Property(e => e.RecordSource).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Image).WithOne(p => p.FoodRecord)
                .HasForeignKey<FoodRecord>(d => d.ImageId)
                .HasConstraintName("FK_FoodRecords_FoodImage");

            entity.HasOne(d => d.User).WithMany(p => p.FoodRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FoodRecords_Users");
        });

        modelBuilder.Entity<FoodRecordItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Calories).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Carbs).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DrinkId).HasColumnName("DrinkID");
            entity.Property(e => e.Fat).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FoodId).HasColumnName("FoodID");
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.Protein).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.Unit).HasMaxLength(20);

            entity.HasOne(d => d.Drink).WithMany(p => p.FoodRecordItems)
                .HasForeignKey(d => d.DrinkId)
                .HasConstraintName("FK_FoodRecordItems_Drink");

            entity.HasOne(d => d.Food).WithMany(p => p.FoodRecordItems)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK_FoodRecordItems_Food");

            entity.HasOne(d => d.Record).WithMany(p => p.FoodRecordItems)
                .HasForeignKey(d => d.RecordId)
                .HasConstraintName("FK_FoodRecordItems_FoodRecords");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(e => e.GoalId);

            entity.Property(e => e.GoalId).HasColumnName("GoalID");
            entity.Property(e => e.GoalType).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_UserGoals_IsActive");
            entity.Property(e => e.TargetBodyFat).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TargetWeight).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserGoals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserGoals_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
