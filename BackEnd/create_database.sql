-- FastPCB Database Schema Creation Script
-- Execute this script in MySQL if automated migrations fail

-- Create Database
CREATE DATABASE IF NOT EXISTS FastPCB;
USE FastPCB;

-- Create Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(500) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Address VARCHAR(500) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL,
    INDEX IX_Users_Email (Email)
) CHARACTER SET utf8mb4;

-- Create Orders table
CREATE TABLE Orders (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    OrderName VARCHAR(200) NOT NULL,
    Description LONGTEXT NOT NULL,
    OrderDate DATETIME(6) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL,
    TrackingNumber VARCHAR(100) NULL,
    FOREIGN KEY FK_Orders_Users_UserId (UserId) REFERENCES Users(Id) ON DELETE RESTRICT,
    INDEX IX_Orders_UserId (UserId)
) CHARACTER SET utf8mb4;

-- Create Tickets table
CREATE TABLE Tickets (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    Subject VARCHAR(200) NOT NULL,
    IssueDescription LONGTEXT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    Status INT NOT NULL,
    ResolvedAt DATETIME(6) NULL,
    FOREIGN KEY FK_Tickets_Users_UserId (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_Tickets_UserId (UserId)
) CHARACTER SET utf8mb4;

-- Create OrderFiles table
CREATE TABLE OrderFiles (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OrderId INT NOT NULL,
    FileName VARCHAR(255) NOT NULL,
    FileUrl VARCHAR(500) NOT NULL,
    FileType VARCHAR(10) NOT NULL,
    FileSize BIGINT NOT NULL,
    UploadDate DATETIME(6) NOT NULL,
    UploadedBy VARCHAR(100) NOT NULL,
    FOREIGN KEY FK_OrderFiles_Orders_OrderId (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    INDEX IX_OrderFiles_OrderId (OrderId)
) CHARACTER SET utf8mb4;

-- Create OrderSpecifications table
CREATE TABLE OrderSpecifications (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OrderId INT NOT NULL,
    LayerCount INT NOT NULL,
    MinimumSpacing DOUBLE NOT NULL,
    Material VARCHAR(100) NOT NULL,
    CopperWeight VARCHAR(50) NOT NULL,
    SolderMask VARCHAR(50) NOT NULL,
    Silkscreen VARCHAR(50) NOT NULL,
    HasVias TINYINT(1) NOT NULL,
    HasPlatedHoles TINYINT(1) NOT NULL,
    SurfaceFinish VARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Notes LONGTEXT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL,
    FOREIGN KEY FK_OrderSpecifications_Orders_OrderId (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    INDEX IX_OrderSpecifications_OrderId (OrderId)
) CHARACTER SET utf8mb4;

-- Create OrderStatusHistories table
CREATE TABLE OrderStatusHistories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OrderId INT NOT NULL,
    Status INT NOT NULL,
    ChangedDate DATETIME(6) NOT NULL,
    Notes LONGTEXT NOT NULL,
    UpdatedBy VARCHAR(100) NOT NULL,
    FOREIGN KEY FK_OrderStatusHistories_Orders_OrderId (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    INDEX IX_OrderStatusHistories_OrderId (OrderId)
) CHARACTER SET utf8mb4;

-- Create Payments table
CREATE TABLE Payments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OrderId INT NOT NULL UNIQUE,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod VARCHAR(100) NOT NULL,
    TransactionId VARCHAR(255) NOT NULL,
    Status INT NOT NULL,
    PaymentDate DATETIME(6) NOT NULL,
    ConfirmedDate DATETIME(6) NULL,
    FOREIGN KEY FK_Payments_Orders_OrderId (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    UNIQUE INDEX IX_Payments_OrderId (OrderId)
) CHARACTER SET utf8mb4;

-- Create TicketResponses table
CREATE TABLE TicketResponses (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    TicketId INT NOT NULL,
    Response LONGTEXT NOT NULL,
    RespondedBy VARCHAR(100) NOT NULL,
    RespondedAt DATETIME(6) NOT NULL,
    IsAdminResponse TINYINT(1) NOT NULL,
    FOREIGN KEY FK_TicketResponses_Tickets_TicketId (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE,
    INDEX IX_TicketResponses_TicketId (TicketId)
) CHARACTER SET utf8mb4;

-- Create __EFMigrationsHistory table for EF Core tracking
CREATE TABLE __EFMigrationsHistory (
    MigrationId VARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion VARCHAR(32) NOT NULL
) CHARACTER SET utf8mb4;

-- Insert the migration record
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) 
VALUES ('20260323000000_InitialCreate', '8.0.10');
