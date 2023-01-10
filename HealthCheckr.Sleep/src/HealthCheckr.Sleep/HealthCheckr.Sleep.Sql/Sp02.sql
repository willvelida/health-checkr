CREATE TABLE [dbo].[Sp02] (
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [DATE] DATE NOT NULL,
  [AvgValue] DECIMAL(5,2) NOT NULL,
  [MinValue] DECIMAL(5,2) NOT NULL,
  [MaxValue] DECIMAL(5,2) NOT NULL
)