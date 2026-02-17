# User Management System

A modern, full-stack user management system built with .NET 8, Blazor Server, and SQLite. This application provides a comprehensive solution for managing users, groups, and permissions with a professional dark-themed UI.

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Table of Contents

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
- [License](#license)
- [Support](#support)

---

## Overview

The User Management System is an enterprise-grade application demonstrating modern software development practices with .NET 8. It features a clean architecture, RESTful API, and responsive Blazor UI.

**Key Capabilities:**
- Full CRUD operations for users
- Group-based user organization
- Permission management system
- RESTful API with Swagger documentation
- Modern dark-themed responsive UI
- SQLite database (portable, no installation required)
- Entity Framework Core with Code-First approach
- Comprehensive error handling and logging

## Key Technical Decisions

### 1. Architecture: Clean Architecture with Repository Pattern

**Why this decision**
- Separation of Concerns: Each layer has a single, well-defined responsibility
- Maintainability: Changes in one layer don't ripple through the entire application
- Testability: Easy to mock dependencies and write unit tests
- Scalability: Can replace implementations without affecting other layers

**Architecture Layers:**

Presentation (Blazor UI)  
API Layer (Controllers)  
Service Layer (Business Logic)  
Repository (Data Access)  
Database (SQLite)  

---

### 2. Database: SQLite

**Why SQLite over SQL Server LocalDB**

**Advantages:**
- Zero Configuration: No database server installation required
- Portability: Single `.db` file that can be copied anywhere
- Cross-Platform: Works on Windows, Linux, and macOS
- Perfect for Development: Instant setup, no configuration
- Docker Friendly: Easy to containerize
- File-Based Backup: Just copy the `.db` file

**Trade-offs:**
- Limited concurrent writes (single writer)
- Not suitable for extremely high-traffic applications
- Some advanced SQL Server features not available

**When to migrate:** If you need high concurrency or advanced features, migrate to PostgreSQL or SQL Server by changing the connection string.

---

### 3. Frontend: Blazor Server

**Why Blazor Server over WebAssembly or MVC**

**Advantages:**
- Full .NET Access: Use any .NET library server-side
- Real-time Updates: SignalR for instant UI updates
- Smaller Download: No need to download .NET runtime
- Better Performance: Rendering happens on server
- Single Language: C# for both frontend and backend

**Trade-offs:**
- Requires persistent SignalR connection
- Higher server resource usage (acceptable for internal apps)

---

### 4. API Design: RESTful with Swagger

**Why REST API**
- Industry Standard: Universally understood
- Self-Documenting: Swagger provides interactive documentation
- Testable: Easy to test with Postman or Swagger UI
- Language Agnostic: Can be consumed by any client

**Endpoint Conventions:**

GET    /api/users           Get all users  
GET    /api/users/{id}      Get specific user  
POST   /api/users           Create user  
PUT    /api/users/{id}      Update user  
DELETE /api/users/{id}      Delete user  

---

### 5. Entity Framework Core (Code-First)

**Why Code-First over Database-First**
- Version Control: Database schema in source control
- Type Safety: Strongly-typed LINQ queries
- Migrations: Easy database versioning
- Cross-Platform: Works with multiple databases

---

### 6. Dependency Injection

**Why use DI throughout**
- Loose Coupling: Components depend on abstractions, not concrete implementations
- Testability: Easy to mock dependencies for unit tests
- Lifetime Management: Automatic resource cleanup

**Lifetime Scopes Used:**
```csharp
// Scoped - New instance per HTTP request
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Singleton - Single instance for app lifetime
builder.Services.AddHttpClient<IUserApiService, UserApiService>();
