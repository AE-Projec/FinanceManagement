This Project uses a MSSQL Database. You wont be able to fully use or test it without that DB.
In Order to do so just copy this Code in your SQL Script:

create database FinanceProject;

CREATE TABLE BudgetLimits (
    BudgetID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Budget_Amount INT not null,
    Budget_Limit_Year INT,
    Budget_Category NVARCHAR(100),
    Creation_Date DATE DEFAULT GETDATE(),
    Budget_Status NVARCHAR(50),
    Approved_By NVARCHAR(100),
    Comment NVARCHAR(255),
    Currency CHAR(3) not null
   
);

CREATE TABLE Earnings (
    EarningID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    TransactionType VARCHAR(15),
    Amount DECIMAL(18, 2),
    TransactionDate DATE DEFAULT GETDATE(),
    Category VARCHAR(50),
    Description VARCHAR(150),
    PaymentMethod VARCHAR(50)
);

CREATE TABLE Expenses (
    ExpenseID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    TransactionType VARCHAR(15),
    Amount DECIMAL(18, 2),
    TransactionDate DATE DEFAULT GETDATE(),
    Category VARCHAR(50),
    TransactionDescription VARCHAR(150),
    PaymentMethod VARCHAR(50),
    Vendor VARCHAR(50)
);

(Exepenses and Earnings are not yet relevant because its not yet fully implemented)

Also : 
In the c# Project you need to change the connectionString in the App.config

