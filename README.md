# Sathtern Internship Portal

A web application for managing student internships end to end: students register,
browse and search open positions, apply, and track the outcome of their
applications, while an admin view posts new internships and approves or rejects
what comes in.

## Features

**For students**

- Register with name, email, phone and university
- Browse every open internship, or search across title, company and location
- Open an internship to see the full description and apply
- Track applications and their status, and withdraw one if you change your mind

**For administrators**

- Dashboard totals for students, internships and applications, broken down by
  application status
- Post and delete internships
- Review registered students
- Filter applications by status and approve or reject them

## Stack

| Layer | Technology |
| --- | --- |
| Backend | ASP.NET Core 8 Web API |
| Data access | Entity Framework Core 8 |
| Database | MySQL 8, via Pomelo.EntityFrameworkCore.MySql 8.0.2 |
| Frontend | Angular 22 — standalone components, signals, reactive forms |
| API docs | Swagger / OpenAPI |

The backend is layered as Controllers → Services → Repositories → EF Core, with
controllers returning DTOs rather than entities and a `ServiceResult` type that
maps service outcomes onto HTTP status codes (404, 409, and so on).

## Screenshots

<!--
Add screenshots here. Suggested shots:
  - Internship listings with the search box in use
  - Internship detail with the apply form
  - My Applications showing status badges
  - Admin dashboard stat tiles and the applications tab
-->

_Screenshots to be added._

## Running it locally

### Requirements

- .NET 8 SDK
- Node.js 20+ and npm
- Angular CLI (`npm install -g @angular/cli`)
- MySQL 8

### 1. Database

Create the database:

```sql
CREATE DATABASE sathtern_internship_portal
  CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
```

Then point the app at it. `backend/appsettings.json` is tracked by git and ships
with a `CHANGE_ME` placeholder — **keep it that way** and put your real
credentials somewhere untracked instead:

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=sathtern_internship_portal;User=root;Password=<your password>;"
```

`backend/appsettings.Development.json` (git-ignored) works too.

Apply the schema:

```bash
cd backend
dotnet ef database update
```

That creates `Students`, `Internships` and `Applications`, with a unique index on
`Student.Email` and a composite unique index on `(StudentId, InternshipId)` so a
student cannot apply to the same internship twice.

If `dotnet ef` is missing: `dotnet tool install --global dotnet-ef --version 8.*`

Note that `Program.cs` pins the server version with
`new MySqlServerVersion(new Version(8, 4, 9))`. Adjust it if your MySQL differs.

### 2. Backend

```bash
cd backend
dotnet run
```

Runs on **http://localhost:5047**, with Swagger UI at
**http://localhost:5047/swagger**.

### 3. Frontend

```bash
cd frontend
npm install
ng serve
```

Runs on **http://localhost:4200**, which is the origin the backend's CORS policy
allows. The API base URL lives in `src/environments/environment.development.ts`.

### Pages

| Route | Purpose |
| --- | --- |
| `/internships` | Listings with search (default route) |
| `/internships/:id` | Full details and the apply form |
| `/register` | Student registration |
| `/my-applications` | Applications for one student, with withdraw |
| `/admin` | Dashboard, and management of internships, students and applications |

## API

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/Students` | All students |
| GET | `/api/Students/{id}` | One student |
| GET | `/api/Students/by-email?email=` | Look up a student by email |
| POST | `/api/Students` | Register |
| PUT | `/api/Students/{id}` | Update |
| DELETE | `/api/Students/{id}` | Delete |
| GET | `/api/Internships?search=` | List, optionally filtered |
| GET | `/api/Internships/{id}` | One internship |
| POST | `/api/Internships` | Create |
| PUT | `/api/Internships/{id}` | Update |
| DELETE | `/api/Internships/{id}` | Delete |
| GET | `/api/Applications?status=` | List, optionally filtered by status |
| GET | `/api/Applications/{id}` | One application |
| GET | `/api/Applications/student/{studentId}` | A student's applications |
| GET | `/api/Applications/internship/{internshipId}` | An internship's applicants |
| POST | `/api/Applications` | Apply |
| PUT | `/api/Applications/{id}/status` | Approve or reject |
| DELETE | `/api/Applications/{id}` | Withdraw |
| GET | `/api/Admin/dashboard` | Totals and status breakdown |

`ApplicationStatus` crosses the wire as a string — `"Pending"`, `"Accepted"` or
`"Rejected"`.

## Tests

```bash
cd frontend
ng test        # Vitest + jsdom
```

Coverage is currently limited to the application shell; see the limitations
below.

## Known limitations

- **No authentication or authorization.** This is the significant one. There is
  no sign-in of any kind, and **the admin dashboard at `/admin` is completely
  open** — anyone who can reach the URL can post internships, delete them, and
  approve or reject applications. Do not deploy this as-is.
- **Students identify themselves by email.** Because there are no sessions,
  applying and viewing your applications means typing the email you registered
  with. Anyone who knows an address can view or withdraw that person's
  applications.
- **Thin test coverage.** Only the app shell has specs. The forms, the debounced
  search, the admin tabs and the confirmation flows are unit-tested nowhere;
  they were verified by hand against the running API.
- **No pagination.** Listings, the student table and the application list all
  fetch everything. Fine at demo size, not beyond it.
- **No file uploads.** There is no CV or cover letter attachment.
- **Delete is permanent.** Removing a student or internship cascades to their
  applications; nothing is soft-deleted or recoverable.
- **No email notifications.** An applicant is not told when their status changes.

## Repository layout

```
Sathtern_InternshipPortal/
├── backend/
│   ├── Controllers/     API endpoints, with shared ServiceResult → HTTP mapping
│   ├── Services/        Business rules and validation
│   ├── Repositories/    Generic repository plus per-entity queries
│   ├── Models/          Entities and DTOs
│   ├── Mapping/         Entity → DTO projections
│   ├── Data/            AppDbContext
│   └── Migrations/      EF Core migrations
└── frontend/
    └── src/app/
        ├── components/  Registration, listings, detail, my applications, admin
        ├── services/    Typed HttpClient wrappers per resource
        └── models/      TypeScript interfaces mirroring the API DTOs
```
