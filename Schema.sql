-- =========================================================
-- Schema.sql
-- Run this in a Query window connected to your SQL Server
-- (Server Explorer -> right click your connection -> New Query,
-- or via the console app method we used earlier).
-- =========================================================

-- Note: the database name contains hyphens, so it must be wrapped in
-- square brackets [ ] every time it's used in SQL (hyphens aren't valid
-- in an unquoted identifier).
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '22-48044-2_LoginDB')
BEGIN
    CREATE DATABASE [22-48044-2_LoginDB];
END
GO

USE [22-48044-2_LoginDB];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID       INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        Email        NVARCHAR(100) NULL,
        FullName     NVARCHAR(100) NULL,
        CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Sanity check: confirm the table exists and see its columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users';
