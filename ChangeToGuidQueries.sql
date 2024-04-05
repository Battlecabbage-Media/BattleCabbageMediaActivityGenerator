ALTER TABLE app.Inventory ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.Kiosk ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.PurchaseLineItems ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.Purchases ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.Rentals  ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.Returns ADD Id uniqueidentifier NOT NULL default newid();

ALTER TABLE app.UserAddresses ADD Id uniqueidentifier NOT NULL default newid();
GO
ALTER TABLE app.UserCreditCards ADD Id uniqueidentifier NOT NULL default newid();
GO
ALTER TABLE app.UserReviews ADD Id uniqueidentifier NOT NULL default newid();
GO
ALTER TABLE app.Users ADD Id uniqueidentifier NOT NULL default newid();
GO
ALTER TABLE app.UserSubscriptionStatus ADD Id uniqueidentifier NOT NULL default newid();
GO

SELECT
    tc.table_schema, 
    tc.constraint_name, 
    tc.table_name, 
    kcu.column_name, 
    ccu.table_schema AS foreign_table_schema,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name,
	CONCAT('ALTER TABLE ', tc.table_schema, '.', tc.table_name, ' DROP CONSTRAINT ', tc.constraint_name, ';') as drop_statement
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'PRIMARY KEY'
    AND tc.table_schema='app'

ALTER TABLE app.PurchaseLineItems DROP CONSTRAINT FK_PurchaseLineItems_Inventory;
ALTER TABLE app.PurchaseLineItems DROP CONSTRAINT FK_PurchaseLineItems_Purchases;
ALTER TABLE app.Purchases DROP CONSTRAINT FK_Purchases_Kiosk;
ALTER TABLE app.PurchaseUser DROP CONSTRAINT FK_PurchaseUser_Purchases_PurchasesPurchaseId;
ALTER TABLE app.PurchaseUser DROP CONSTRAINT FK_PurchaseUser_Users_PurchasingUsersUserId;
ALTER TABLE app.PurchaseUserCreditCard DROP CONSTRAINT FK_PurchaseUserCreditCard_Purchases_PurchasesPurchaseId;
ALTER TABLE app.PurchaseUserCreditCard DROP CONSTRAINT FK_PurchaseUserCreditCard_UserCreditCards_PaymentCardsCreditCardId;
ALTER TABLE app.Rentals DROP CONSTRAINT FK_Rentals_movies;
ALTER TABLE app.Rentals DROP CONSTRAINT FK_Rentals_Users;
ALTER TABLE app.Returns DROP CONSTRAINT FK_Returns_PurchaseLineItems;
ALTER TABLE app.Returns DROP CONSTRAINT FK_Returns_Rentals;
ALTER TABLE app.UserAddresses DROP CONSTRAINT FK_UserAddresses_Users;
ALTER TABLE app.UserCreditCards DROP CONSTRAINT FK_UserCreditCards_Users;
ALTER TABLE app.UserReviews DROP CONSTRAINT FK_UserReviews_movies;
ALTER TABLE app.UserReviews DROP CONSTRAINT FK_UserReviews_Users;
ALTER TABLE app.UserSubscriptionStatus DROP CONSTRAINT FK_UserSubscriptionStatus_UserSubscriptionStatus;

ALTER TABLE app.Inventory DROP CONSTRAINT PK_Inventory;
ALTER TABLE app.Kiosk DROP CONSTRAINT PK_Kiosk;
ALTER TABLE app.PurchaseLineItems DROP CONSTRAINT PK_PurchaseLineItems;
ALTER TABLE app.Purchases DROP CONSTRAINT PK_Purchases;
ALTER TABLE app.PurchaseUser DROP CONSTRAINT PK_PurchaseUser;
ALTER TABLE app.PurchaseUserCreditCard DROP CONSTRAINT PK_PurchaseUserCreditCard;
ALTER TABLE app.Rentals DROP CONSTRAINT PK_Rentals;
ALTER TABLE app.Returns DROP CONSTRAINT PK_Returns;
ALTER TABLE app.UserAddresses DROP CONSTRAINT PK_UserAddresses;
ALTER TABLE app.UserCreditCards DROP CONSTRAINT PK_UserCreditCards;
ALTER TABLE app.UserReviews DROP CONSTRAINT PK_UserReviews;
ALTER TABLE app.Users DROP CONSTRAINT PK_Users;
ALTER TABLE app.UserSubscriptionStatus DROP CONSTRAINT PK_UserSubscriptionStatus;

ALTER TABLE Production.TransactionHistoryArchive
   ADD CONSTRAINT PK_TransactionHistoryArchive_TransactionID PRIMARY KEY CLUSTERED (TransactionID);

SELECT CONCAT('ALTER TABLE ', TABLE_SCHEMA, '.', TABLE_NAME, ' ADD CONSTRAINT PK_', TABLE_NAME, '_Id PRIMARY KEY CLUSTERED (Id)') as pk_statement FROM information_schema.tables WHERE TABLE_SCHEMA = 'app'

ALTER TABLE app.Rentals ADD CONSTRAINT PK_Rentals_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.Kiosk ADD CONSTRAINT PK_Kiosk_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.Users ADD CONSTRAINT PK_Users_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.UserSubscriptionStatus ADD CONSTRAINT PK_UserSubscriptionStatus_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.UserAddresses ADD CONSTRAINT PK_UserAddresses_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.UserCreditCards ADD CONSTRAINT PK_UserCreditCards_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.Inventory ADD CONSTRAINT PK_Inventory_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.Purchases ADD CONSTRAINT PK_Purchases_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.UserReviews ADD CONSTRAINT PK_UserReviews_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.PurchaseLineItems ADD CONSTRAINT PK_PurchaseLineItems_Id PRIMARY KEY CLUSTERED (Id)
ALTER TABLE app.Returns ADD CONSTRAINT PK_Returns_Id PRIMARY KEY CLUSTERED (Id)

/****** Object:  Table [app].[PurchaseUser]    Script Date: 4/2/2024 2:31:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PurchaseUser](
	[PurchasesPurchaseId] [uniqueidentifier] NOT NULL,
	[PurchasingUsersUserId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [app].[PurchaseUserCreditCard]    Script Date: 4/2/2024 2:32:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PurchaseUserCreditCard](
	[PaymentCardsCreditCardId] [uniqueidentifier] NOT NULL,
	[PurchasesPurchaseId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO

DROP TABLE app.PurchaseUser
DROP TABLE app.PurchaseUserCreditCard

ALTER TABLE app.PurchaseLineItems ADD UItemId uniqueidentifier NULL;
ALTER TABLE app.PurchaseLineItems ADD UPurchaseId uniqueidentifier NULL;

UPDATE app.PurchaseLineItems SET UItemId = i.Id FROM app.Inventory as i WHERE i.InventoryId = ItemId
UPDATE app.PurchaseLineItems SET UPurchaseId = p.Id FROM app.Purchases as p WHERE p.PurchaseId = app.PurchaseLineItems.PurchaseId

ALTER TABLE app.Rentals ADD UUserId uniqueidentifier NULL;
ALTER TABLE app.Rentals ADD UPurchaseLineItemId uniqueidentifier NULL;

UPDATE app.Rentals SET UUserId = u.Id FROM app.Users as u WHERE u.UserId = app.Rentals.UserId
UPDATE app.Rentals SET UPurchaseLineItemId = u.Id FROM app.PurchaseLineItems as u WHERE u.PurchaseLineItemId = app.Rentals.PurchaseLineItemId

ALTER TABLE app.Returns ADD URentalId uniqueidentifier NULL;
ALTER TABLE app.Returns ADD ULateChargeLineItemId uniqueidentifier NULL;

UPDATE app.Returns SET URentalId = r.Id FROM app.Rentals as r WHERE r.RentalId = app.Returns.RentalId

UPDATE app.Returns SET ULateChargeLineItemId = li.Id FROM app.PurchaseLineItems as li WHERE li.PurchaseLineItemId = app.Returns.LateChargeLineItemId

ALTER TABLE app.UserAddresses ADD UUserId uniqueidentifier NULL;

UPDATE app.UserAddresses SET UUserId = u.Id FROM app.Users as u WHERE u.UserId = app.UserAddresses.UserId

ALTER TABLE app.UserCreditCards ADD UUserId uniqueidentifier NULL;

UPDATE app.UserCreditCards SET UUserId = u.Id FROM app.Users as u WHERE u.UserId = app.UserCreditCards.UserId


SELECT TOP 10 * FROM app.Purchases WHERE PurchaseLocationId = 0

UPDATE app.Purchases SET PurchaseLocationId = 999999 WHERE PurchaseLocationId = 0

ALTER TABLE app.Purchases ADD UPurchaseLocationId uniqueidentifier NULL;
UPDATE app.Purchases SET UPurchaseLocationId = k.Id FROM app.Kiosk as k WHERE PurchaseLocationId = k.KioskId


CREATE TABLE [app].[PurchaseUser](
	[PurchasesPurchaseId] [uniqueidentifier] NOT NULL,
	[PurchasingUsersUserId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO

/****** Object:  Table [app].[PurchaseUserCreditCard]    Script Date: 4/2/2024 2:32:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PurchaseUserCreditCard](
	[PaymentCardsCreditCardId] [uniqueidentifier] NOT NULL,
	[PurchasesPurchaseId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO


INSERT INTO app.PurchaseUser
SELECT p.Id as PurchasesPurchaseId, u.Id as PurchasingUsersUserId FROM app.Purchases as p
JOIN dbo.PurchaseUserHold as puh ON puh.PurchasesPurchaseId = p.PurchaseId
JOIN app.Users as u ON puh.PurchasingUsersUserId = u.UserId

INSERT INTO app.[PurchaseUserCreditCard]
SELECT uc.Id as [PaymentCardsCreditCardId], p.Id as PurchasesPurchaseId FROM app.Purchases as p
JOIN dbo.PurchaseUserHold as puh ON puh.PurchasesPurchaseId = p.PurchaseId
JOIN app.UserCreditCards as uc ON uc.UserId = p.PurchasingUserId

select count(*) FROM app.UserCreditCards
select count(*) from app.Users
select count(distinct PurchasingUserId) FROM app.Purchases



