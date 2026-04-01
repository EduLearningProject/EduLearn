# EduLearn v8.0 — University LMS + SIS Platform

## Quick Start (For Team Members)

### Prerequisites
- Visual Studio 2022 (ASP.NET + Data workloads)
- .NET 8 SDK
- SQL Server LocalDB (installed with VS 2022)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### Setup Steps
1. Clone the repo:
   ```
   git clone https://github.com/EduLearningProject/EduLearn.git
   cd EduLearn
   ```

2. Create the database (run from solution root):
   ```
   migrate-database.bat
   ```
   **OR** run these commands manually **in this exact order**:
   ```
   dotnet ef database update --project EduLearn.AuthService
   dotnet ef database update --project EduLearn.LMSService
   dotnet ef database update --project EduLearn.SISService
   dotnet ef database update --project EduLearn.NotificationService
   ```
   > **IMPORTANT:** Order matters! Auth creates the Users table first. Other services reference it via foreign keys. Running SIS before Auth will fail.

3. Open `EduLearn.slnx` in Visual Studio 2022

4. Set Multiple Startup Projects:
   - Right-click Solution → Properties → Multiple startup projects
   - Set AuthService, SISService, LMSService to "Start"

5. Press F5 — Swagger UI opens for each service:
   - AuthService: http://localhost:5001/swagger
   - SISService: http://localhost:5002/swagger
   - LMSService: http://localhost:5003/swagger
   - AnalyticsService: http://localhost:5004/swagger
   - NotificationService: http://localhost:5005/swagger

### Architecture
5 ASP.NET Core 8 microservices sharing 1 SQL Server database (EduLearnDb):

| Service | Port | Tables Owned |
|---------|------|-------------|
| AuthService | 5001 | Users |
| SISService | 5002 | Enrollments, Attendance |
| LMSService | 5003 | Courses, CourseMaterials, Assessments, AssessmentSubmissions, ForumPosts |
| AnalyticsService | 5004 | None (read-only) |
| NotificationService | 5005 | Notifications |

### Migration Order (Why It Matters)
Each service has its own DbContext and migrations. Tables have foreign key dependencies:
- **Auth first** → Creates Users table (referenced by everything)
- **LMS second** → Creates Courses table (referenced by SIS)
- **SIS third** → Creates Enrollments + Attendance (references Users + Courses)
- **Notification last** → Creates Notifications (references Users)
- **Analytics never** → Read-only, no migrations

Services use `ExcludeFromMigrations()` on tables they reference but don't own. This prevents duplicate CREATE TABLE statements but requires the owning service's migration to run first.

### Connection String
All services connect to: `Server=(localdb)\MSSQLLocalDB;Database=EduLearnDb;Trusted_Connection=true`

If your SQL Server is on a different instance, update `ConnectionStrings.DefaultConnection` in each service's `appsettings.json`.

### Git Conventions
- Branch naming: `feature/<desc>`, `fix/<desc>`, `chore/<desc>`
- Commit messages: Conventional Commits (`feat:`, `fix:`, `test:`, `docs:`)
- Never push directly to main
