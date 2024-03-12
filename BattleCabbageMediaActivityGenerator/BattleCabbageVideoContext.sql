IF SCHEMA_ID(N'app') IS NULL EXEC(N'CREATE SCHEMA [app];');
GO


CREATE TABLE [app].[Inventory] (
    [InventoryId] int NOT NULL IDENTITY,
    [ItemDescription] varchar(40) NOT NULL,
    [CurrentPrice] decimal(6,2) NOT NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY ([InventoryId])
);
GO


CREATE TABLE [app].[Kiosk] (
    [KioskId] int NOT NULL IDENTITY,
    [Address] nvarchar(250) NOT NULL,
    [State] nchar(2) NOT NULL,
    [ZipCode] nchar(10) NOT NULL,
    [InstallDate] datetime2 NULL,
    CONSTRAINT [PK_Kiosk] PRIMARY KEY ([KioskId])
);
GO


CREATE TABLE [app].[Users] (
    [UserId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PhoneNumber] nvarchar(15) NOT NULL,
    [MemberSince] datetime2 NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [ModifiedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [app].[Purchases] (
    [PurchaseId] int NOT NULL IDENTITY,
    [TransactionCreatedOn] datetime2 NOT NULL,
    [PurchasingUserId] int NOT NULL,
    [PurchaseLocationId] int NOT NULL,
    [PaymentCardId] int NOT NULL,
    CONSTRAINT [PK_Purchases] PRIMARY KEY ([PurchaseId]),
    CONSTRAINT [FK_Purchases_Kiosk] FOREIGN KEY ([PurchaseLocationId]) REFERENCES [app].[Kiosk] ([KioskId])
);
GO


CREATE TABLE [app].[UserAddresses] (
    [AddressId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Address] nvarchar(250) NOT NULL,
    [State] nchar(3) NOT NULL,
    [ZipCode] nvarchar(10) NOT NULL,
    [Default] bit NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [ModifiedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_UserAddresses] PRIMARY KEY ([AddressId]),
    CONSTRAINT [FK_UserAddresses_Users] FOREIGN KEY ([UserId]) REFERENCES [app].[Users] ([UserId])
);
GO


CREATE TABLE [app].[UserCreditCards] (
    [CreditCardId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CreditCardNumber] nvarchar(30) NOT NULL,
    [CreditCardExpiration] datetime2 NOT NULL,
    [Default] bit NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_UserCreditCards] PRIMARY KEY ([CreditCardId]),
    CONSTRAINT [FK_UserCreditCards_Users] FOREIGN KEY ([UserId]) REFERENCES [app].[Users] ([UserId])
);
GO


CREATE TABLE [app].[UserReviews] (
    [UserReviewId] int NOT NULL IDENTITY,
    [movie_id] int NOT NULL,
    [UserReviewScore] float NOT NULL,
    [UserReview] nvarchar(4000) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_UserReviews] PRIMARY KEY ([UserReviewId]),
    CONSTRAINT [FK_UserReviews_Users] FOREIGN KEY ([UserId]) REFERENCES [app].[Users] ([UserId]),
    CONSTRAINT [FK_UserReviews_movies] FOREIGN KEY ([movie_id]) REFERENCES [movies] ([movie_id])
);
GO


CREATE TABLE [app].[PurchaseLineItems] (
    [PurchaseLineItemId] int NOT NULL IDENTITY,
    [ItemId] int NOT NULL,
    [Quantity] int NOT NULL,
    [TotalPrice] decimal(7,2) NULL,
    [PurchaseId] int NOT NULL,
    CONSTRAINT [PK_PurchaseLineItems] PRIMARY KEY ([PurchaseLineItemId]),
    CONSTRAINT [FK_PurchaseLineItems_Inventory] FOREIGN KEY ([ItemId]) REFERENCES [app].[Inventory] ([InventoryId]),
    CONSTRAINT [FK_PurchaseLineItems_Purchases] FOREIGN KEY ([PurchaseId]) REFERENCES [app].[Purchases] ([PurchaseId])
);
GO


CREATE TABLE [app].[PurchaseUser] (
    [PurchasesPurchaseId] int NOT NULL,
    [PurchasingUsersUserId] int NOT NULL,
    CONSTRAINT [PK_PurchaseUser] PRIMARY KEY ([PurchasesPurchaseId], [PurchasingUsersUserId]),
    CONSTRAINT [FK_PurchaseUser_Purchases_PurchasesPurchaseId] FOREIGN KEY ([PurchasesPurchaseId]) REFERENCES [app].[Purchases] ([PurchaseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseUser_Users_PurchasingUsersUserId] FOREIGN KEY ([PurchasingUsersUserId]) REFERENCES [app].[Users] ([UserId]) ON DELETE CASCADE
);
GO


CREATE TABLE [app].[PurchaseUserCreditCard] (
    [PaymentCardsCreditCardId] int NOT NULL,
    [PurchasesPurchaseId] int NOT NULL,
    CONSTRAINT [PK_PurchaseUserCreditCard] PRIMARY KEY ([PaymentCardsCreditCardId], [PurchasesPurchaseId]),
    CONSTRAINT [FK_PurchaseUserCreditCard_Purchases_PurchasesPurchaseId] FOREIGN KEY ([PurchasesPurchaseId]) REFERENCES [app].[Purchases] ([PurchaseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseUserCreditCard_UserCreditCards_PaymentCardsCreditCardId] FOREIGN KEY ([PaymentCardsCreditCardId]) REFERENCES [app].[UserCreditCards] ([CreditCardId]) ON DELETE CASCADE
);
GO


CREATE TABLE [app].[Rentals] (
    [RentalId] int NOT NULL IDENTITY,
    [MovieId] int NOT NULL,
    [UserId] int NOT NULL,
    [RentalDate] datetime2 NOT NULL,
    [ExpectedReturnDate] datetime2 NOT NULL,
    [PurchaseLineItemId] int NOT NULL,
    CONSTRAINT [PK_Rentals] PRIMARY KEY ([RentalId]),
    CONSTRAINT [FK_Rentals_PurchaseLineItems_PurchaseLineItemId] FOREIGN KEY ([PurchaseLineItemId]) REFERENCES [app].[PurchaseLineItems] ([PurchaseLineItemId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Rentals_Users] FOREIGN KEY ([UserId]) REFERENCES [app].[Users] ([UserId]),
    CONSTRAINT [FK_Rentals_movies] FOREIGN KEY ([MovieId]) REFERENCES [movies] ([movie_id])
);
GO


CREATE TABLE [app].[UserSubscriptionStatus] (
    [UserSubscriptionStatusId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [RenewalDay] int NOT NULL,
    [Active] bit NOT NULL,
    [MostRecentSubscriptionPurchaseId] int NULL,
    CONSTRAINT [PK_UserSubscriptionStatus] PRIMARY KEY ([UserSubscriptionStatusId]),
    CONSTRAINT [FK_UserSubscriptionStatus_PurchaseLineItems_MostRecentSubscriptionPurchaseId] FOREIGN KEY ([MostRecentSubscriptionPurchaseId]) REFERENCES [app].[PurchaseLineItems] ([PurchaseLineItemId]),
    CONSTRAINT [FK_UserSubscriptionStatus_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [app].[Users] ([UserId]) ON DELETE CASCADE
);
GO


CREATE TABLE [app].[Returns] (
    [ReturnId] int NOT NULL IDENTITY,
    [RentalId] int NOT NULL,
    [ReturnDate] datetime2 NOT NULL,
    [LateDays] int NOT NULL,
    [LateChargeLineItemId] int NULL,
    CONSTRAINT [PK_Returns] PRIMARY KEY ([ReturnId]),
    CONSTRAINT [FK_Returns_PurchaseLineItems_LateChargeLineItemId] FOREIGN KEY ([LateChargeLineItemId]) REFERENCES [app].[PurchaseLineItems] ([PurchaseLineItemId]),
    CONSTRAINT [FK_Returns_Rentals] FOREIGN KEY ([RentalId]) REFERENCES [app].[Rentals] ([RentalId])
);
GO


CREATE INDEX [IX_PurchaseLineItems_ItemId] ON [app].[PurchaseLineItems] ([ItemId]);
GO


CREATE INDEX [IX_PurchaseLineItems_PurchaseId] ON [app].[PurchaseLineItems] ([PurchaseId]);
GO


CREATE INDEX [IX_Purchases_PurchaseLocationId] ON [app].[Purchases] ([PurchaseLocationId]);
GO


CREATE INDEX [IX_PurchaseUser_PurchasingUsersUserId] ON [app].[PurchaseUser] ([PurchasingUsersUserId]);
GO


CREATE INDEX [IX_PurchaseUserCreditCard_PurchasesPurchaseId] ON [app].[PurchaseUserCreditCard] ([PurchasesPurchaseId]);
GO


CREATE INDEX [IX_Rentals_MovieId] ON [app].[Rentals] ([MovieId]);
GO


CREATE UNIQUE INDEX [IX_Rentals_PurchaseLineItemId] ON [app].[Rentals] ([PurchaseLineItemId]);
GO


CREATE INDEX [IX_Rentals_UserId] ON [app].[Rentals] ([UserId]);
GO


CREATE UNIQUE INDEX [IX_Returns_LateChargeLineItemId] ON [app].[Returns] ([LateChargeLineItemId]) WHERE [LateChargeLineItemId] IS NOT NULL;
GO


CREATE UNIQUE INDEX [IX_Returns_RentalId] ON [app].[Returns] ([RentalId]);
GO


CREATE INDEX [IX_UserAddresses_UserId] ON [app].[UserAddresses] ([UserId]);
GO


CREATE INDEX [IX_UserCreditCards_UserId] ON [app].[UserCreditCards] ([UserId]);
GO


CREATE INDEX [IX_UserReviews_movie_id] ON [app].[UserReviews] ([movie_id]);
GO


CREATE INDEX [IX_UserReviews_UserId] ON [app].[UserReviews] ([UserId]);
GO


CREATE UNIQUE INDEX [IX_UserSubscriptionStatus_MostRecentSubscriptionPurchaseId] ON [app].[UserSubscriptionStatus] ([MostRecentSubscriptionPurchaseId]) WHERE [MostRecentSubscriptionPurchaseId] IS NOT NULL;
GO


CREATE UNIQUE INDEX [IX_UserSubscriptionStatus_UserId] ON [app].[UserSubscriptionStatus] ([UserId]);
GO