using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BattleCabbageMediaActivityGenerator.Migrations
{
    /// <inheritdoc />
    public partial class InitialSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "Inventory",
                schema: "app",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemDescription = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                });

            migrationBuilder.CreateTable(
                name: "Kiosk",
                schema: "app",
                columns: table => new
                {
                    KioskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    State = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    InstallDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kiosk", x => x.KioskId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MemberSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "app",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasingUserId = table.Column<int>(type: "int", nullable: false),
                    PurchaseLocationId = table.Column<int>(type: "int", nullable: false),
                    PaymentCardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_Kiosk",
                        column: x => x.PurchaseLocationId,
                        principalSchema: "app",
                        principalTable: "Kiosk",
                        principalColumn: "KioskId");
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "app",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    State = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserCreditCards",
                schema: "app",
                columns: table => new
                {
                    CreditCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreditCardNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreditCardExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreditCards", x => x.CreditCardId);
                    table.ForeignKey(
                        name: "FK_UserCreditCards_Users",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserReviews",
                schema: "app",
                columns: table => new
                {
                    UserReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_id = table.Column<int>(type: "int", nullable: false),
                    UserReviewScore = table.Column<double>(type: "float", nullable: false),
                    UserReview = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.UserReviewId);
                    table.ForeignKey(
                        name: "FK_UserReviews_Users",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserReviews_movies",
                        column: x => x.movie_id,
                        principalTable: "movies",
                        principalColumn: "movie_id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseLineItems",
                schema: "app",
                columns: table => new
                {
                    PurchaseLineItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(7,2)", nullable: true),
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseLineItems", x => x.PurchaseLineItemId);
                    table.ForeignKey(
                        name: "FK_PurchaseLineItems_Inventory",
                        column: x => x.ItemId,
                        principalSchema: "app",
                        principalTable: "Inventory",
                        principalColumn: "InventoryId");
                    table.ForeignKey(
                        name: "FK_PurchaseLineItems_Purchases",
                        column: x => x.PurchaseId,
                        principalSchema: "app",
                        principalTable: "Purchases",
                        principalColumn: "PurchaseId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseUser",
                schema: "app",
                columns: table => new
                {
                    PurchasesPurchaseId = table.Column<int>(type: "int", nullable: false),
                    PurchasingUsersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseUser", x => new { x.PurchasesPurchaseId, x.PurchasingUsersUserId });
                    table.ForeignKey(
                        name: "FK_PurchaseUser_Purchases_PurchasesPurchaseId",
                        column: x => x.PurchasesPurchaseId,
                        principalSchema: "app",
                        principalTable: "Purchases",
                        principalColumn: "PurchaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseUser_Users_PurchasingUsersUserId",
                        column: x => x.PurchasingUsersUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseUserCreditCard",
                schema: "app",
                columns: table => new
                {
                    PaymentCardsCreditCardId = table.Column<int>(type: "int", nullable: false),
                    PurchasesPurchaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseUserCreditCard", x => new { x.PaymentCardsCreditCardId, x.PurchasesPurchaseId });
                    table.ForeignKey(
                        name: "FK_PurchaseUserCreditCard_Purchases_PurchasesPurchaseId",
                        column: x => x.PurchasesPurchaseId,
                        principalSchema: "app",
                        principalTable: "Purchases",
                        principalColumn: "PurchaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseUserCreditCard_UserCreditCards_PaymentCardsCreditCardId",
                        column: x => x.PaymentCardsCreditCardId,
                        principalSchema: "app",
                        principalTable: "UserCreditCards",
                        principalColumn: "CreditCardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                schema: "app",
                columns: table => new
                {
                    RentalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RentalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseLineItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.RentalId);
                    table.ForeignKey(
                        name: "FK_Rentals_PurchaseLineItems_PurchaseLineItemId",
                        column: x => x.PurchaseLineItemId,
                        principalSchema: "app",
                        principalTable: "PurchaseLineItems",
                        principalColumn: "PurchaseLineItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Users",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Rentals_movies",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "movie_id");
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptionStatus",
                schema: "app",
                columns: table => new
                {
                    UserSubscriptionStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RenewalDay = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MostRecentSubscriptionPurchaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptionStatus", x => x.UserSubscriptionStatusId);
                    table.ForeignKey(
                        name: "FK_UserSubscriptionStatus_PurchaseLineItems_MostRecentSubscriptionPurchaseId",
                        column: x => x.MostRecentSubscriptionPurchaseId,
                        principalSchema: "app",
                        principalTable: "PurchaseLineItems",
                        principalColumn: "PurchaseLineItemId");
                    table.ForeignKey(
                        name: "FK_UserSubscriptionStatus_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                schema: "app",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LateDays = table.Column<int>(type: "int", nullable: false),
                    LateChargeLineItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_Returns_PurchaseLineItems_LateChargeLineItemId",
                        column: x => x.LateChargeLineItemId,
                        principalSchema: "app",
                        principalTable: "PurchaseLineItems",
                        principalColumn: "PurchaseLineItemId");
                    table.ForeignKey(
                        name: "FK_Returns_Rentals",
                        column: x => x.RentalId,
                        principalSchema: "app",
                        principalTable: "Rentals",
                        principalColumn: "RentalId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseLineItems_ItemId",
                schema: "app",
                table: "PurchaseLineItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseLineItems_PurchaseId",
                schema: "app",
                table: "PurchaseLineItems",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseLocationId",
                schema: "app",
                table: "Purchases",
                column: "PurchaseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseUser_PurchasingUsersUserId",
                schema: "app",
                table: "PurchaseUser",
                column: "PurchasingUsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseUserCreditCard_PurchasesPurchaseId",
                schema: "app",
                table: "PurchaseUserCreditCard",
                column: "PurchasesPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_MovieId",
                schema: "app",
                table: "Rentals",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_PurchaseLineItemId",
                schema: "app",
                table: "Rentals",
                column: "PurchaseLineItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_UserId",
                schema: "app",
                table: "Rentals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_LateChargeLineItemId",
                schema: "app",
                table: "Returns",
                column: "LateChargeLineItemId",
                unique: true,
                filter: "[LateChargeLineItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_RentalId",
                schema: "app",
                table: "Returns",
                column: "RentalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "app",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCreditCards_UserId",
                schema: "app",
                table: "UserCreditCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_movie_id",
                schema: "app",
                table: "UserReviews",
                column: "movie_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId",
                schema: "app",
                table: "UserReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionStatus_MostRecentSubscriptionPurchaseId",
                schema: "app",
                table: "UserSubscriptionStatus",
                column: "MostRecentSubscriptionPurchaseId",
                unique: true,
                filter: "[MostRecentSubscriptionPurchaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionStatus_UserId",
                schema: "app",
                table: "UserSubscriptionStatus",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseUser",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseUserCreditCard",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Returns",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserReviews",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserSubscriptionStatus",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserCreditCards",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Rentals",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PurchaseLineItems",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Inventory",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Purchases",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Kiosk",
                schema: "app");
        }
    }
}
