IF EXISTS (SELECT * FROM sys.databases WHERE name = 'JT5618_DB')
BEGIN
    ALTER DATABASE JT5618_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE JT5618_DB;
END
GO

CREATE DATABASE JT5618_DB;
GO

USE JT5618_DB;
GO

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO

-- Clear existing data and reset identity counters to start from 1
DELETE FROM Products;
DELETE FROM Categories;
DBCC CHECKIDENT ('Products', RESEED, 1);
DBCC CHECKIDENT ('Categories', RESEED, 1);

-- Insert Categories
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, Description) VALUES
(1, 'Motherboard', 'Main Circuit Board'),
(2, 'CPU', 'Central Processing Unit'),
(3, 'GPU', 'Graphics Processing Unit'),
(4, 'RAM', 'Random Access Memory'),
(5, 'HDD', 'Hard Disk Drive'),
(6, 'SSD', 'Solid State Drive'),
(7, 'Monitor', 'Display Device'),
(8, 'Keyboard', 'Typing Input Device'),
(9, 'Mouse', 'Pointing Input Device');
SET IDENTITY_INSERT Categories OFF;
GO

-- Insert Products (using the exact IDs we want)
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, Name, Description, Price, Quantity, CategoryId) VALUES
-- Motherboards
(1, 'ASUS ROG STRIX B550-F', 'High-end gaming motherboard', 179.99, 10, 1),
(2, 'MSI MPG B550', 'Mid-range gaming board', 149.99, 15, 1),

-- CPUs
(3, 'AMD Ryzen 7 5800X', '8-core gaming CPU', 299.99, 8, 2),
(4, 'Intel i5-12600K', '10-core hybrid CPU', 279.99, 12, 2),

-- GPUs
(5, 'NVIDIA RTX 4070', 'Mid-range gaming GPU', 599.99, 5, 3),
(6, 'AMD RX 6800 XT', 'High-end gaming GPU', 649.99, 4, 3),

-- RAM
(7, 'Corsair Vengeance 32GB', 'DDR4 gaming memory', 129.99, 20, 4),
(8, 'G.Skill Trident 16GB', 'RGB gaming RAM', 89.99, 25, 4),

-- HDDs
(9, 'Seagate 4TB', 'Storage drive', 89.99, 30, 5),
(10, 'WD Black 2TB', 'Gaming HDD', 79.99, 25, 5),

-- SSDs
(11, 'Samsung 970 EVO 1TB', 'NVMe SSD', 99.99, 15, 6),
(12, 'WD Black SN850 2TB', 'Gaming SSD', 199.99, 10, 6),

-- Monitors
(13, 'LG 27GL850', '27" Gaming Monitor', 449.99, 8, 7),
(14, 'ASUS TUF 24"', 'Gaming Monitor', 249.99, 12, 7),

-- Keyboards
(15, 'Logitech G915', 'Wireless Mechanical', 199.99, 10, 8),
(16, 'Razer Huntsman', 'Optical Gaming', 149.99, 15, 8),

-- Mice
(17, 'Logitech G Pro X', 'Wireless Gaming Mouse', 149.99, 20, 9),
(18, 'Razer DeathAdder V2', 'Ergonomic Gaming Mouse', 69.99, 25, 9);
SET IDENTITY_INSERT Products OFF;
GO

-- Verify the identity values
SELECT IDENT_CURRENT('Categories') as CategoryIdentity;
SELECT IDENT_CURRENT('Products') as ProductIdentity;
GO
