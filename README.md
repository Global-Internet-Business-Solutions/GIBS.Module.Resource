# GIBS Resource Manager Module for Oqtane

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)
![Oqtane](https://img.shields.io/badge/Oqtane-10.2.1-green.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

A comprehensive resource booking and management module for Oqtane Framework. Designed for managing reservable resources such as meeting rooms, equipment, facilities, vehicles, or any other bookable assets.

## 📋 Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Configuration](#configuration)
- [User Guide](#user-guide)
- [Administration](#administration)
- [Notifications](#notifications)
- [Technical Details](#technical-details)
- [License](#license)

## ✨ Features

### Resource Management
- **Multiple Resource Types**: Support for different resource categories (rooms, equipment, vehicles, etc.)
- **Resource Details**: Name, description, type, maximum capacity
- **Active/Inactive Status**: Enable or disable resources without deletion
- **Buffer Times**: Configure buffer time before and after reservations
- **Weekly Availability Schedules**: Set availability by day of week and time slots

### Reservation System
- **User Reservations**: Users can view available resources and make bookings
- **Reservation Status Management**:
  - Pending (awaiting approval)
  - Confirmed (approved and active)
  - Cancelled (user or admin cancelled)
- **Configurable Default Status**: Set whether new reservations are automatically confirmed or require approval
- **Notes Field**: Add custom notes to reservations
- **Conflict Prevention**: Automatic detection and prevention of overlapping bookings
- **Buffer Time Enforcement**: Respects configured buffer times between reservations

### User Interface

#### Public Pages
- **Resource List**: Browse all active resources grouped by type with capacity and availability information
- **Resource Details**: View individual resource information and weekly availability schedule
- **Reserve/Booking Calendar**: Interactive calendar showing available time slots
- **My Bookings**: Personal dashboard for users to view, edit, and cancel their own reservations

#### Administrative Pages
- **BackOffice Dashboard**: Centralized resource and reservation management
- **Resource Management**: Create, edit, and manage all resources
- **Availability Management**: Configure weekly availability schedules for each resource
- **Reservation Management**: View and manage all reservations with status updates
- **Manage All Reservations**: Comprehensive view of all bookings across all resources
- **Print Schedule**: Printable daily and weekly schedules for planning and reporting

### 📧 Notification System

#### Email Notifications
- **New Reservation Confirmation**: Automatic email when a reservation is created
  - Confirmed status: "Your Reservation Has Been Confirmed!"
  - Pending status: "Your Reservation Has Been Received!"
- **Status Change Notifications**: Email when reservation changes from Pending to Confirmed
- **HTML Formatted Emails**: Professional-looking email templates with all booking details
- **User Details**: Includes user name, resource, date/time, status, and notes

#### SMS Notifications (Twilio Integration)
- **Real-time SMS Alerts**: Send SMS when new reservations are created
- **Twilio Configuration**: Integrated Twilio SMS service
- **Configurable Recipients**: Set default phone number for notifications
- **Enable/Disable Toggle**: Turn SMS notifications on/off as needed

## 🚀 Installation

### Requirements
- Oqtane Framework 10.2.1 or higher
- .NET 10.0
- SQL Server, MySQL, PostgreSQL, or SQLite database

### Installation Steps

1. **Download the Module**
   ```
   Download the latest release package (GIBS.Module.Resource.nupkg)
   ```

2. **Install via Oqtane Admin**
   - Log in to your Oqtane site as an administrator
   - Navigate to **Admin Dashboard** → **Module Management**
   - Click **Install Module**
   - Upload the `.nupkg` file
   - Follow the installation wizard

3. **Add Module to a Page**
   - Navigate to the page where you want to add the resource manager
   - Click **Add Module**
   - Select **Resource** from the module list
   - Configure the module settings

## ⚙️ Configuration

### Module Settings

Access module settings through the module's **Settings** option.

#### General Settings

| Setting | Description | Default |
|---------|-------------|---------|
| **Custom Title** | Custom title displayed on the booking calendar | "Booking Calendar" |
| **Default Status** | Default status for new reservations (Pending or Confirmed) | Pending |
| **Cancellation Policy** | Text describing your cancellation policy | (empty) |
| **Email Notification** | Email address to receive booking notifications | (empty) |

#### Twilio SMS Settings

Configure Twilio integration for SMS notifications:

| Setting | Description | Required |
|---------|-------------|----------|
| **Enable Twilio** | Toggle SMS notifications on/off | No |
| **Account SID** | Your Twilio Account SID | Yes (if enabled) |
| **Auth Token** | Your Twilio Auth Token | Yes (if enabled) |
| **From Phone Number** | Twilio phone number to send from | Yes (if enabled) |
| **Webhook URL** | URL reserved for future webhook use | Optional |
| **Send To Number** | Default recipient phone number for SMS alerts | Yes (if enabled) |

### Email Configuration

Email notifications use Oqtane's built-in notification system. Configure SMTP settings in:
- **Oqtane Admin** → **Site Settings** → **SMTP Configuration**

## 📖 User Guide

### Making a Reservation

1. **Browse Resources**
   - Navigate to the **Resource List** page
   - Resources are grouped by type
   - View description, capacity, and status

2. **Check Availability**
   - Click on a resource to view details
   - Review the weekly availability schedule
   - See buffer times and constraints

3. **Book a Time Slot**
   - Click **Reserve** or navigate to the booking calendar
   - Select a date
   - Available time slots are shown (booked slots are disabled)
   - Click on an available time slot
   - Add optional notes
   - Submit your reservation

4. **Manage Your Bookings**
   - Navigate to **My Bookings**
   - View all your current and past reservations
   - Edit reservation notes
   - Cancel reservations if needed

### Understanding Reservation Status

- 🟡 **Pending**: Your reservation is awaiting approval
- 🟢 **Confirmed**: Your reservation is approved and active
- 🔴 **Cancelled**: Reservation has been cancelled

## 🔧 Administration

### Setting Up Resources

1. **Navigate to BackOffice**
   - Click on the **BackOffice** link in the module

2. **Create a Resource**
   - Click **Add Resource**
   - Fill in the details:
     - **Name**: Resource identifier (e.g., "Conference Room A")
     - **Description**: Detailed description
     - **Resource Type**: Category (e.g., "Meeting Room", "Equipment")
     - **Maximum Capacity**: Number of people/items
     - **Is Active**: Enable/disable the resource
     - **Buffer Before**: Minutes of buffer time before each booking
     - **Buffer After**: Minutes of buffer time after each booking
   - Click **Save**

3. **Configure Availability**
   - Select a resource
   - Click **Manage Availability**
   - Add availability slots by day of week:
     - Monday: 9:00 AM - 5:00 PM
     - Tuesday: 9:00 AM - 5:00 PM
     - etc.
   - Create multiple slots per day if needed (e.g., split morning/afternoon)

### Managing Reservations

#### Reservation Management Page
- View all reservations for selected resources
- Filter by resource
- Update reservation status
- Edit reservation details
- Add or modify notes
- Delete reservations

#### Manage All Reservations Page
- Comprehensive view across all resources
- Sort and filter capabilities
- Quick status updates
- Bulk management features

### Printing Schedules

The **Print Schedule** feature provides professional printouts:

#### Daily View
- Select a specific date
- Choose "All Resources" or specific resource
- Includes all bookings for that day
- Shows reservation notes

#### Weekly View
- Select a specific resource
- Shows entire week of bookings
- Includes user names and notes
- Perfect for facility planning

**To Print:**
1. Navigate to **Print Schedule**
2. Select view type (Daily/Weekly)
3. Choose date and resource(s)
4. Click **Load Schedule**
5. Use browser print (Ctrl+P or Cmd+P)

## 📬 Notifications

### Email Notifications

Emails are automatically sent in these scenarios:

#### New Reservation (Confirmed Status)
```
Subject: Reservation Confirmed - [Resource Name]
Body: 
Your Reservation Has Been Confirmed!
Hello [User Name],
Your reservation has been confirmed with the following details:
• Resource: [Resource Name]
• When: [Date/Time]
• Status: Confirmed
• Notes: [Optional Notes]
```

#### New Reservation (Pending Status)
```
Subject: Reservation Received - [Resource Name]
Body:
Your Reservation Has Been Received!
Hello [User Name],
Your reservation has been received and is pending confirmation. Details:
• Resource: [Resource Name]
• When: [Date/Time]
• Status: Pending
• Notes: [Optional Notes]
```

#### Status Change (Pending → Confirmed)
```
Subject: Reservation Confirmed - [Resource Name]
Body:
Your Reservation Has Been Confirmed!
Hello [User Name],
Your reservation status has been updated to confirmed. Details:
[booking details]
```

### SMS Notifications (Twilio)

When enabled, SMS messages are sent for new reservations:

```
New Reservation Booked!
Resource: [Resource Name]
User: [User Name]
When: [Date/Time]
Status: [Status]
```

**Note**: SMS notifications are sent to the configured "Send To Number" (typically an administrator), not to the end user.

## 🛠️ Technical Details

### Database Schema

#### GIBSResource Table
- `ResourceId` (Primary Key)
- `ModuleId` (Foreign Key to Oqtane Module)
- `Name`, `Description`, `ResourceType`
- `IsActive`, `MaxCapacity`
- `BufferBeforeMinutes`, `BufferAfterMinutes`

#### GIBSResourceAvailability Table
- `AvailabilityId` (Primary Key)
- `ResourceId` (Foreign Key)
- `DayOfWeek` (0-6, Sunday-Saturday)
- `StartTime`, `EndTime` (TimeSpan)

#### GIBSReservation Table
- `ReservationId` (Primary Key)
- `ResourceId` (Foreign Key)
- `UserId` (Foreign Key to Oqtane User)
- `StartTime`, `EndTime` (DateTime)
- `Status` (Enum: Pending/Confirmed/Cancelled)
- `Notes` (string)

### Technologies Used

- **Framework**: Oqtane 10.2.1 on .NET 10.0
- **UI**: Blazor WebAssembly
- **Database**: Entity Framework Core
- **Notifications**: Oqtane Notification System
- **SMS**: Twilio SDK (optional)

### Architecture

- **Client-Server Model**: Blazor WebAssembly client with ASP.NET Core server
- **Repository Pattern**: Server-side data access
- **Service Layer**: Business logic separation
- **Dependency Injection**: Oqtane DI container
- **Modular Design**: Self-contained Oqtane module

### API Endpoints

- `/api/Resource` - Resource CRUD operations
- `/api/Reservation` - Reservation management
- `/api/ResourceAvailability` - Availability configuration

### Permissions

- **View**: Basic users can view resources and make reservations
- **Edit**: Administrators can manage resources and all reservations

## 🤝 Contributing

We welcome contributions! Areas for enhancement:

- Multi-language support
- Recurring reservations
- Calendar integrations (iCal, Google Calendar)
- Resource categories and filtering
- Advanced reporting
- Mobile app
- Payment integration

## 📄 License

This project is licensed under the MIT License.

```
MIT License

Copyright (c) GIBS

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 📞 Support

For issues, questions, or feature requests:
- GitHub Issues: [Report an issue](https://github.com/Global-Internet-Business-Solutions/GIBS.Module.Resource/issues)
- Documentation: This README
- Oqtane Community: [Oqtane Forums](https://github.com/oqtane/oqtane.framework/discussions)

---

**Built with ❤️ for the Oqtane Community**

*Version 1.0.0 - Compatible with Oqtane 10.2.1+*