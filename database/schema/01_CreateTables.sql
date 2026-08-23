/* =========================================================
   HappyEat-portfolio
   Database Schema
   SQL Server
   ========================================================= */

-- =========================================================
-- 1. Users
-- =========================================================

CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    BirthDate DATE NULL,
    Gender NVARCHAR(10) NULL,

    CONSTRAINT PK_Users
        PRIMARY KEY (UserID)
);
GO


-- =========================================================
-- 2. Food
-- =========================================================

CREATE TABLE Food
(
    FoodID INT IDENTITY(1,1) NOT NULL,
    FoodName NVARCHAR(50) NOT NULL,
    FoodCategory NVARCHAR(50) NULL,
    Calories DECIMAL(10,2) NOT NULL,
    Carbs DECIMAL(10,2) NULL,
    Protein DECIMAL(10,2) NULL,
    Fat DECIMAL(10,2) NULL,

    CONSTRAINT PK_Food
        PRIMARY KEY (FoodID)
);
GO


-- =========================================================
-- 3. Drink
-- =========================================================

CREATE TABLE Drink
(
    DrinkID INT IDENTITY(1,1) NOT NULL,
    DrinkName NVARCHAR(50) NULL,
    Size NVARCHAR(10) NULL,
    DrinkCategory NVARCHAR(50) NULL,
    Calories DECIMAL(10,2) NULL,
    Carbs DECIMAL(10,2) NULL,
    Protein DECIMAL(10,2) NULL,
    Fat DECIMAL(10,2) NULL,

    CONSTRAINT PK_Drink
        PRIMARY KEY (DrinkID)
);
GO


-- =========================================================
-- 4. BodyRecords
-- =========================================================

CREATE TABLE BodyRecords
(
    BodyRecordID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    RecordDate DATE NOT NULL,
    ActivityLevel DECIMAL(4,3) NOT NULL,
    Height DECIMAL(5,2) NOT NULL,
    Weight DECIMAL(5,2) NOT NULL,
    WaistSize DECIMAL(5,2) NULL,
    NeckSize DECIMAL(5,2) NULL,
    HipSize DECIMAL(5,2) NULL,
    BodyFat DECIMAL(5,2) NULL,
    MuscleMass DECIMAL(5,2) NULL,
    VisceralFat DECIMAL(5,2) NULL,

    CONSTRAINT PK_BodyRecords
        PRIMARY KEY (BodyRecordID),

    CONSTRAINT FK_BodyRecords_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);
GO


-- =========================================================
-- 5. UserGoals
-- =========================================================

CREATE TABLE UserGoals
(
    GoalID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    GoalType NVARCHAR(10) NOT NULL,
    TargetWeight DECIMAL(5,2) NULL,
    TargetBodyFat DECIMAL(5,2) NULL,
    StartDate DATE NOT NULL,
    TargetDate DATE NULL,
    IsActive BIT NOT NULL
        CONSTRAINT DF_UserGoals_IsActive DEFAULT 1,

    CONSTRAINT PK_UserGoals
        PRIMARY KEY (GoalID),

    CONSTRAINT FK_UserGoals_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);
GO


-- =========================================================
-- 6. FoodImage
-- =========================================================

CREATE TABLE FoodImage
(
    ImageID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    ImagePath NVARCHAR(500) NULL,
    UploadTime DATETIME2 NOT NULL
        CONSTRAINT DF_FoodImage_UploadTime DEFAULT GETDATE(),

    CONSTRAINT PK_FoodImage
        PRIMARY KEY (ImageID),

    CONSTRAINT FK_FoodImage_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);
GO


-- =========================================================
-- 7. FoodRecords
-- =========================================================

CREATE TABLE FoodRecords
(
    RecordID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    ImageID INT NULL,
    RecordSource NVARCHAR(20) NOT NULL,
    RecordDate DATETIME2 NOT NULL,
    MealType NVARCHAR(50) NULL,
    Description NVARCHAR(500) NULL,

    CONSTRAINT PK_FoodRecords
        PRIMARY KEY (RecordID),

    CONSTRAINT FK_FoodRecords_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID),

    CONSTRAINT FK_FoodRecords_FoodImage
        FOREIGN KEY (ImageID)
        REFERENCES FoodImage(ImageID)
);
GO


-- =========================================================
-- 8. FoodDetected
-- =========================================================

CREATE TABLE FoodDetected
(
    DetectID INT IDENTITY(1,1) NOT NULL,
    ImageID INT NOT NULL,
    EstimatedFood NVARCHAR(200) NULL,
    EstimatedWeight DECIMAL(10,2) NULL,
    EstimatedCalories DECIMAL(10,2) NOT NULL,
    EstimatedCarbs DECIMAL(10,2) NULL,
    EstimatedProtein DECIMAL(10,2) NULL,
    EstimatedFat DECIMAL(10,2) NULL,
    AINote NVARCHAR(500) NULL,

    CONSTRAINT PK_FoodDetected
        PRIMARY KEY (DetectID),

    CONSTRAINT FK_FoodDetected_FoodImage
        FOREIGN KEY (ImageID)
        REFERENCES FoodImage(ImageID)
);
GO


-- =========================================================
-- 9. FoodRecordItems
-- =========================================================

CREATE TABLE FoodRecordItems
(
    ItemID INT IDENTITY(1,1) NOT NULL,
    ItemName NVARCHAR(100) NULL,
    RecordID INT NOT NULL,
    FoodID INT NULL,
    DrinkID INT NULL,
    Quantity DECIMAL(10,2) NULL,
    Unit NVARCHAR(20) NULL,
    Calories DECIMAL(10,2) NOT NULL,
    Carbs DECIMAL(10,2) NULL,
    Protein DECIMAL(10,2) NULL,
    Fat DECIMAL(10,2) NULL,

    CONSTRAINT PK_FoodRecordItems
        PRIMARY KEY (ItemID),

    CONSTRAINT FK_FoodRecordItems_FoodRecords
        FOREIGN KEY (RecordID)
        REFERENCES FoodRecords(RecordID),

    CONSTRAINT FK_FoodRecordItems_Food
        FOREIGN KEY (FoodID)
        REFERENCES Food(FoodID),

    CONSTRAINT FK_FoodRecordItems_Drink
        FOREIGN KEY (DrinkID)
        REFERENCES Drink(DrinkID),

    -- FoodID / DrinkID 不可同時有值
    -- 允許兩者皆為 NULL（例如 AI 辨識到資料庫不存在的食物）
    CONSTRAINT CK_FoodRecordItems_FoodOrDrink
        CHECK (
            NOT (
                FoodID IS NOT NULL
                AND DrinkID IS NOT NULL
            )
        )
);
GO