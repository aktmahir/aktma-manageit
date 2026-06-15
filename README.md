# Calendar Management System

A C# .NET MVC web application for managing user calendars with event scheduling, categorization, and reminders.

## Features

- **User Management**: Create, read, update, and delete user profiles
- **Event Management**: Create and manage calendar events with detailed information
- **Event Categorization**: Organize events by categories (Work, Personal, Meeting, Birthday, Holiday, Other)
- **All-Day Events**: Support for all-day event tracking
- **Event Details**: Store location, description, reminders, and time information
- **Responsive UI**: Built with Bootstrap 5 for mobile and desktop compatibility
- **Database Integration**: SQL Server with Entity Framework Core
- **Authentication**: Cookie-based login with role claims

## Tech Stack

- **Framework**: ASP.NET Core MVC on .NET 8
- **Database**: SQL Server with Entity Framework Core 8
- **Frontend**: HTML5, Bootstrap 5, CSS
- **Language**: C#

## Project Structure

```
.
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── EventsController.cs
│   └── UsersController.cs
├── Models/              # Data Models
│   ├── User.cs
│   └── Event.cs
├── Views/               # Razor Views
│   ├── Home/
│   ├── Events/
│   ├── Users/
│   └── Shared/
├── Data/                # Database Context
│   └── CalendarDbContext.cs
├── Program.cs           # Application Entry Point
├── appsettings.json     # Configuration
└── CalendarApp.csproj   # Project File
```

## Prerequisites

- .NET 8 SDK or later
- Visual Studio 2022 or Visual Studio Code
- SQL Server LocalDB (or any SQL Server instance)

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd aktma-manageit
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Configure Database Connection
Edit `appsettings.json` and update the connection string if needed:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CalendarDb;Trusted_Connection=true;"
}
```

### 4. Create Database & Apply Migrations
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will start at `https://localhost:5001`

### 6. Sign In
Use one of the seeded demo accounts:

- Admin: `admin@example.com` / `Admin123!`
- Company owner: `john@example.com` / `John123!`
- Member: `jane@example.com` / `Jane123!`

## Models

### User Model
- **Id**: User identifier
- **Name**: User's full name
- **Email**: User's email address (unique)
- **CreatedAt**: Account creation timestamp
- **Events**: Collection of user's events

### Event Model
- **Id**: Event identifier
- **UserId**: Reference to the user
- **Title**: Event title
- **Description**: Event description
- **StartTime**: Event start date/time
- **EndTime**: Event end date/time
- **Location**: Event location
- **Category**: Event category (Work, Personal, Meeting, Birthday, Holiday, Other)
- **IsAllDay**: Boolean flag for all-day events
- **ReminderTime**: Reminder information
- **CreatedAt**: Event creation timestamp

## Usage

### Home Page
- View all events for the current user
- Quick access to event management features

### Users Management
- View all registered users
- Create new user accounts
- Edit user information
- Delete user accounts (cascades to events)

### Events Management
- Create new calendar events
- View event details
- Edit existing events
- Delete events
- Filter events by user

## Database

The application uses Entity Framework Core with SQL Server. The database schema includes:

- **Users Table**: Stores user information with email uniqueness constraint
- **Events Table**: Stores event details with foreign key relationship to Users

Initial seed data includes:
- 2 sample users (John Doe, Jane Smith)
- 2 sample events

## Navigation

- **Home**: `/` - Main calendar view
- **Events**: `/events` - Event management
- **Users**: `/users` - User management
- **Login**: `/Account/Login` - Sign in to the app

## Future Enhancements

- Event notifications and reminders
- Calendar view (month/week/day)
- Event search and filtering
- User authentication & authorization
- Event sharing between users
- Export events to iCalendar format
- API endpoints for mobile apps
- Unit and integration tests
Manage your time
