# Hotel Management API

ASP.NET Core Web API with JWT Authentication for Hotel Management System.

## Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or SQL Server Express)
- Existing database: `ClientCalendarDB`

## Database Setup

The API connects to an existing database `ClientCalendarDB` with the following tables:
- `Clients`
- `CalendarEntries`
- `Users` (needs to be created)

### Create Users Table

Run the SQL script to create the Users table:

```bash
# Navigate to the Database folder and run the SQL script
sqlcmd -S localhost -i Database/CreateUsersTable.sql
```

Or execute the script manually in SQL Server Management Studio.

## Configuration

Update `appsettings.json` with your database connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClientCalendarDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Running the Application

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Authentication

#### POST /api/auth/login
Login with username and password.

**Request:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@example.com",
  "expiresAt": "2026-01-31T12:00:00Z"
}
```

#### POST /api/auth/register
Register a new user.

**Request:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "newuser",
  "email": "user@example.com",
  "expiresAt": "2026-01-31T12:00:00Z"
}
```

## Default Credentials

- **Username:** admin
- **Password:** admin123

## JWT Configuration

The JWT token is configured with:
- **Secret Key:** Configured in `appsettings.json`
- **Issuer:** HotelManagementAPI
- **Audience:** HotelManagementClient
- **Expiration:** 24 hours

## CORS Configuration

CORS is configured to allow requests from:
- `http://localhost:4200` (Angular frontend)

## Project Structure

```
backend/
├── Controllers/
│   └── AuthController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Database/
│   └── CreateUsersTable.sql
├── DTOs/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   └── RegisterRequest.cs
├── Models/
│   ├── Client.cs
│   ├── CalendarEntry.cs
│   └── User.cs
├── Services/
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Program.cs
└── appsettings.json
```

## Security Notes

⚠️ **Important:** 
- Change the JWT secret key in production
- Use a stronger password hashing algorithm (e.g., BCrypt) in production
- The current implementation uses SHA256 for simplicity
- Enable HTTPS in production
- Update CORS policy for production domains

