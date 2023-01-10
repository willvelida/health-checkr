CREATE TABLE [dbo].[SleepSummary] (
    [Id]                 INT  IDENTITY NOT NULL PRIMARY KEY,
    [TotalMinutesAsleep] INT  NOT NULL,
    [TotalSleepRecords]  INT  NOT NULL,
    [TotalTimeInBed]     INT  NOT NULL,
    [DeepSleep]          INT  NOT NULL,
    [LightSleep]         INT  NOT NULL,
    [REMSleep]           INT  NOT NULL,
    [AwakeMinutes]       INT  NOT NULL,
    [Date]               DATE NOT NULL,
)
GO;

CREATE TABLE [dbo].[Sp02] (
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [DATE] DATE NOT NULL,
  [AvgValue] DECIMAL(5,2) NOT NULL,
  [MinValue] DECIMAL(5,2) NOT NULL,
  [MaxValue] DECIMAL(5,2) NOT NULL
)
GO;

CREATE TABLE [dbo].[BreathingRate] (
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [Date] DATE NOT NULL,
  [BreathingRate] DECIMAL(5,2) NOT NULL
)
GO;

CREATE TABLE [dbo].[Sleep]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [Date] DATE NOT NULL,
  [Efficiency] INT NOT NULL,
  [Duration] INT NOT NULL,
  [StartTime] DATETIME NOT NULL,
  [EndTime] DATETIME NOT NULL,
  [MinutesAfterWakeup] INT NOT NULL,
  [MinutesAsleep] INT NOT NULL,
  [MinutesAwake] INT NOT NULL,
  [MinutesToFallAsleep] INT NOT NULL,
  [RestlessCount] INT NOT NULL,
  [RestlessDuration] INT NOT NULL,
  [TimeInBed] INT NOT NULL,
  [SleepSummaryId] INT FOREIGN KEY REFERENCES [dbo].[SleepSummary] ([Id])
)
GO;