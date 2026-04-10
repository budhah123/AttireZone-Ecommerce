-- ============================================================
-- AttireZone CustomerWishlists Table Setup Script
-- ============================================================

USE [AttireZone];
GO

IF OBJECT_ID(N'dbo.CustomerWishlists', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CustomerWishlists]
    (
        [WishlistId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_CustomerWishlists_CreatedAt] DEFAULT GETDATE(),
        CONSTRAINT [UQ_CustomerWishlists_User_Product] UNIQUE ([UserId], [ProductId])
    );

    PRINT 'CustomerWishlists table created successfully.';
END
ELSE
BEGIN
    PRINT 'CustomerWishlists table already exists. No schema change needed.';
END
GO

IF OBJECT_ID(N'dbo.CustomerWishlists', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CustomerWishlists_UserId' AND object_id = OBJECT_ID(N'dbo.CustomerWishlists'))
    BEGIN
        CREATE INDEX [IX_CustomerWishlists_UserId] ON [dbo].[CustomerWishlists]([UserId]);
        PRINT 'Created index IX_CustomerWishlists_UserId.';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CustomerWishlists_ProductId' AND object_id = OBJECT_ID(N'dbo.CustomerWishlists'))
    BEGIN
        CREATE INDEX [IX_CustomerWishlists_ProductId] ON [dbo].[CustomerWishlists]([ProductId]);
        PRINT 'Created index IX_CustomerWishlists_ProductId.';
    END

    IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
       AND COL_LENGTH('dbo.Users', 'UserId') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerWishlists_Users')
    BEGIN
        ALTER TABLE [dbo].[CustomerWishlists] WITH NOCHECK
            ADD CONSTRAINT [FK_CustomerWishlists_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]);

        PRINT 'Added FK_CustomerWishlists_Users.';
    END

    DECLARE @productIdColumn SYSNAME = NULL;

    IF COL_LENGTH('dbo.Products', 'id') IS NOT NULL
    BEGIN
        SET @productIdColumn = 'id';
    END
    ELSE IF COL_LENGTH('dbo.Products', 'ProductId') IS NOT NULL
    BEGIN
        SET @productIdColumn = 'ProductId';
    END

    IF @productIdColumn IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerWishlists_Products')
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'ALTER TABLE [dbo].[CustomerWishlists] WITH NOCHECK '
            + N'ADD CONSTRAINT [FK_CustomerWishlists_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([' + @productIdColumn + N']);';

        EXEC sp_executesql @sql;
        PRINT 'Added FK_CustomerWishlists_Products.';
    END
END
GO
