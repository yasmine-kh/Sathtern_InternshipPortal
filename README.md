# Sathtern Internship Portal

A web application for managing student internship listings and applications.

## Stack

- **Backend:** ASP.NET Core 8 (Web API), Entity Framework Core 8
- **Database:** MySQL, via Pomelo.EntityFrameworkCore.MySql 8.0.2
- **Frontend:** Angular (planned, not yet started)

## Current status

Backend skeleton with entities and DbContext in place; repositories and
services are not yet implemented.

What exists today:

- `Models/Entities` — `Student`, `Internship`, `Application`, `ApplicationStatus`
- `Data/AppDbContext` — DbSets for all three entities, a unique index on
  `Student.Email`, and a composite unique index on `(StudentId, InternshipId)`
  enforcing one application per student per internship
- `Program.cs` — MySQL connection, a CORS policy for `http://localhost:4200`,
  and Swagger

The `Controllers/`, `Repositories/` and `Services/` folders are empty
placeholders. No controllers are registered, so the API currently exposes no
endpoints. No EF Core migrations have been created yet.

## Requirements

- .NET 8 SDK
- A running MySQL server

## Running it

1. Set your MySQL connection string. Keep real credentials out of
   `appsettings.json`, which is tracked by git — put them in
   `appsettings.Development.json` (git-ignored) or use user secrets:

   ```bash
   cd backend
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=sathtern_internship_portal;User=root;Password=yourpassword;"
   ```

2. Build and run:

   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```

3. Swagger UI is served at `/swagger` in the Development environment.

Note: the server version is pinned to MySQL 8.0.36 in `Program.cs` rather than
auto-detected, so the app starts without a reachable database. Adjust it if
your server version differs.
