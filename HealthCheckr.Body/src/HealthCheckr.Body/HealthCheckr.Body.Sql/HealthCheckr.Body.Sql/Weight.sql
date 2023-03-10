CREATE TABLE [dbo].[Weight]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [BmiValue] DECIMAL(5,2) NOT NULL,
  [Date] DATE NOT NULL,
  [FatPercentage] DECIMAL(5,2) NOT NULL,
  [Source] VARCHAR(100) NOT NULL,
  [Time] TIME NOT NULL,
  [WeightInKG] DECIMAL(5, 2) NOT NULL
)