-- Users
CREATE TABLE IF NOT EXISTS Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Username     TEXT NOT NULL UNIQUE,
    Email        TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt    TEXT NOT NULL
);

-- UserProfiles
CREATE TABLE IF NOT EXISTS UserProfiles (
    Id                INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId            INTEGER NOT NULL UNIQUE,
    MonthlyIncome     REAL NOT NULL,
    RentAmount        REAL NOT NULL,
    HomeSavingsAmount REAL NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Categories
CREATE TABLE IF NOT EXISTS Categories (
    Id    INTEGER PRIMARY KEY AUTOINCREMENT,
    Name  TEXT NOT NULL,
    Icon  TEXT NOT NULL,
    Color TEXT NOT NULL
);

-- Expenses
CREATE TABLE IF NOT EXISTS Expenses (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    Title      TEXT NOT NULL,
    Amount     REAL NOT NULL,
    Date       TEXT NOT NULL,
    Note       TEXT,
    Source     TEXT NOT NULL DEFAULT 'manual',
    UserId     INTEGER NOT NULL,
    CategoryId INTEGER NOT NULL,
    FOREIGN KEY (UserId)     REFERENCES Users(Id)      ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE RESTRICT
);

-- Budgets
CREATE TABLE IF NOT EXISTS Budgets (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    LimitAmount REAL NOT NULL,
    Month       INTEGER NOT NULL,
    Year        INTEGER NOT NULL,
    UserId      INTEGER NOT NULL,
    CategoryId  INTEGER NOT NULL,
    FOREIGN KEY (UserId)     REFERENCES Users(Id)      ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE RESTRICT
);

-- AIInsights
CREATE TABLE IF NOT EXISTS AIInsights (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    InsightText TEXT NOT NULL,
    Type        TEXT NOT NULL,
    GeneratedAt TEXT NOT NULL,
    UserId      INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Seed Categories
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Food', '🍔', '#FF6B6B' WHERE NOT EXISTS (SELECT 1 FROM Categories);
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Transport', '🚗', '#4ECDC4' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Transport');
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Shopping', '🛍️', '#45B7D1' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Shopping');
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Entertainment', '🎬', '#96CEB4' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Entertainment');
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Health', '💊', '#FFEAA7' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Health');
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Utilities', '💡', '#DDA0DD' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Utilities');
INSERT INTO Categories (Name, Icon, Color)
SELECT 'Other', '📦', '#B0B0B0' WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Other');