-- First, disable foreign key constraints
ALTER TABLE Products NOCHECK CONSTRAINT ALL;
ALTER TABLE Categories NOCHECK CONSTRAINT ALL;

-- Delete all existing data
DELETE FROM Products;
DELETE FROM Categories;

-- Reset identity counters
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);

-- Insert Categories starting from ID 1
INSERT INTO Categories (Name, Description) VALUES
('Motherboard', 'Main Circuit Board'),
('CPU', 'Central Processing Unit'),
('GPU', 'Graphics Processing Unit'),
('RAM', 'Random Access Memory'),
('HDD', 'Hard Disk Drive'),
('SSD', 'Solid State Drive'),
('Monitor', 'Display Device'),
('Keyboard', 'Typing Input Device'),
('Mouse', 'Pointing Input Device');

-- Re-enable foreign key constraints
ALTER TABLE Products CHECK CONSTRAINT ALL;
ALTER TABLE Categories CHECK CONSTRAINT ALL; 