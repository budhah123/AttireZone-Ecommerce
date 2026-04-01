-- ============================================================
-- AttireZone Products isPopular Migration Script
-- Adds [isPopular] BIT column if it does not already exist.
-- ============================================================

USE [AttireZone];
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    PRINT 'Products table not found. No changes made.';
    RETURN;
END
GO

IF COL_LENGTH('dbo.Products', 'isPopular') IS NULL
BEGIN
    ALTER TABLE [dbo].[Products]
        ADD [isPopular] BIT NOT NULL CONSTRAINT [DF_Products_isPopular] DEFAULT ((0));

    PRINT 'Added [isPopular] column to dbo.Products.';
END
ELSE
BEGIN
    PRINT '[isPopular] column already exists on dbo.Products. No schema change needed.';
END
GO
