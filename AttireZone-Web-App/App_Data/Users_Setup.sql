-- =====================================================================
-- Users Table Setup Script for AttireZone Authentication System
-- Note: Execute this script against the AttireZone database
-- =====================================================================

-- Drop table if it exists (for clean reinstall)
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;

-- Create Users table
CREATE TABLE dbo.Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL,
    Password NVARCHAR(256) NOT NULL, -- Should be hashed in production
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastModifiedDate DATETIME DEFAULT GETDATE(),
    Role NVARCHAR(20) NOT NULL DEFAULT 'Customer' -- 'Customer' or 'Admin'
);

-- Create index on Email for faster lookups
CREATE INDEX IX_Users_Email ON dbo.Users(Email);

-- Insert sample data (optional - for testing)
-- INSERT INTO dbo.Users (FullName, Email, Password, CreatedDate, LastModifiedDate, Role)
-- VALUES 
--     ('John Doe', 'john@example.com', 'hashed_password_here', GETDATE(), GETDATE(), 'Customer'),
--     ('Jane Smith', 'jane@example.com', 'hashed_password_here', GETDATE(), GETDATE(), 'Admin');

PRINT 'Users table created successfully!';
