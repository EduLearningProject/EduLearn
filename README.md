# EduLearn v10.0 — University Learning Management & Student Information System

6 Microservices • 25 Entities • 9 Modules • 7 Roles • Enterprise Scope

## Quick Start (For Team Members)

### Prerequisites
- Visual Studio 2022 (ASP.NET + Data workloads)
- .NET 8 SDK
- SQL Server LocalDB (installed with VS 2022)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### Setup Steps

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
   .\add-migrations.bat
   .\migrate-database.bat
   ```
   **OR** run manually:
   ```
   dotnet ef migrations add InitialCreate --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator
   dotnet ef database update --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator
   ```

4. Open `EduLearn.slnx` in Visual Studio 2022

5. Run any service:
   ```
   cd EduLearn.AuthService
   dotnet run
   ```
   Open: http://localhost:5001/swagger

### Architecture

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

### Resetting the Database

```sql
-- In SSMS, connect to (localdb)\MSSQLLocalDB, run on master:
USE master;
ALTER DATABASE EduLearnDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE EduLearnDb;
```
Then re-run `.\migrate-database.bat`.

### Connection String

All services connect to: `Server=(localdb)\MSSQLLocalDB;Database=EduLearnDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true`

Update `ConnectionStrings.DefaultConnection` in each service's `appsettings.json` if using a different SQL Server instance.

### Team Assignment

| Member | Name | Service | Features |
|--------|------|---------|----------|
| M1 | Ashish | AuthService (:5001) | IAM-01 through IAM-04 |
| M2 | Saurav | SISService (:5002) | SRA-01 through SRA-03, ETS-01 through ETS-03 |
| M3 | Vikash | LMSService (:5003) | CCM-01 through CCM-03, LMS-01, LMS-02, AGI-01 through AGI-04 |
| M4 | Utkarsh | AnalyticsService (:5004) | RKA-01 through RKA-03 |
| M5 | Tanya | FinanceService (:5006) | SFB-01 through SFB-04 |
| M6 | Swarna Priyanshu | NotificationService (:5005) | NHT-01 through NHT-03 |

### Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Backend | ASP.NET Core 8.0 | REST APIs for 6 microservices |
| ORM | Entity Framework Core 8.0 | Code-first, LINQ queries |
| Database | SQL Server (LocalDB / Azure SQL) | 25 entity tables, single schema |
| Auth | JWT + BCrypt | Stateless auth with refresh token rotation |
| Real-Time | SignalR | WebSocket for live notifications |
| Testing | NUnit 3 + Moq | Unit + integration tests |
| Frontend | React 18 + TypeScript | SPA with role-based UI (post-interim) |

### Git Conventions

- Branch naming: `feature/<desc>`, `fix/<desc>`, `chore/<desc>`
- Commit messages: Conventional Commits (`feat:`, `fix:`, `test:`, `docs:`)
- Never push directly to main
