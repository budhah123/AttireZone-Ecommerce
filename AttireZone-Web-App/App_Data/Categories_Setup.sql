-- ============================================================
-- AttireZone Categories Table Setup Script
-- ============================================================

USE [AttireZone];
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categories]
    (
        [id] INT IDENTITY(1,1) PRIMARY KEY,
        [name] NVARCHAR(150) NOT NULL,
        [description] NVARCHAR(MAX) NULL,
        [created_date] DATETIME NOT NULL DEFAULT GETDATE()
    );

    PRINT 'Categories table created successfully.';
END
ELSE
BEGIN
    PRINT 'Categories table already exists. No changes made.';
END
GO
