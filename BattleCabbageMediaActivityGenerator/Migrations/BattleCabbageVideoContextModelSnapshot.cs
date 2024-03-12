﻿// <auto-generated />
using System;
using BattleCabbageMediaActivityGenerator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BattleCabbageMediaActivityGenerator.Migrations
{
    [DbContext(typeof(BattleCabbageVideoContext))]
    partial class BattleCabbageVideoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Inventory", b =>
                {
                    b.Property<int>("InventoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InventoryId"));

                    b.Property<decimal>("CurrentPrice")
                        .HasColumnType("decimal(6, 2)");

                    b.Property<string>("ItemDescription")
                        .IsRequired()
                        .HasMaxLength(40)
                        .IsUnicode(false)
                        .HasColumnType("varchar(40)");

                    b.HasKey("InventoryId");

                    b.ToTable("Inventory", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Kiosk", b =>
                {
                    b.Property<int>("KioskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("KioskId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime?>("InstallDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nchar(2)")
                        .IsFixedLength();

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nchar(10)")
                        .IsFixedLength();

                    b.HasKey("KioskId");

                    b.ToTable("Kiosk", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("movie_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MovieId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)")
                        .HasColumnName("description");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasColumnName("external_id");

                    b.Property<int>("GenreId")
                        .HasColumnType("int")
                        .HasColumnName("genre_id");

                    b.Property<string>("MpaaRating")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)")
                        .HasColumnName("mpaa_rating");

                    b.Property<double>("PopularityScore")
                        .HasColumnType("float")
                        .HasColumnName("popularity_score");

                    b.Property<string>("PosterUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("poster_url");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("date")
                        .HasColumnName("release_date");

                    b.Property<string>("Tagline")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("tagline");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("title");

                    b.HasKey("MovieId");

                    b.ToTable("movies", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<int>("PaymentCardId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseLocationId")
                        .HasColumnType("int");

                    b.Property<int>("PurchasingUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionCreatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("PurchaseId");

                    b.HasIndex("PurchaseLocationId");

                    b.ToTable("Purchases", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", b =>
                {
                    b.Property<int>("PurchaseLineItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseLineItemId"));

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(7, 2)");

                    b.HasKey("PurchaseLineItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("PurchaseId");

                    b.ToTable("PurchaseLineItems", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Rental", b =>
                {
                    b.Property<int>("RentalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RentalId"));

                    b.Property<DateTime>("ExpectedReturnDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseLineItemId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RentalDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("RentalId");

                    b.HasIndex("MovieId");

                    b.HasIndex("PurchaseLineItemId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Rentals", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Return", b =>
                {
                    b.Property<int>("ReturnId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReturnId"));

                    b.Property<int?>("LateChargeLineItemId")
                        .HasColumnType("int");

                    b.Property<int>("LateDays")
                        .HasColumnType("int");

                    b.Property<int>("RentalId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ReturnId");

                    b.HasIndex("LateChargeLineItemId")
                        .IsUnique()
                        .HasFilter("[LateChargeLineItemId] IS NOT NULL");

                    b.HasIndex("RentalId")
                        .IsUnique();

                    b.ToTable("Returns", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("UserId");

                    b.ToTable("Users", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserAddress", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nchar(3)")
                        .IsFixedLength();

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAddresses", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserCreditCard", b =>
                {
                    b.Property<int>("CreditCardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CreditCardId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreditCardExpiration")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreditCardNumber")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CreditCardId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCreditCards", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserReview", b =>
                {
                    b.Property<int>("UserReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserReviewId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("MovieId")
                        .HasColumnType("int")
                        .HasColumnName("movie_id");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserReview1")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasColumnName("UserReview");

                    b.Property<double>("UserReviewScore")
                        .HasColumnType("float");

                    b.HasKey("UserReviewId");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("UserReviews", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserSubscriptionStatus", b =>
                {
                    b.Property<int>("UserSubscriptionStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserSubscriptionStatusId"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("MostRecentSubscriptionPurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("RenewalDay")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserSubscriptionStatusId");

                    b.HasIndex("MostRecentSubscriptionPurchaseId")
                        .IsUnique()
                        .HasFilter("[MostRecentSubscriptionPurchaseId] IS NOT NULL");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSubscriptionStatus", "app");
                });

            modelBuilder.Entity("PurchaseUser", b =>
                {
                    b.Property<int>("PurchasesPurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("PurchasingUsersUserId")
                        .HasColumnType("int");

                    b.HasKey("PurchasesPurchaseId", "PurchasingUsersUserId");

                    b.HasIndex("PurchasingUsersUserId");

                    b.ToTable("PurchaseUser", "app");
                });

            modelBuilder.Entity("PurchaseUserCreditCard", b =>
                {
                    b.Property<int>("PaymentCardsCreditCardId")
                        .HasColumnType("int");

                    b.Property<int>("PurchasesPurchaseId")
                        .HasColumnType("int");

                    b.HasKey("PaymentCardsCreditCardId", "PurchasesPurchaseId");

                    b.HasIndex("PurchasesPurchaseId");

                    b.ToTable("PurchaseUserCreditCard", "app");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Purchase", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Kiosk", "PurchaseLocation")
                        .WithMany("Purchases")
                        .HasForeignKey("PurchaseLocationId")
                        .IsRequired()
                        .HasConstraintName("FK_Purchases_Kiosk");

                    b.Navigation("PurchaseLocation");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Inventory", "Item")
                        .WithMany("PurchaseLineItems")
                        .HasForeignKey("ItemId")
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseLineItems_Inventory");

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Purchase", "Purchase")
                        .WithMany("PurchaseLineItems")
                        .HasForeignKey("PurchaseId")
                        .IsRequired()
                        .HasConstraintName("FK_PurchaseLineItems_Purchases");

                    b.Navigation("Item");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Rental", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Movie", "Movie")
                        .WithMany("Rentals")
                        .HasForeignKey("MovieId")
                        .IsRequired()
                        .HasConstraintName("FK_Rentals_movies");

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", "PurchaseLineItem")
                        .WithOne("Rental")
                        .HasForeignKey("BattleCabbageMediaActivityGenerator.Models.Rental", "PurchaseLineItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", "User")
                        .WithMany("Rentals")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Rentals_Users");

                    b.Navigation("Movie");

                    b.Navigation("PurchaseLineItem");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Return", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", "LateChargeLineItem")
                        .WithOne("Return")
                        .HasForeignKey("BattleCabbageMediaActivityGenerator.Models.Return", "LateChargeLineItemId");

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Rental", "Rental")
                        .WithOne("Return")
                        .HasForeignKey("BattleCabbageMediaActivityGenerator.Models.Return", "RentalId")
                        .IsRequired()
                        .HasConstraintName("FK_Returns_Rentals");

                    b.Navigation("LateChargeLineItem");

                    b.Navigation("Rental");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserAddress", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", "User")
                        .WithMany("UserAddresses")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserAddresses_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserCreditCard", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", "User")
                        .WithMany("UserCreditCards")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserCreditCards_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserReview", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Movie", "Movie")
                        .WithMany("UserReviews")
                        .HasForeignKey("MovieId")
                        .IsRequired()
                        .HasConstraintName("FK_UserReviews_movies");

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", "User")
                        .WithMany("UserReviews")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserReviews_Users");

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.UserSubscriptionStatus", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", "MostRecentSubscriptionPurchase")
                        .WithOne("UserSubscriptionStatus")
                        .HasForeignKey("BattleCabbageMediaActivityGenerator.Models.UserSubscriptionStatus", "MostRecentSubscriptionPurchaseId");

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", "User")
                        .WithOne("UserSubscriptionStatus")
                        .HasForeignKey("BattleCabbageMediaActivityGenerator.Models.UserSubscriptionStatus", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MostRecentSubscriptionPurchase");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PurchaseUser", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Purchase", null)
                        .WithMany()
                        .HasForeignKey("PurchasesPurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.User", null)
                        .WithMany()
                        .HasForeignKey("PurchasingUsersUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PurchaseUserCreditCard", b =>
                {
                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.UserCreditCard", null)
                        .WithMany()
                        .HasForeignKey("PaymentCardsCreditCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BattleCabbageMediaActivityGenerator.Models.Purchase", null)
                        .WithMany()
                        .HasForeignKey("PurchasesPurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Inventory", b =>
                {
                    b.Navigation("PurchaseLineItems");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Kiosk", b =>
                {
                    b.Navigation("Purchases");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Movie", b =>
                {
                    b.Navigation("Rentals");

                    b.Navigation("UserReviews");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Purchase", b =>
                {
                    b.Navigation("PurchaseLineItems");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.PurchaseLineItem", b =>
                {
                    b.Navigation("Rental");

                    b.Navigation("Return");

                    b.Navigation("UserSubscriptionStatus");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.Rental", b =>
                {
                    b.Navigation("Return");
                });

            modelBuilder.Entity("BattleCabbageMediaActivityGenerator.Models.User", b =>
                {
                    b.Navigation("Rentals");

                    b.Navigation("UserAddresses");

                    b.Navigation("UserCreditCards");

                    b.Navigation("UserReviews");

                    b.Navigation("UserSubscriptionStatus")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
