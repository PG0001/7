# EventHub – Advanced Notification, Reminder & Scheduling Module

## Overview
This project implements a modular notification and scheduling system for EventHub. It supports multi-channel notifications (Email, SMS, In-App) and scheduled reminders for upcoming events. The system is designed to be scalable, configurable, and easy to extend.

---

## Features

### 1. Notification Module
- **Email Notifications**
  - Event booking confirmation
  - Event cancellation or updates
- **SMS Notifications**
  - Short reminders (e.g., “Your event starts in 1 hour”)
  - Uses Twilio or a dummy service
- **In-App Notifications**
  - Stored in the database
  - Users can view read/unread notifications
  - API endpoint to fetch latest notifications

### 2. Notification Scheduler
- **Event Reminders**
  - Sends reminders 24 hours and 1 hour before events
  - Skips 24-hour reminder if event is created less than 24 hours before start
- **Background Worker**
  - Uses `IHostedService` / `BackgroundService`
  - Checks pending reminders every minute
  - Triggers notifications automatically

### 3. Notification Templates
- Templates stored in DB
- Supports placeholders:
  - `{{UserName}}`
  - `{{EventName}}`
  - `{{Date}}`
  - `{{StartTime}}`
- Template types:
  - Booking Confirmation
  - Booking Cancellation
  - Event Reminder
  - Event Update

### 4. Admin Configuration
- Enable/disable SMS notifications
- Enable/disable Email notifications
- Configure reminder timings (e.g., 1 hour, 2 hours before event)
- Stored in `NotificationSettings` table in DB

---

## Database Structure

### Tables
1. **Users**
2. **Events**
3. **Notifications**
4. **NotificationTemplates**
5. **NotificationSettings**
6. **ScheduledJobs** (for reminders)

### Relationships
- Notifications → User (FK)
- Notifications → Event (FK, optional)
- ScheduledJobs → Event & User (FK)

*(See ER diagram for full details)*

---

## API Endpoints

### Notifications
- `POST /api/Notifications/send` – Send a notification (manual trigger)
- `GET /api/Notifications/user/{userId}` – Get user notifications

### Templates
- `GET /api/Templates` – List templates
- `POST /api/Templates` – Create template
- `PUT /api/Templates/{id}` – Update template
- `DELETE /api/Templates/{id}` – Delete template

### Settings
- `GET /api/Settings` – Get notification settings
- `PUT /api/Settings` – Update settings (enable/disable email/SMS, reminder timings)

---

## Usage

1. **Configure Email & SMS**
   - Update `EmailSettings` in `appsettings.json`:
     ```json
     "EmailSettings": {
         "SmtpServer": "smtp.gmail.com",
         "SmtpPort": 587,
         "Username": "your_email@gmail.com",
         "Password": "your_email_password",
         "DisplayName": "EventHub Notifications"
     }
     ```
   - Update `TwilioSettings` if using SMS

2. **Create Notification Templates** in DB or via API

3. **Send Notification**
   - Call `POST /api/Notifications/send` with JSON payload:
     ```json
     {
       "UserId": 1,
       "EventId": 10,
       "Type": "Email",
       "Title": "Event Reminder",
       "Message": "Hi {{UserName}}, your event {{EventName}} starts at {{StartTime}}",
       "Email": "user@example.com",
       "Phone": "+911234567890",
       "TemplateId": 1,
       "Placeholders": {
         "UserName": "Alice",
         "EventName": "Level Up Workshop",
         "StartTime": "10:00 AM"
       }
     }
     ```

4. **Background Reminders**
   - Hosted service automatically sends 24-hour and 1-hour reminders
   - Check logs to see reminders being triggered

---

## Logging
- Logs when notifications are:
  - Scheduled
  - Sent successfully
  - Failed

---

## Architecture
- Clean Architecture: Domain / Application / Infrastructure / API
- Repository Pattern for all DB operations
- Hosted service for scheduled reminders
- Notification service handles multi-channel notifications and template rendering

---

## Tech Stack
- .NET 8 Web API
- Entity Framework Core
- SQL Server
- Twilio (SMS)
- SMTP Email
- Swagger for API documentation

---

## Demo
1. Book an event via API.
2. Check in-app notification saved in DB.
3. Email and SMS (if enabled) are sent immediately.
4. Scheduled reminders automatically trigger according to configured timings.

---

## Author
- Pragadeswaran
- National Institute of Technology Puducherry, B.Tech EEE 2025
