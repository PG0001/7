-- ============================================
-- 1. CREATE DATABASE (if not exists)
-- ============================================
IF DB_ID('EventHubDB') IS NULL
BEGIN
    CREATE DATABASE EventHubDB;
END
GO

USE EventHubDB;
GO

-- ============================================
-- 2. USERS
-- ============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20),
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

-- Insert example user
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'alice@example.com')
BEGIN
    INSERT INTO Users (UserName, Email, PhoneNumber)
    VALUES ('Alice', 'alice@example.com', '+911234567890');
END

-- ============================================
-- 3. EVENTS
-- ============================================
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY(UserId) REFERENCES Users(UserId)
);

-- Insert example event
IF NOT EXISTS (SELECT 1 FROM Events WHERE EventName = 'Interview with Bob')
BEGIN
    INSERT INTO Events (UserId, EventName, Description, StartTime, EndTime)
    VALUES (1, 'Interview with Bob', 'Technical Interview', DATEADD(HOUR,26, SYSUTCDATETIME()), DATEADD(HOUR,27, SYSUTCDATETIME()));
END

-- ============================================
-- 4. NOTIFICATION TEMPLATES
-- ============================================
CREATE TABLE NotificationTemplates (
    TemplateId INT IDENTITY(1,1) PRIMARY KEY,
    TemplateName NVARCHAR(100) NOT NULL UNIQUE,
    Subject NVARCHAR(200),
    Body NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

-- Seed templates
IF NOT EXISTS (SELECT 1 FROM NotificationTemplates WHERE TemplateName='booking_confirmation')
BEGIN
    INSERT INTO NotificationTemplates (TemplateName, Subject, Body)
    VALUES ('booking_confirmation', 'Your booking is confirmed', 'Hello {{UserName}}, your booking for {{EventName}} is confirmed on {{StartTime}}.');
END

IF NOT EXISTS (SELECT 1 FROM NotificationTemplates WHERE TemplateName='event_reminder')
BEGIN
    INSERT INTO NotificationTemplates (TemplateName, Subject, Body)
    VALUES ('event_reminder', 'Reminder: {{EventName}}', 'Hi {{UserName}}, this is a reminder for {{EventName}} at {{StartTime}}.');
END

-- ============================================
-- 5. NOTIFICATION SETTINGS
-- ============================================
CREATE TABLE NotificationSettings (
    SettingId INT IDENTITY(1,1) PRIMARY KEY,
    IsEmailEnabled BIT DEFAULT 1,
    IsSmsEnabled BIT DEFAULT 1,
    Reminder24hrEnabled BIT DEFAULT 1,
    Reminder1hrEnabled BIT DEFAULT 1,
    ReminderHoursBeforeEvent INT DEFAULT 24,
    ReminderHoursBeforeEvent1 INT DEFAULT 1,
    MaxSendAttempts INT DEFAULT 5,
    UpdatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

-- Seed default settings
IF NOT EXISTS (SELECT 1 FROM NotificationSettings)
BEGIN
    INSERT INTO NotificationSettings (IsEmailEnabled, IsSmsEnabled, Reminder24hrEnabled, Reminder1hrEnabled)
    VALUES (1,1,1,1);
END

-- ============================================
-- 6. NOTIFICATIONS
-- ============================================
CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    EventId INT NULL,
    Type NVARCHAR(10) NOT NULL, -- EMAIL | SMS | IN_APP
    Title NVARCHAR(200) NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    SentAt DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY(UserId) REFERENCES Users(UserId),
    FOREIGN KEY(EventId) REFERENCES Events(EventId)
);

-- ============================================
-- 7. SCHEDULED JOBS
-- ============================================
CREATE TABLE ScheduledJobs (
    JobId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    EventId INT NOT NULL,
    NotificationType NVARCHAR(20) NOT NULL, -- REMINDER_24HR | REMINDER_1HR
    ScheduledTime DATETIME2 NOT NULL,
    IsTriggered BIT DEFAULT 0,
    TriggeredAt DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY(UserId) REFERENCES Users(UserId),
    FOREIGN KEY(EventId) REFERENCES Events(EventId)
);

-- Seed example scheduled jobs
IF NOT EXISTS (SELECT 1 FROM ScheduledJobs WHERE EventId=1 AND NotificationType='REMINDER_24HR')
BEGIN
    INSERT INTO ScheduledJobs (UserId, EventId, NotificationType, ScheduledTime)
    VALUES (1,1,'REMINDER_24HR', DATEADD(HOUR,2,SYSUTCDATETIME()));
END

IF NOT EXISTS (SELECT 1 FROM ScheduledJobs WHERE EventId=1 AND NotificationType='REMINDER_1HR')
BEGIN
    INSERT INTO ScheduledJobs (UserId, EventId, NotificationType, ScheduledTime)
    VALUES (1,1,'REMINDER_1HR', DATEADD(HOUR,25,SYSUTCDATETIME()));
END

-- ============================================
-- 8. INDEXES
-- ============================================
CREATE INDEX idx_user_notifications ON Notifications(UserId, IsRead);
CREATE INDEX idx_scheduled_time ON ScheduledJobs(ScheduledTime, IsTriggered);
CREATE INDEX idx_event_user ON Events(UserId);





Select* FROM Users;
Select* FROM Events;
Select* FROM NotificationTemplates;
Select* FROM NotificationSettings;
Select* FROM Notifications;
Select* FROM ScheduledJobs;
ALTER TABLE ScheduledJobs
ALTER COLUMN NotificationType INT NOT NULL;

EXEC sp_help 'ScheduledJobs';
-- Step 1: Add a temporary INT column
ALTER TABLE ScheduledJobs
ADD NotificationType INT;

-- Step 2: Map string values to enum values into the new column
UPDATE ScheduledJobs
SET NotificationType =
    CASE NotificationType
        WHEN 'REMINDER_24HR' THEN 1
        WHEN 'REMINDER_1HR' THEN 2
        ELSE 0
    END;

-- Step 3: Drop old column
ALTER TABLE ScheduledJobs
DROP COLUMN NotificationType;

-- Step 4: Rename temporary column to original name
EXEC sp_rename 'ScheduledJobs.NotificationTypeInt', 'NotificationType', 'COLUMN';
SELECT * 
FROM ScheduledJobs
WHERE NotificationType IS NULL
   OR EventId IS NULL
   OR UserId IS NULL;
   -- Set default enum value (0) for NULL NotificationType
UPDATE ScheduledJobs
SET NotificationType = 0
WHERE NotificationType IS NULL;

-- Set EventId/UserId to 1 or correct value
UPDATE ScheduledJobs
SET EventId = 1
WHERE EventId IS NULL;

UPDATE ScheduledJobs
SET UserId = 1
WHERE UserId IS NULL;
ALTER TABLE ScheduledJobs
ALTER COLUMN NotificationType INT NOT NULL;

ALTER TABLE ScheduledJobs
ALTER COLUMN EventId INT NOT NULL;

ALTER TABLE ScheduledJobs
ALTER COLUMN UserId INT NOT NULL;


ALTER TABLE Notifications
ADD ChannelResponse NVARCHAR(MAX) NULL,
    Status NVARCHAR(50) NULL,
    TemplateId INT NULL;
    SELECT * FROM Users;
