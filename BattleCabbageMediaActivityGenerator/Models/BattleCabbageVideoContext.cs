using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class BattleCabbageVideoContext : DbContext
{
    public BattleCabbageVideoContext()
    {
    }

    public BattleCabbageVideoContext(DbContextOptions<BattleCabbageVideoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Kiosk> Kiosks { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<PurchaseLineItem> PurchaseLineItems { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Return> Returns { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserCreditCard> UserCreditCards { get; set; }

    public virtual DbSet<UserReview> UserReviews { get; set; }

    public virtual DbSet<UserSubscriptionStatus> UserSubscriptionStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory", "app");

            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Kiosk>(entity =>
        {
            entity.ToTable("Kiosk", "app");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies", t => t.ExcludeFromMigrations());

            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .HasColumnName("description");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(30)
                .HasColumnName("external_id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.MpaaRating)
                .HasMaxLength(5)
                .HasColumnName("mpaa_rating");
            entity.Property(e => e.PopularityScore).HasColumnName("popularity_score");
            entity.Property(e => e.PosterUrl)
                .HasMaxLength(500)
                .HasColumnName("poster_url");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Tagline)
                .HasMaxLength(500)
                .HasColumnName("tagline");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Purchases", "app");

            entity.HasMany(d => d.PaymentCards).WithMany(p => p.Purchases);

            entity.HasOne(d => d.PurchaseLocation).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.PurchaseLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Kiosk");

            entity.HasMany(d => d.PurchasingUsers).WithMany(p => p.Purchases);
        });

        modelBuilder.Entity<PurchaseLineItem>(entity =>
        {
            entity.ToTable("PurchaseLineItems", "app");

            entity.Property(e => e.TotalPrice).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.PurchaseLineItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseLineItems_Inventory");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseLineItems)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseLineItems_Purchases");

            entity.HasOne(d => d.Rental).WithOne(p => p.PurchaseLineItem).HasForeignKey<PurchaseLineItem>("PurchaseLineItemId");

            entity.HasOne(d => d.Return).WithOne(p => p.LateChargeLineItem).HasForeignKey<PurchaseLineItem>("PurchaseLineItemId");

            entity.HasOne(d => d.UserSubscriptionStatus).WithOne(p => p.MostRecentSubscriptionPurchase).HasForeignKey<PurchaseLineItem>("PurchaseLineItemId");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.ToTable("Rentals", "app");

            entity.HasOne(d => d.Movie).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_movies");

            entity.HasOne(d => d.User).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_Users");

            entity.HasOne(entity => entity.Return).WithOne(entity => entity.Rental)
                .HasForeignKey<Rental>(entity => entity.RentalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Returns_Rentals");

            entity.HasOne(d => d.PurchaseLineItem).WithOne(p => p.Rental).HasForeignKey<Rental>("PurchaseLineItemId");
        });

        modelBuilder.Entity<Return>(entity =>
        {
            entity.ToTable("Returns", "app");

            entity.HasOne(d => d.Rental).WithOne(p => p.Return)
                .HasForeignKey<Return>("RentalId")
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Returns_Rentals");

            entity.HasOne(d => d.LateChargeLineItem).WithOne(p => p.Return).HasForeignKey<Return>("LateChargeLineItemId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "app");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);

            entity.HasMany(p => p.Purchases).WithMany(u => u.PurchasingUsers);

            //entity.HasOne(d => d.UserSubscriptionStatus).WithOne(p => p.User).HasForeignKey<User>("UserSubscriptionStatus");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("UserAddresses", "app");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.State)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.ZipCode).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAddresses_Users");
        });

        modelBuilder.Entity<UserCreditCard>(entity =>
        {
            entity.HasKey(e => e.CreditCardId);

            entity.ToTable("UserCreditCards", "app");

            entity.Property(e => e.CreditCardNumber).HasMaxLength(30);

            entity.HasOne(d => d.User).WithMany(p => p.UserCreditCards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserCreditCards_Users");

            entity.HasMany(p => p.Purchases).WithMany(pc => pc.PaymentCards);
        });

        modelBuilder.Entity<UserReview>(entity =>
        {
            entity.ToTable("UserReviews", "app");

            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.UserReview1)
                .HasMaxLength(4000)
                .HasColumnName("UserReview");

            entity.HasOne(d => d.Movie).WithMany(p => p.UserReviews)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserReviews_movies");

            entity.HasOne(d => d.User).WithMany(p => p.UserReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserReviews_Users");
        });

        modelBuilder.Entity<UserSubscriptionStatus>(entity =>
        {
            entity.ToTable("UserSubscriptionStatus", "app");

            entity.HasOne(d => d.MostRecentSubscriptionPurchase).WithOne(p => p.UserSubscriptionStatus)
                .HasForeignKey<UserSubscriptionStatus>("MostRecentSubscriptionPurchaseId");

            entity.HasOne(d => d.User).WithOne(p => p.UserSubscriptionStatus)
                .HasForeignKey<UserSubscriptionStatus>("UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
