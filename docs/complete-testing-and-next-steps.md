# EduLearn — Complete Swagger Testing & Next Steps
# Schema Version: FinalizeSchema (RefreshToken removed, 19 enums, JsonStringEnumConverter)
# Last Updated: 2026-04-06

---

## PART 1: Current Endpoint Audit

### What Exists Right Now (15 endpoints + Health across 6 services)

| Service | Port | Endpoint | Method | Purpose |
|---|---|---|---|---|
| **AuthService** | 5001 | `/api/Health` | GET | Health check |
| | | `/api/Users` | POST | Create user |
| | | `/api/Users` | GET | List all users |
| | | `/api/Users/{id}` | GET | Get user by ID |
| | | `/api/Users/{id}` | PUT | Update user profile |
| | | `/api/Users/{id}/status` | PUT | Update user status |
| **LMSService** | 5003 | `/api/Health` | GET | Health check |
| | | `/api/Courses` | POST | Create course |
| | | `/api/Courses` | GET | List all courses |
| | | `/api/Courses/{id}` | GET | Get course by ID |
| | | `/api/Courses/{id}` | PUT | Update course |
| **SISService** | 5002 | `/api/Health` | GET | Health check |
| | | `/api/enrollment/enroll` | POST | Enroll student in section |
| | | `/api/enrollment/{id}/drop` | DELETE | Drop enrollment |
| | | `/api/enrollment/student/{studentId}` | GET | Get student enrollments |
| | | `/api/enrollment/section/{sectionId}` | GET | Get section roster |
| **FinanceService** | 5006 | `/api/Health` | GET | Health check only |
| **AnalyticsService** | 5004 | `/api/Health` | GET | Health check only |
| **NotificationService** | 5005 | `/api/Health` | GET | Health check only |

---

## IMPORTANT: Swagger Behavior Changes After Enum Migration

Role and Status fields are now strongly-typed enums with `JsonStringEnumConverter`. This changes Swagger UI behavior:

- **Role field** (POST /api/Users) — renders as a **dropdown** showing: Student, Instructor, Registrar, DeptAdmin, Finance, ITAdmin, Auditor. Select from the list instead of typing.
- **Status field** (PUT /api/Users/{id}/status) — renders as a **dropdown** showing: Active, Inactive, Suspended, Locked.
- **Invalid role/status tests** — Swagger's dropdown physically prevents invalid input. To test these negative cases, use Postman or curl with a raw JSON body containing an invalid value.
- **API responses** — Role and Status return as strings (`"Enrolled"`, `"Active"`) not integers. This is correct.

---

## PART 2: Step-by-Step Swagger Testing

**Important:** Run ONE service at a time. Stop each with `Ctrl+C` before starting the next. All paths below use your actual machine path.

---

### SERVICE 1: AuthService — http://localhost:5001/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.AuthService"
dotnet run
```
Open browser: **http://localhost:5001/swagger**

Swagger shows 6 endpoints: Health GET, Users POST, Users GET, Users GET/{id}, Users PUT/{id}, Users PUT/{id}/status

#### Test 1.1 — Health Check
`GET /api/Health` → Try it out → Execute
**Expected:** 200 — `{"status":"healthy","service":"AuthService","timestamp":"..."}`

#### Test 1.2 — Create ITAdmin
`POST /api/Users` → Try it out → paste body. Role field is a dropdown — select **ITAdmin**:
```json
{
  "username": "ashish.admin",
  "fullName": "Ashish Kumar",
  "email": "ashish@edulearn.com",
  "phone": "+91-9876543210",
  "role": "ITAdmin",
  "password": "Admin@123"
}
```
**Expected:** 201 — `"userID": 1`, `"role": "ITAdmin"`, `"status": "Active"`

#### Test 1.3 — Create Instructor
```json
{
  "username": "dr.priya",
  "fullName": "Dr. Priya Sharma",
  "email": "priya@edulearn.com",
  "phone": "+91-9876543211",
  "role": "Instructor",
  "password": "Inst@123"
}
```
**Expected:** 201 — `"userID": 2`

#### Test 1.4 — Create Student 1 (Rahul)
```json
{
  "username": "rahul.student",
  "fullName": "Rahul Kumar",
  "email": "rahul@edulearn.com",
  "phone": "+91-9876543212",
  "role": "Student",
  "password": "Stud@123"
}
```
**Expected:** 201 — `"userID": 3`

#### Test 1.5 — Create Student 2 (Sneha)
```json
{
  "username": "sneha.student",
  "fullName": "Sneha Gupta",
  "email": "sneha@edulearn.com",
  "phone": "+91-9876543213",
  "role": "Student",
  "password": "Stud@456"
}
```
**Expected:** 201 — `"userID": 4`

#### Test 1.6 — Duplicate Email (Negative)
POST `/api/Users` with `"email": "ashish@edulearn.com"`, different username
**Expected:** 409 — `"code": "DUPLICATE_EMAIL"`

#### Test 1.7 — Duplicate Username (Negative)
POST `/api/Users` with `"username": "ashish.admin"`, different email
**Expected:** 409 — `"code": "DUPLICATE_USERNAME"`

#### Test 1.8 — Invalid Role (Negative — use Postman/curl, not Swagger dropdown)
```json
{
  "username": "test.user",
  "fullName": "Test User",
  "email": "test@edulearn.com",
  "role": "SuperAdmin",
  "password": "Test@123"
}
```
**Expected:** 400 — validation error (model binding rejects unknown enum value)
> Note: Swagger UI dropdown prevents this input. Use Postman: POST http://localhost:5001/api/Users with raw JSON above.

#### Test 1.9 — Get All Users
`GET /api/Users` → Execute
**Expected:** 200 — array with 4 users. Verify `"role"` shows as string (e.g. `"ITAdmin"` not `2`)

#### Test 1.10 — Get User by ID
`GET /api/Users/1`
**Expected:** 200 — Ashish Kumar, `"role": "ITAdmin"`, `"status": "Active"`

#### Test 1.11 — Get Non-Existent User (Negative)
`GET /api/Users/999`
**Expected:** 404 — `"code": "USER_NOT_FOUND"`

#### Test 1.12 — Update User Profile
`PUT /api/Users/3`:
```json
{
  "fullName": "Rahul Kumar Singh",
  "email": "rahul.updated@edulearn.com",
  "phone": "+91-9999999999"
}
```
**Expected:** 200 — updated fullName and email, `"updatedAt"` is not null

#### Test 1.13 — Update Status to Suspended
`PUT /api/Users/4/status` — Status is a dropdown, select **Suspended**:
```json
{
  "status": "Suspended"
}
```
**Expected:** 200 — `"status": "Suspended"`

#### Test 1.14 — Restore Status to Active
`PUT /api/Users/4/status` — select **Active**:
```json
{
  "status": "Active"
}
```
**Expected:** 200 — `"status": "Active"`

#### Test 1.15 — Invalid Status (Negative — use Postman/curl)
POST to `PUT /api/Users/4/status` with body `{"status": "Banned"}`
**Expected:** 400 — validation error
> Note: Swagger dropdown prevents this. Use Postman: PUT http://localhost:5001/api/Users/4/status

**Stop: `Ctrl+C`**

---

### SERVICE 2: LMSService — http://localhost:5003/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.LMSService"
dotnet run
```
Open browser: **http://localhost:5003/swagger**

Swagger shows 5 endpoints: Health GET, Courses POST, Courses GET, Courses GET/{id}, Courses PUT/{id}

#### Test 2.1 — Health Check
`GET /api/Health` → **Expected:** 200 — `"service": "LMSService"`

#### Test 2.2 — Create Course 1
`POST /api/Courses`:
```json
{
  "code": "CS101",
  "title": "Introduction to Computer Science",
  "description": "Fundamentals of CS",
  "credits": 3,
  "departmentID": 1,
  "level": "100-level",
  "prerequisitesJSON": null
}
```
**Expected:** 201 — `"courseID": 1`, `"status": "Active"`

#### Test 2.3 — Create Course 2
```json
{
  "code": "CS201",
  "title": "Data Structures and Algorithms",
  "description": "Advanced DSA",
  "credits": 4,
  "departmentID": 1,
  "level": "200-level",
  "prerequisitesJSON": "[1]"
}
```
**Expected:** 201 — `"courseID": 2`

#### Test 2.4 — Create Course 3
```json
{
  "code": "MATH201",
  "title": "Linear Algebra",
  "description": "Vectors, matrices, linear transformations",
  "credits": 4,
  "departmentID": 2,
  "level": "200-level",
  "prerequisitesJSON": null
}
```
**Expected:** 201 — `"courseID": 3`

#### Test 2.5 — Duplicate Code (Negative)
POST with `"code": "CS101"` again
**Expected:** 409 — `"code": "DUPLICATE_COURSE_CODE"`

#### Test 2.6 — Get All Courses
`GET /api/Courses` → **Expected:** 200 — 3 courses, each with `"status": "Active"`

#### Test 2.7 — Get Course by ID
`GET /api/Courses/2` → **Expected:** 200 — CS201 with `"prerequisitesJSON": "[1]"`

#### Test 2.8 — Get Non-Existent (Negative)
`GET /api/Courses/999` → **Expected:** 404

#### Test 2.9 — Update Course
`PUT /api/Courses/1`:
```json
{
  "code": "CS101",
  "title": "Intro to CS (Updated)",
  "description": "Updated description",
  "credits": 4,
  "departmentID": 1,
  "level": "100-level",
  "prerequisitesJSON": null
}
```
**Expected:** 200 — updated title and credits. Note: Course entity has no UpdatedAt field — do not expect a timestamp in the response.

**Stop: `Ctrl+C`**

---

### SERVICE 3: Seed Data via SSMS

Before testing SISService, insert prerequisite data. Connect to `(localdb)\MSSQLLocalDB`, query on `EduLearnDb`:

```sql
-- Verify users and courses exist from API tests
SELECT UserID, Username, Role FROM Users ORDER BY UserID;
-- Expected: 4 rows (IDs 1-4), Role stored as string e.g. 'ITAdmin', 'Student'

SELECT CourseID, Code, Title FROM Courses ORDER BY CourseID;
-- Expected: 3 rows (IDs 1-3)

-- Programs
INSERT INTO Programs (Name, DegreeType, DurationTerms, Status)
VALUES ('B.Tech Computer Science', 'Bachelor', 8, 'Active');

INSERT INTO Programs (Name, DegreeType, DurationTerms, Status)
VALUES ('M.Tech Data Science', 'Master', 4, 'Active');

-- Rooms
INSERT INTO Rooms (Building, RoomNumber, Capacity, ResourcesJSON, Status)
VALUES ('Block A', 'A101', 60, '{"projector":true,"whiteboard":true}', 'Available');

INSERT INTO Rooms (Building, RoomNumber, Capacity, ResourcesJSON, Status)
VALUES ('Block B', 'B201', 30, '{"projector":true,"computers":30}', 'Available');

-- Students (UserID 3 = Rahul, UserID 4 = Sneha)
-- EnrollmentStatus stored as enum string 'Active'
INSERT INTO Students (UserID, MRN, Name, DOB, Gender, EnrollmentStatus, ProgramID, EntryTerm, CreatedAt)
VALUES (3, 'STU2026001', 'Rahul Kumar', '2004-05-15', 'Male', 'Active', 1, 'Fall 2026', GETUTCDATE());

INSERT INTO Students (UserID, MRN, Name, DOB, Gender, EnrollmentStatus, ProgramID, EntryTerm, CreatedAt)
VALUES (4, 'STU2026002', 'Sneha Gupta', '2004-08-22', 'Female', 'Active', 1, 'Fall 2026', GETUTCDATE());

-- Sections
-- Section 1: CS101, capacity 60 (normal enrollment testing)
INSERT INTO Sections (CourseID, Term, InstructorID, RoomID, Capacity, EnrolledCount, ScheduleJSON, Status)
VALUES (1, 'Fall 2026', 2, 1, 60, 0, '[{"day":"Mon","startTime":"09:00","endTime":"10:30","room":"A101"}]', 'Open');

-- Section 2: CS101, capacity 1 (waitlist testing)
INSERT INTO Sections (CourseID, Term, InstructorID, RoomID, Capacity, EnrolledCount, ScheduleJSON, Status)
VALUES (1, 'Fall 2026', 2, 2, 1, 0, '[{"day":"Tue","startTime":"14:00","endTime":"15:30","room":"B201"}]', 'Open');

-- Section 3: MATH201, capacity 60
INSERT INTO Sections (CourseID, Term, InstructorID, RoomID, Capacity, EnrolledCount, ScheduleJSON, Status)
VALUES (3, 'Fall 2026', 2, 1, 60, 0, '[{"day":"Fri","startTime":"11:00","endTime":"12:30","room":"A101"}]', 'Open');

-- Verify
SELECT s.SectionID, c.Code, s.Term, s.Capacity, s.EnrolledCount, s.Status
FROM Sections s INNER JOIN Courses c ON s.CourseID = c.CourseID;

SELECT st.StudentID, st.MRN, st.Name, u.Username, st.EnrollmentStatus, p.Name AS Program
FROM Students st
INNER JOIN Users u ON st.UserID = u.UserID
INNER JOIN Programs p ON st.ProgramID = p.ProgramID;
```

---

### SERVICE 4: SISService — http://localhost:5002/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.SISService"
dotnet run
```
Open browser: **http://localhost:5002/swagger**

Swagger shows 5 endpoints: Health GET, enrollment/enroll POST, enrollment/{id}/drop DELETE, enrollment/student/{id} GET, enrollment/section/{id} GET

#### Test 4.1 — Health Check
`GET /api/Health` → **Expected:** 200 — `"service": "SISService"`

#### Test 4.2 — Enroll Student 1 in Section 1 (Happy Path)
`POST /api/enrollment/enroll`:
```json
{
  "studentID": 1,
  "sectionID": 1
}
```
**Expected:** 201 — `"status": "Enrolled"` (string, not integer), `"studentName": "Rahul Kumar"`, `"waitlistPosition": null`

#### Test 4.3 — Duplicate Enrollment (Negative)
Same body again: `{"studentID": 1, "sectionID": 1}`
**Expected:** 409 — `"code": "DUPLICATE_ENROLLMENT"`

#### Test 4.4 — Enroll Student 2 in Section 1
```json
{
  "studentID": 2,
  "sectionID": 1
}
```
**Expected:** 201 — `"status": "Enrolled"`, `"studentName": "Sneha Gupta"`

#### Test 4.5 — Waitlist Testing (Section 2, capacity = 1)
First enrollment fills it:
```json
{
  "studentID": 1,
  "sectionID": 2
}
```
**Expected:** 201 — `"status": "Enrolled"`

Second enrollment hits capacity:
```json
{
  "studentID": 2,
  "sectionID": 2
}
```
**Expected:** 201 — `"status": "Waitlisted"`, `"waitlistPosition": 1`

#### Test 4.6 — Invalid Student (Negative)
```json
{
  "studentID": 999,
  "sectionID": 1
}
```
**Expected:** 400 — `"code": "STUDENT_NOT_FOUND"`

#### Test 4.7 — Invalid Section (Negative)
```json
{
  "studentID": 1,
  "sectionID": 999
}
```
**Expected:** 400 — `"code": "SECTION_NOT_FOUND"`

#### Test 4.8 — Get Enrollments by Student
`GET /api/enrollment/student/1`
**Expected:** 200 — array with 2 enrollments (Section 1: Enrolled, Section 2: Enrolled). Note the EnrollID for Rahul's Section 2 enrollment — you need it for Test 4.10.

#### Test 4.9 — Get Section Roster
`GET /api/enrollment/section/1`
**Expected:** 200 — 2 entries: Rahul and Sneha, both `"status": "Enrolled"`

#### Test 4.10 — Drop + Auto-Promote
From the GET response in Test 4.8, find the actual EnrollID for Rahul's Section 2 enrollment (do not assume it's 3 — read the response).

`DELETE /api/enrollment/{enrollID}/drop` (use the actual EnrollID)
**Expected:** 204 No Content

Verify auto-promotion:
`GET /api/enrollment/section/2`
**Expected:** Sneha's status is now `"Enrolled"`, `"waitlistPosition": null` (promoted from waitlist)

#### Test 4.11 — Re-Enroll After Drop
```json
{
  "studentID": 1,
  "sectionID": 2
}
```
**Expected:** 201 Created — re-enrollment works because dropped enrollments are excluded from the duplicate check

#### Test 4.12 — Enroll in Different Course
```json
{
  "studentID": 1,
  "sectionID": 3
}
```
**Expected:** 201 — MATH201 enrollment, `"status": "Enrolled"`

**Stop: `Ctrl+C`**

---

### SERVICE 5: FinanceService — http://localhost:5006/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.FinanceService"
dotnet run
```
Open browser: **http://localhost:5006/swagger**

#### Test 5.1 — Health Check
`GET /api/Health` → **Expected:** 200 — `"service": "FinanceService"`

**Stop: `Ctrl+C`**

---

### SERVICE 6: AnalyticsService — http://localhost:5004/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.AnalyticsService"
dotnet run
```

#### Test 6.1 — Health Check
`GET /api/Health` → **Expected:** 200 — `"service": "AnalyticsService"`

**Stop: `Ctrl+C`**

---

### SERVICE 7: NotificationService — http://localhost:5005/swagger

```powershell
cd "C:\Users\2487294\OneDrive - Cognizant\Desktop\EduLearn\EduLearn\EduLearn.NotificationService"
dotnet run
```

#### Test 7.1 — Health Check
`GET /api/Health` → **Expected:** 200 — `"service": "NotificationService"`

**Stop: `Ctrl+C`**

---

### POST-TEST: Database Integrity Verification (SSMS)

```sql
-- 1. Schema: RefreshToken columns must be gone
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' ORDER BY ORDINAL_POSITION;
-- Must NOT contain RefreshToken or RefreshTokenExpiry
-- Must contain: UserID, Username, FullName, Email, Phone, Role, PasswordHash, MFAEnabled, Status, CreatedAt, UpdatedAt

-- 2. Enum strings stored correctly (not integers)
SELECT UserID, Username, Role, Status FROM Users ORDER BY UserID;
-- Role must show 'ITAdmin', 'Instructor', 'Student' — not 0, 1, 2
-- Status must show 'Active', 'Suspended' — not 0, 1

-- 3. Section enrolled counts updated correctly
SELECT s.SectionID, c.Code, s.Capacity, s.EnrolledCount
FROM Sections s INNER JOIN Courses c ON s.CourseID = c.CourseID ORDER BY s.SectionID;
-- Section 1: EnrolledCount = 2 (Rahul + Sneha)
-- Section 2: EnrolledCount = 1 (whoever was promoted + re-enrolled)
-- Section 3: EnrolledCount = 1

-- 4. Enrollment statuses stored as strings
SELECT e.EnrollID, st.Name, c.Code, e.Status, e.WaitlistPosition
FROM Enrollments e
INNER JOIN Students st ON e.StudentID = st.StudentID
INNER JOIN Sections sec ON e.SectionID = sec.SectionID
INNER JOIN Courses c ON sec.CourseID = c.CourseID
ORDER BY e.EnrollID;
-- Status must show 'Enrolled', 'Dropped' — not integers

-- 5. FK constraints: 31 total, all NoAction
SELECT COUNT(*) AS TotalFKs FROM sys.foreign_keys;
-- Expected: 31

SELECT COUNT(*) AS CascadeFKs FROM sys.foreign_keys
WHERE delete_referential_action_desc != 'NO_ACTION';
-- Expected: 0

-- 6. Migration history: both migrations applied
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
-- Must show: InitialCreate AND FinalizeSchema

-- 7. Orphan check (all must return 0)
SELECT 'Students without User' AS Check, COUNT(*) AS Count
FROM Students WHERE UserID NOT IN (SELECT UserID FROM Users)
UNION ALL
SELECT 'Enrollments without Student', COUNT(*)
FROM Enrollments WHERE StudentID NOT IN (SELECT StudentID FROM Students)
UNION ALL
SELECT 'Enrollments without Section', COUNT(*)
FROM Enrollments WHERE SectionID NOT IN (SELECT SectionID FROM Sections)
UNION ALL
SELECT 'Sections without Course', COUNT(*)
FROM Sections WHERE CourseID NOT IN (SELECT CourseID FROM Courses);
```

---

## PART 3: Database Phase Status — COMPLETE AND LOCKED

| Item | Status |
|---|---|
| 25 entity classes in EduLearn.Shared | ✅ |
| 9 enum files in EduLearn.Shared/Enums/ | ✅ |
| 19 string fields converted to strongly-typed enums | ✅ |
| RefreshToken + RefreshTokenExpiry removed from User | ✅ |
| MigrationDbContext — all 31 FKs, HasConversion on all enums | ✅ |
| 6 per-service DbContexts — HasConversion on owned + referenced enums | ✅ |
| JsonStringEnumConverter in all 6 Program.cs | ✅ |
| InitialCreate + FinalizeSchema migrations applied | ✅ |
| 25 tables, 31 FKs, 5 unique indexes verified | ✅ |
| [RegularExpression] validators replaced by enum type safety | ✅ |
| AuthService — UsersController (5 endpoints) | ✅ |
| LMSService — CoursesController (4 endpoints) | ✅ |
| SISService — EnrollmentsController (4 endpoints) | ✅ |
| Health endpoints on all 6 services | ✅ |
| Build: 0 errors, 0 warnings | ✅ |

**The Database branch is ready to merge into main.**

---

## PART 4: Merge Procedure

```powershell
# On the Database branch — commit everything
git add .
git commit -m "feat: finalize schema — remove RefreshToken columns, add 19 enums with HasConversion"

# Push to remote
git push origin Database

# On GitHub — open a Pull Request: Database → main
# PR title: "feat: EduLearn v10.0 database schema finalized"
# PR description should list: 25 tables, 31 FKs, 19 enums, FinalizeSchema migration
# Get at least one team member to review before merging
```

After merge, every team member pulls main and creates their feature branch from it:
```powershell
git checkout main
git pull origin main
git checkout -b feature/IAM-01-jwt-auth        # Ashish
git checkout -b feature/SRA-01-admissions      # Saurav
git checkout -b feature/CCM-01-programs        # Vikash
git checkout -b feature/RKA-01-reports         # Utkarsh
git checkout -b feature/SFB-01-fee-schedules   # Tanya
git checkout -b feature/NHT-01-notifications   # Swarna
```

---

## PART 5: Next Steps — Feature Implementation

### Interim Target: April 24, 2026 (18 days) — ≥50% backend features working

#### M1 — Ashish (AuthService :5001)
- IAM-01: `AuthController` — POST /api/auth/login (BCrypt verify + JWT generate), POST /api/auth/register, POST /api/auth/logout. Add `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet. JWT 1-2 hour expiry (no refresh tokens — removed from schema by design).
- IAM-02: Add `[Authorize]` + `[Authorize(Roles="...")]` to all existing controllers once JWT middleware is wired. Share `AuthResponseDto` (Token, UserID, Role, FullName) in EduLearn.Shared/DTOs/Auth/.
- IAM-04: `AuditLogController` — GET /api/audit-log, GET /api/audit-log/user/{userId}, GET /api/audit-log/resource/{type}/{id}. Append-only — no POST/PUT/DELETE.

#### M2 — Saurav (SISService :5002)
- SRA-01: `ApplicantsController` — POST/GET/GET{id}/PUT{id}/status. ApplicationStatus enum already on entity.
- SRA-02: `StudentsController` — GET/GET{id}/PUT{id}. Student.EnrollmentStatus enum already on entity.
- ETS-02: `SectionsController` + `RoomsController` — POST/GET/PUT for both.
- ETS-01 (enhance existing): Add prerequisite check to EnrollmentsController — read Course.PrerequisitesJSON, verify against completed enrollments.

#### M3 — Vikash (LMSService :5003)
- CCM-01: `ProgramsController` — POST/GET/GET{id}/PUT. Programs entity already in LMSDbContext.
- LMS-01: `ContentController` — POST /api/content/upload, GET /api/content/course/{courseId}, PUT /api/content/{id}/version. ContentType and ContentStatus enums already on entity.
- AGI-01: `AssessmentsController` — POST/GET/PUT/PUT{id}/publish. AssessmentStatus + AssessmentType enums already on entity.
- AGI-02: `SubmissionsController` — POST (student submit), POST {id}/grade (instructor grade). SubmissionStatus enum already on entity. Grade endpoint must create GradeChange record.

#### M4 — Utkarsh (AnalyticsService :5004)
- RKA-01: `ReportsController` — POST /api/reports/generate, GET /api/reports, GET /api/reports/{id}, GET /api/reports/{id}/download. ReportScope enum already on entity. All queries use `.AsNoTracking()`.
- RKA-02: `KPIsController` — GET /api/kpis, POST /api/kpis/recalculate, GET /api/kpis/{id}, GET /api/dashboard/overview. KPI calculations are pure deterministic C# reading from AnalyticsDbContext's referenced tables.

#### M5 — Tanya (FinanceService :5006)
- SFB-01: `FeesController` — POST/GET/PUT. FeeScheduleStatus enum already on entity. Fee schedules immutable once Active — enforce in PUT.
- SFB-02: `InvoicesController` — POST /api/invoices/generate, GET /api/invoices/student/{id}, GET /api/invoices/{id}. Auto-apply active scholarships in generation.
- SFB-03: `PaymentsController` — POST /api/payments, GET /api/payments/invoice/{id}, GET /api/payments/student/{id}. PaymentStatus + PaymentMethod enums already on entity.

#### M6 — Swarna (NotificationService :5005)
- NHT-01: `NotificationsController` — GET /api/notifications, PUT {id}/read, PUT /read-all, GET /unread-count. Add SignalR hub. NotificationCategory + Severity + Status enums already on entity.
- NHT-03: `TicketsController` — POST/GET/GET{id}/PUT{id}/assign/PUT{id}/resolve/PUT{id}/close. TicketStatus + TicketPriority enums already on entity.

### Post-Interim → Final (April 25 – June 16)
Remaining 12 features: IAM-03 (MFA/TOTP), SRA-03 (Transcripts), CCM-02 (Syllabi), CCM-03 (Prerequisite enforcement), ETS-03 (Timetable conflict detection), LMS-02 (Discussions), AGI-03 (Grade audit trail), AGI-04 (Plagiarism), SFB-04 (Scholarships), RKA-03 (Audit packages), NHT-02 (Email service), plus React 18 frontend, Azure deployment, SonarQube, OWASP ZAP.
