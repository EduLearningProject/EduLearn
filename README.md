# EduLearn v10.0 — University Learning Management & Student Information System

6 Microservices • 25 Entities • 9 Modules • 7 Roles • Enterprise Scope

---

## What Changed in the `Database` Branch

This branch establishes the complete database foundation for the project. Below is a summary of every change made.

### 1. Shared Enum Library (`EduLearn.Shared/Enums/`)

All magic strings previously scattered across entities, DTOs, and controllers have been replaced with strongly-typed C# enums. 9 new enum files were created:

| File | Enums Defined |
|------|--------------|
| `UserEnums.cs` | `UserRole`, `UserStatus` |
| `EnrollmentEnums.cs` | `EnrollmentStatus`, `StudentLifecycleStatus` |
| `CourseEnums.cs` | `CourseStatus` |
| `AdmissionsEnums.cs` | `ApplicationStatus`, `TranscriptStatus` |
| `AssessmentEnums.cs` | `AssessmentType`, `AssessmentStatus`, `SubmissionStatus` |
| `ContentEnums.cs` | `ContentType`, `ContentStatus`, `DiscussionStatus` |
| `FinanceEnums.cs` | `FeeScheduleStatus`, `InvoiceStatus`, `PaymentStatus`, `PaymentMethod`, `ScholarshipStatus` |
| `NotificationEnums.cs` | `NotificationCategory`, `NotificationSeverity`, `NotificationStatus`, `TicketStatus`, `TicketPriority` |
| `AnalyticsEnums.cs` | `ReportScope`, `ReportingPeriod` |

All enum values are stored in the database as **strings** (e.g., `"Enrolled"`, `"Active"`) via EF Core's `HasConversion<string>()` — never as integers.

### 2. Entities Updated (15 files)

All entity classes in `EduLearn.Shared/Entities/` now use the typed enums instead of raw strings for status, type, role, and category fields:

`User`, `Student`, `Applicant`, `Transcript`, `Enrollment`, `Course`, `Assessment`, `Submission`, `Content`, `Discussion`, `FeeSchedule`, `Invoice`, `Payment`, `Scholarship`, `Notification`, `Ticket`, `Report`, `KPI`

### 3. DTOs Updated (5 files)

DTOs now use enum types — regex validators for role/status strings have been removed:
- `CreateUserDto.cs` — `UserRole Role`
- `UpdateStatusDto.cs` — `UserStatus Status`
- `UserResponseDto.cs` — `UserRole Role`, `UserStatus Status`
- `EnrollmentResponseDto.cs` — `EnrollmentStatus Status`
- `CourseResponseDto.cs` — `CourseStatus Status`

### 4. DbContexts Updated (7 files)

All per-service DbContexts and `MigrationDbContext` now configure `HasConversion<string>().HasMaxLength(N)` for every enum property. This ensures EF Core stores and reads enums as nvarchar strings correctly.

Services updated: `AuthDbContext`, `SISDbContext`, `LMSDbContext`, `AnalyticsDbContext`, `NotificationDbContext`, `FinanceDbContext`, `MigrationDbContext`

### 5. Controller Updated (1 file)

`EnrollmentsController.cs` — all 9 string literal comparisons/assignments replaced with typed enum values (e.g., `EnrollmentStatus.Dropped`, `EnrollmentStatus.Waitlisted`).

### 6. Swagger Enum Display (6 Program.cs files)

All 6 service `Program.cs` files now:
- Serialize enums as strings in JSON responses via `JsonStringEnumConverter`
- Display enum string options as dropdowns in Swagger UI via `EnumSchemaFilter` + `UseInlineDefinitionsForEnums()`

This means Swagger shows `"Student" | "Instructor" | "Registrar" | ...` instead of `0 | 1 | 2 | ...`.

### 7. Database Migration: `FinalizeSchema`

A new migration `20260406101147_FinalizeSchema` was added to `EduLearn.DbMigrator/Migrations/`. It:
- Drops `RefreshToken` and `RefreshTokenExpiry` columns from the `Users` table
- Adjusts column lengths for `Tickets.Priority` (→ 20), `Reports.Scope` (→ 30), `Notifications.Severity` (→ 20), `KPIs.ReportingPeriod` (→ 20) to match actual max enum value lengths

---

## Quick Start (For Team Members)

### Prerequisites
- Visual Studio 2022 (ASP.NET + Data workloads)
- .NET 8 SDK
- SQL Server LocalDB (installed with VS 2022)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### First-Time Setup

1. Clone the repo:
   ```
   git clone <repo-url>
   cd EduLearn
   ```

2. Build the solution:
   ```
   dotnet build EduLearn.slnx
   ```

3. Create the database (run from solution root):
   ```
   .\migrate-database.bat
   ```
   **OR** run manually:
   ```
   dotnet ef database update --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator
   ```

4. Open `EduLearn.slnx` in Visual Studio 2022

5. Run any service:
   ```
   cd EduLearn.AuthService
   dotnet run
   ```
   Open: http://localhost:5001/swagger

### Pulling Updates From This Branch

If you already have a local database from a previous pull:

1. `git pull`
2. Run `.\migrate-database.bat` — EF Core will detect and apply any new migrations automatically

> **Important:** Skipping step 2 after a pull that contains new migrations will cause a runtime startup error.

### Resetting the Database

```sql
-- In SSMS, connect to (localdb)\MSSQLLocalDB, run on master:
USE master;
ALTER DATABASE EduLearnDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE EduLearnDb;
```
Then re-run `.\migrate-database.bat`.

---

## Architecture

6 ASP.NET Core 8 microservices + 1 migration project sharing a single SQL Server database (EduLearnDb):

| Service | Port | Modules | Tables Owned |
|---------|------|---------|-------------|
| AuthService | 5001 | IAM | Users, AuditLogs |
| SISService | 5002 | SRA, ETS | Students, Applicants, Transcripts, Sections, Enrollments, Rooms |
| LMSService | 5003 | CCM, LMS, AGI | Courses, Programs, Syllabi, Contents, Discussions, Assessments, Submissions, GradeChanges |
| AnalyticsService | 5004 | RKA | Reports, KPIs, AuditPackages |
| NotificationService | 5005 | NHT | Notifications, Tickets |
| FinanceService | 5006 | SFB | FeeSchedules, Invoices, Payments, Scholarships |
| **DbMigrator** | — | — | Creates ALL 25 tables (migration-only, not a runtime service) |

### Database Migration Strategy

EduLearn uses a **dedicated MigrationDbContext** pattern. A single `EduLearn.DbMigrator` project contains one `MigrationDbContext` that maps all 25 entities with all FK constraints. This creates the entire database schema in one atomic migration — zero FK ordering issues.

Individual services use their own DbContexts at runtime for queries, but **never run migrations**. Referenced tables use `ExcludeFromMigrations()` as a safety net.

**Why this pattern?** Multiple services with cross-service FK dependencies (e.g., LMS's Assessments table references SIS's Sections table) cannot be migrated independently without FK ordering failures. The single-migrator pattern is used by production frameworks like ABP and is recommended by Microsoft for shared-database architectures.

### Connection String

All services connect to: `Server=(localdb)\MSSQLLocalDB;Database=EduLearnDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true`

Update `ConnectionStrings.DefaultConnection` in each service's `appsettings.json` if using a different SQL Server instance.

---

## Team Assignment

| Member | Name | Service | Features |
|--------|------|---------|----------|
| M1 | Ashish | AuthService (:5001) | IAM-01 through IAM-04 |
| M2 | Saurav | SISService (:5002) | SRA-01 through SRA-03, ETS-01 through ETS-03 |
| M3 | Vikash | LMSService (:5003) | CCM-01 through CCM-03, LMS-01, LMS-02, AGI-01 through AGI-04 |
| M4 | Utkarsh | AnalyticsService (:5004) | RKA-01 through RKA-03 |
| M5 | Tanya | FinanceService (:5006) | SFB-01 through SFB-04 |
| M6 | Swarna Priyanshu | NotificationService (:5005) | NHT-01 through NHT-03 |

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Backend | ASP.NET Core 8.0 | REST APIs for 6 microservices |
| ORM | Entity Framework Core 8.0 | Code-first, LINQ queries |
| Database | SQL Server (LocalDB / Azure SQL) | 25 entity tables, single schema |
| Auth | JWT + BCrypt | Stateless auth with refresh token rotation |
| Real-Time | SignalR | WebSocket for live notifications |
| Testing | NUnit 3 + Moq | Unit + integration tests |
| Frontend | React 18 + TypeScript | SPA with role-based UI (post-interim) |

---

## Git Conventions

- Branch naming: `feature/<desc>`, `fix/<desc>`, `chore/<desc>`
- Commit messages: Conventional Commits (`feat:`, `fix:`, `test:`, `docs:`)
- Never push directly to main
