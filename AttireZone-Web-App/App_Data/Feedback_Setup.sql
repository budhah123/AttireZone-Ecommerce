-- ============================================================
-- AttireZone Feedback Table Setup Script
-- ============================================================

USE [AttireZone];
GO

IF OBJECT_ID(N'dbo.Feedback', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Feedback]
    (
        [FeedbackId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Rating] INT NOT NULL,
        [Comment] NVARCHAR(1000) NULL,
        [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_Feedback_CreatedAt] DEFAULT GETDATE(),
        CONSTRAINT [CK_Feedback_Rating] CHECK ([Rating] BETWEEN 1 AND 5)
    );

    PRINT 'Feedback table created successfully.';
END
ELSE
BEGIN
    PRINT 'Feedback table already exists. No schema change needed.';
END
GO

IF OBJECT_ID(N'dbo.Feedback', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Feedback_UserId' AND object_id = OBJECT_ID(N'dbo.Feedback'))
    BEGIN
        CREATE INDEX [IX_Feedback_UserId] ON [dbo].[Feedback]([UserId]);
        PRINT 'Created index IX_Feedback_UserId.';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Feedback_ProductId' AND object_id = OBJECT_ID(N'dbo.Feedback'))
    BEGIN
        CREATE INDEX [IX_Feedback_ProductId] ON [dbo].[Feedback]([ProductId]);
        PRINT 'Created index IX_Feedback_ProductId.';
    END

    IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
       AND COL_LENGTH('dbo.Users', 'UserId') IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Feedback_Users')
    BEGIN
        ALTER TABLE [dbo].[Feedback] WITH NOCHECK
            ADD CONSTRAINT [FK_Feedback_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]);

        PRINT 'Added FK_Feedback_Users.';
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
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Feedback_Products')
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'ALTER TABLE [dbo].[Feedback] WITH NOCHECK '
            + N'ADD CONSTRAINT [FK_Feedback_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([' + @productIdColumn + N']);';

        EXEC sp_executesql @sql;
        PRINT 'Added FK_Feedback_Products.';
    END
END
GO
