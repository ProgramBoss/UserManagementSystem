# User Management System

A modern, full-stack user management system built with .NET 8, Blazor Server, and SQLite. This application provides a comprehensive solution for managing users, groups, and permissions with a professional dark-themed UI.

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ?? Table of Contents

- [Overview](#overview)
- [Solution Structure](#solution-structure)
- [Key Technical Decisions](#key-technical-decisions)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [How to Run Locally](#how-to-run-locally)
- [How to Test](#how-to-test)
- [Features](#features)
- [Architecture](#architecture)
- [Database](#database)
- [API Documentation](#api-documentation)
- [Deployment](#deployment)
- [Troubleshooting](#troubleshooting)

---

## ?? Overview

The User Management System is an enterprise-grade application that demonstrates modern software development practices with .NET 8. It features a clean architecture, RESTful API, and responsive Blazor UI.

**Key Capabilities:**
- ? Full CRUD operations for users
- ? Group-based user organization
- ? Permission management system
- ? RESTful API with Swagger documentation
- ? Modern dark-themed responsive UI
- ? SQLite database (portable, no installation required)
- ? Entity Framework Core with Code-First approach
- ? Comprehensive error handling and logging

---

## ?? Solution Structure

```
UserManagementSystem/
?
??? UserManagementSystem.Api/          # Backend API Project
?   ??? Controllers/                   # API Controllers
?   ?   ??? UsersController.cs        # User management endpoints
?   ?   ??? GroupsController.cs       # Group management endpoints
?   ?
?   ??? Data/                         # Data Access Layer
?   ?   ??? ApplicationDbContext.cs   # EF Core DbContext
?   ?
?   ??? DTOs/                         # Data Transfer Objects
?   ?   ??? UserDto.cs               # User response model
?   ?   ??? CreateUserDto.cs         # User creation model
?   ?   ??? UpdateUserDto.cs         # User update model
?   ?   ??? GroupDto.cs              # Group response model
?   ?
?   ??? Models/                       # Domain Models
?   ?   ??? User.cs                  # User entity
?   ?   ??? Group.cs                 # Group entity
?   ?   ??? Permission.cs            # Permission entity
?   ?   ??? UserGroup.cs             # User-Group join table
?   ?   ??? GroupPermission.cs       # Group-Permission join table
?   ?
?   ??? Repositories/                 # Repository Pattern
?   ?   ??? IUserRepository.cs       # User repository interface
?   ?   ??? UserRepository.cs        # User repository implementation
?   ?   ??? IGroupRepository.cs      # Group repository interface
?   ?   ??? GroupRepository.cs       # Group repository implementation
?   ?
?   ??? Services/                     # Business Logic Layer
?   ?   ??? IUserService.cs          # User service interface
?   ?   ??? UserService.cs           # User service implementation
?   ?
?   ??? Migrations/                   # EF Core Migrations
?   ??? appsettings.json             # Configuration file
?   ??? Program.cs                    # Application entry point
?
??? UserManagementSystem.Web/         # Frontend Blazor Project
?   ??? Pages/                        # Blazor Pages/Components
?   ?   ??? Index.razor              # Home page
?   ?   ??? Users.razor              # User management page
?   ?   ??? Dashboard.razor          # Statistics dashboard
?   ?
?   ??? Shared/                       # Shared Components
?   ?   ??? MainLayout.razor         # Main layout
?   ?   ??? NavMenu.razor            # Navigation menu
?   ?
?   ??? Services/                     # API Client Services
?   ?   ??? IUserApiService.cs       # User API client interface
?   ?   ??? UserApiService.cs        # User API client
?   ?   ??? IGroupApiService.cs      # Group API client interface
?   ?   ??? GroupApiService.cs       # Group API client
?   ?
?   ??? Models/                       # Client-side DTOs
?   ?   ??? UserDto.cs               # User model
?   ?   ??? GroupDto.cs              # Group model
?   ?
?   ??? wwwroot/css/                  # Static Assets
?   ?   ??? site.css                 # Custom dark theme styles
?   ?
?   ??? appsettings.json             # Configuration
?   ??? Program.cs                    # Application entry point
?
??? UserManagementSystem.sln         # Solution file
```

---

## ?? Key Technical Decisions

### 1. **Architecture: Clean Architecture with Repository Pattern**

**Why this decision?**
- **Separation of Concerns:** Each layer has a single, well-defined responsibility
- **Maintainability:** Changes in one layer don't ripple through the entire application
- **Testability:** Easy to mock dependencies and write unit tests
- **Scalability:** Can replace implementations without affecting other layers

**Architecture Layers:**
```
???????????????????????????????????????
?   Presentation (Blazor UI)          ?
???????????????????????????????????????
?   API Layer (Controllers)           ?
???????????????????????????????????????
?   Service Layer (Business Logic)    ?
???????????????????????????????????????
?   Repository (Data Access)          ?
???????????????????????????????????????
?   Database (SQLite)                 ?
???????????????????????????????????????
```

---

### 2. **Database: SQLite**

**Why SQLite over SQL Server LocalDB?**

? **Advantages:**
- **Zero Configuration:** No database server installation required
- **Portability:** Single `.db` file that can be copied anywhere
- **Cross-Platform:** Works on Windows, Linux, and macOS
- **Perfect for Development:** Instant setup, no configuration
- **Docker Friendly:** Easy to containerize
- **File-Based Backup:** Just copy the `.db` file

? **Trade-offs:**
- Limited concurrent writes (single writer)
- Not suitable for extremely high-traffic applications
- Some advanced SQL Server features not available

**When to migrate:** If you need high concurrency or advanced features, you can easily migrate to PostgreSQL or SQL Server by changing the connection string.

---

### 3. **Frontend: Blazor Server**

**Why Blazor Server over WebAssembly or MVC?**

? **Advantages:**
- **Full .NET Access:** Use any .NET library server-side
- **Real-time Updates:** SignalR for instant UI updates
- **Smaller Download:** No need to download .NET runtime
- **Better Performance:** Rendering happens on server
- **Single Language:** C# for both frontend and backend

? **Trade-offs:**
- Requires persistent SignalR connection
- Higher server resource usage (but acceptable for internal apps)

---

### 4. **API Design: RESTful with Swagger**

**Why REST API?**
- **Industry Standard:** Universally understood
- **Self-Documenting:** Swagger provides interactive documentation
- **Testable:** Easy to test with Postman or Swagger UI
- **Language Agnostic:** Can be consumed by any client

**Endpoint Conventions:**
```
GET    /api/users          ? Get all users
GET    /api/users/{id}     ? Get specific user
POST   /api/users          ? Create user
PUT    /api/users/{id}     ? Update user
DELETE /api/users/{id}     ? Delete user
```

---

### 5. **Entity Framework Core (Code-First)**

**Why Code-First over Database-First?**
- **Version Control:** Database schema in source control
- **Type Safety:** Strongly-typed LINQ queries
- **Migrations:** Easy database versioning
- **Cross-Platform:** Works with multiple databases

---

### 6. **Dependency Injection**

**Why use DI throughout?**
- **Loose Coupling:** Components depend on abstractions, not concrete implementations
- **Testability:** Easy to mock dependencies for unit tests
- **Lifetime Management:** Automatic resource cleanup

**Lifetime Scopes Used:**
```csharp
// Scoped - New instance per HTTP request
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Singleton - Single instance for app lifetime  
builder.Services.AddHttpClient<IUserApiService, UserApiService>();
```

---

### 7. **Dark Gray Theme**

**Why dark theme?**
- **Modern Aesthetic:** Professional and contemporary look
- **Eye Comfort:** Reduces eye strain for users
- **Focus:** Helps users focus on content
- **Differentiation:** Stands out from default templates

**Color Palette:**
- Background: `#1a1a1a` (dark gray)
- Cards: `#2d2d2d` (medium gray)
- Accent: `#4a9eff` (blue)
- Text: `#e0e0e0` (light gray)

---

## ?? Prerequisites

### Required:
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (8.0 or later)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or **[VS Code](https://code.visualstudio.com/)**

### Optional Tools:
- **[Git](https://git-scm.com/)** - Version control
- **[DB Browser for SQLite](https://sqlitebrowser.org/)** - Database viewer
- **[Postman](https://www.postman.com/)** - API testing

### Verify Installation:
```bash
dotnet --version
# Should output: 8.0.x or higher
```

---

## ?? Getting Started

### 1. Clone Repository
```bash
git clone <repository-url>
cd UserManagementSystem
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build Solution
```bash
dotnet build
```

---

## ?? How to Run Locally

### Method 1: Visual Studio (Recommended)

1. **Open Solution**
   - Double-click `UserManagementSystem.sln`

2. **Set Multiple Startup Projects**
   - Right-click solution ? Properties
   - Select "Multiple startup projects"
   - Set both to **Start**:
     - ? UserManagementSystem.Api
     - ? UserManagementSystem.Web

3. **Run Application**
   - Press **F5** (Start Debugging)
   - Both projects will start

4. **Access Application**
   - **Web UI:** https://localhost:7001
   - **API Swagger:** https://localhost:7000/swagger
   - **Database:** `UserManagementSystem.Api/usermanagement.db`

---

### Method 2: Command Line

**Terminal 1 - API:**
```bash
cd UserManagementSystem.Api
dotnet run
```

**Terminal 2 - Web UI:**
```bash
cd UserManagementSystem.Web
dotnet run
```

**Access:**
- Web UI: https://localhost:7001
- API: https://localhost:7000/swagger

---

### Method 3: Visual Studio Code

1. **Open Folder**
   ```bash
   code .
   ```

2. **Install Extensions**
   - C# (Microsoft)
   - C# Dev Kit

3. **Run API** (Terminal 1)
   ```bash
   dotnet run --project UserManagementSystem.Api
   ```

4. **Run Web** (Terminal 2)
   ```bash
   dotnet run --project UserManagementSystem.Web
   ```

---

## ?? How to Test

### 1. Manual Testing via Web UI

**Create User:**
1. Navigate to https://localhost:7001/users
2. Click "Add New User"
3. Fill form:
   - First Name: John
   - Last Name: Doe
   - Email: john.doe@example.com
   - Select groups: Admin, Level 1
4. Click "Create"

**View Dashboard:**
- Go to https://localhost:7001/dashboard
- View statistics

**Edit/Delete User:**
- Click Edit or Delete buttons
- Confirm changes

---

### 2. API Testing via Swagger

1. **Open Swagger UI**
   - Navigate to: https://localhost:7000/swagger

2. **Test Endpoints:**

**GET All Users:**
```
GET /api/users
Click "Try it out" ? "Execute"
```

**POST Create User:**
```json
POST /api/users
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phoneNumber": "555-0200",
  "groupIds": [1, 2]
}
```

**PUT Update User:**
```json
PUT /api/users/1
{
  "firstName": "Jane",
  "lastName": "Smith-Updated",
  "email": "jane.smith@example.com",
  "isActive": true,
  "groupIds": [1, 3]
}
```

**DELETE User:**
```
DELETE /api/users/1
```

---

### 3. Database Inspection

**Using DB Browser for SQLite:**

1. Open `UserManagementSystem.Api/usermanagement.db`
2. View tables:
   - Users
   - Groups
   - Permissions
   - UserGroups
   - GroupPermissions

**Sample Queries:**
```sql
-- View all users with groups
SELECT u.FirstName, u.LastName, g.Name as GroupName
FROM Users u
LEFT JOIN UserGroups ug ON u.Id = ug.UserId
LEFT JOIN Groups g ON ug.GroupId = g.Id;

-- Count users per group
SELECT g.Name, COUNT(ug.UserId) as UserCount
FROM Groups g
LEFT JOIN UserGroups ug ON g.Id = ug.GroupId
GROUP BY g.Name;
```

---

## ? Features

### Core Features
- ? **User CRUD Operations** - Create, Read, Update, Delete
- ? **Group Management** - Organize users into groups
- ? **Permission System** - Role-based access control
- ? **RESTful API with Swagger** - Interactive API documentation
- ? **Responsive Design** - Works on mobile and desktop
- ? **Search & Filter** - Find users quickly
- ? **Dark Theme** - Professional gray color scheme
- ? **Real-time Updates** - SignalR for instant UI updates

### UI Features
- ? **Modal Dialogs** - User-friendly forms
- ? **Loading States** - Visual feedback
- ? **Error Handling** - Friendly error messages
- ? **Confirmation Dialogs** - Prevent accidental deletions

### API Features
- ? **RESTful Design** - Standard HTTP methods
- ? **Swagger Documentation** - Interactive API docs
- ? **CORS Enabled** - Cross-origin requests
- ? **Logging** - Comprehensive logging
- ? **Error Responses** - Structured error messages

---

## ??? Architecture

### Request Flow

```
????????????????
? User Browser ?
????????????????
       ? HTTPS
????????????????????
? Blazor Server UI ? (Port 7001)
????????????????????
       ? SignalR
???????????????????????
? UserApiService      ?
???????????????????????
       ? HTTP REST
??????????????????
? API Controller ? (Port 7000)
??????????????????
       ?
?????????????????
? UserService   ? (Business Logic)
?????????????????
       ?
????????????????????
? UserRepository   ? (Data Access)
????????????????????
       ?
???????????????????
? EF Core Context ?
???????????????????
       ?
?????????????????
? SQLite DB     ?
?????????????????
```

---

## ??? Database

### Schema Overview

**Tables:**
- `Users` - User information
- `Groups` - Group definitions
- `Permissions` - Available permissions
- `UserGroups` - User-Group relationships (many-to-many)
- `GroupPermissions` - Group-Permission relationships (many-to-many)

### Seed Data

**4 Pre-configured Groups:**
1. **Admin** - Full system access
2. **Level 1** - Basic access
3. **Level 2** - Intermediate access
4. **Manager** - Management privileges

**8 Pre-configured Permissions:**
1. Create
2. Read
3. Update
4. Delete
5. ManageUsers
6. ManageGroups
7. ViewReports
8. ManageSystem

---

## ?? API Documentation

### Base URL
```
https://localhost:7000/api
```

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users` | Get all users |
| GET | `/users/{id}` | Get user by ID |
| POST | `/users` | Create user |
| PUT | `/users/{id}` | Update user |
| DELETE | `/users/{id}` | Delete user |
| GET | `/users/count` | Get total users |
| GET | `/groups` | Get all groups |

### Example Requests

**Create User:**
```json
POST /api/users
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "groupIds": [1, 2]
}
```

**Response:**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "isActive": true,
  "groups": [...]
}
```

---

## ?? Deployment

### Docker Deployment

```dockerfile
# API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY published-api .
ENTRYPOINT ["dotnet", "UserManagementSystem.Api.dll"]
```

### Publish Commands

```bash
# Publish API
dotnet publish UserManagementSystem.Api -c Release -o ./publish/api

# Publish Web
dotnet publish UserManagementSystem.Web -c Release -o ./publish/web
```

---

## ?? Troubleshooting

### Common Issues

**1. Database Not Created**
```bash
cd UserManagementSystem.Api
dotnet ef database update
```

**2. Port Already in Use**
- Change ports in `launchSettings.json`

**3. API Not Connecting**
- Verify API is running
- Check `appsettings.json` BaseUrl setting

**4. Dark Theme Not Loading**
- Hard refresh: `Ctrl + Shift + R`
- Clear browser cache

---

## ?? License

MIT License - Free to use and modify

---

## ????? Support

For issues or questions:
- Review this README
- Check Troubleshooting section
- Open an issue on GitHub

---

**Built with ?? using .NET 8 and Blazor**
