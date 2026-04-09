# EduLearn v10.0 — Architecture Quick Reference
# Claude Code: Read this file when you need schema, endpoint, or architecture details.
# Full PRD: EduLearn-PRD-v10.0.docx (17 sections, 55 tables, 25 entities, 82 endpoints)

## 6 Microservices + DbMigrator (Single Shared SQL Server Database: EduLearnDb)

| Service | Port | Tables Owned (Write) | Tables Referenced (Read-Only) |
|---------|------|---------------------|-------------------------------|
| AuthService | 5001 | User, AuditLog | — |
| SISService | 5002 | Student, Applicant, Transcript, Section, Enrollment, Room | User, Course, Program |
| LMSService | 5003 | Course, Program, Syllabus, Content, Discussion, Assessment, Submission, GradeChange | User, Section, Student |
| AnalyticsService | 5004 | Report, KPI, AuditPackage | User, Student, Course, Program, Section, Enrollment, Assessment, Submission |
| NotificationService | 5005 | Notification, Ticket | User |
| FinanceService | 5006 | FeeSchedule, Invoice, Payment, Scholarship | Student, Program, User |
| **DbMigrator** | — | ALL 25 tables (migration-only, never runs as service) | — |

## Migration Strategy
- **Single MigrationDbContext** in EduLearn.DbMigrator maps ALL 25 entities
- One `InitialCreate` migration creates everything atomically — zero FK ordering issues
- Per-service DbContexts are for RUNTIME queries only, never migrations
- Commands: `.\migrate-database.bat` or `dotnet ef database update --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator`

## 9 Modules → 6 Services

| Module | Prefix | Service | Features |
|--------|--------|---------|----------|
| Identity & Access Management | IAM | AuthService | IAM-01 to IAM-04 |
| Student Registry & Admissions | SRA | SISService | SRA-01 to SRA-03 |
| Course Catalog & Curriculum | CCM | LMSService | CCM-01 to CCM-03 |
| Enrollment & Timetable | ETS | SISService | ETS-01 to ETS-03 |
| Learning Content & LMS | LMS | LMSService | LMS-01 to LMS-03 |
| Assessment, Grading & Integrity | AGI | LMSService | AGI-01 to AGI-04 |
| Student Finance & Billing | SFB | FinanceService | SFB-01 to SFB-04 |
| Reporting, KPIs & Audit | RKA | AnalyticsService | RKA-01 to RKA-03 |
| Notifications, Helpdesk & Tasks | NHT | NotificationService | NHT-01 to NHT-03 |

## 7 User Roles
Student | Instructor | Registrar | DeptAdmin | Finance | ITAdmin | Auditor

## 25 Entities — Schema Summary

### 1. User (Auth) — UserID, Username(unique), FullName, Email(unique), Phone, Role, PasswordHash, MFAEnabled, Status, RefreshToken, RefreshTokenExpiry, CreatedAt, UpdatedAt
### 2. AuditLog (Auth) — AuditID, UserID FK→User, Action, ResourceType, ResourceID, DetailsJSON, Timestamp
### 3. Applicant (SIS) — ApplicantID, Name, DOB, NationalID, ContactInfoJSON, ProgramApplied, ApplicationStatus, SubmittedAt, DocumentsURIJSON
### 4. Student (SIS) — StudentID, UserID FK→User(unique), MRN(unique), Name, DOB, Gender, ContactInfoJSON, EnrollmentStatus, ProgramID FK→Program, EntryTerm, ExpectedGraduationTerm, CreatedAt
### 5. Transcript (SIS) — TranscriptID, StudentID FK→Student, IssuedAt, EntriesJSON, GPA, Status, TranscriptURI
### 6. Course (LMS) — CourseID, Code(unique), Title, Description, Credits, DepartmentID(nullable no FK), Level, PrerequisitesJSON, Status, CreatedAt
### 7. Program (LMS) — ProgramID, Name, DepartmentID(nullable no FK), DegreeType, RequiredCoursesJSON, ElectivesJSON, DurationTerms, Status
### 8. Syllabus (LMS) — SyllabusID, CourseID FK→Course, Version, LearningOutcomesJSON, AssessmentPlanJSON, CreatedByFK FK→User, CreatedAt, SyllabusURI
### 9. Section (SIS) — SectionID, CourseID FK→Course, Term, InstructorID FK→User, RoomID FK→Room(nullable), Capacity, EnrolledCount, ScheduleJSON, Status
### 10. Enrollment (SIS) — EnrollID, StudentID FK→Student, SectionID FK→Section, EnrolledAt, Status, WaitlistPosition, GradePostedFlag
### 11. Room (SIS) — RoomID, Building, RoomNumber, Capacity, ResourcesJSON, Status
### 12. Content (LMS) — ContentID, CourseID FK→Course, Title, Type, URI, UploadedByFK FK→User, UploadedAt, Version, MetadataJSON, Status
### 13. Discussion (LMS) — DiscussionID, CourseID FK→Course, ThreadStarterID FK→User, Title, PostsJSON, CreatedAt, Status
### 14. Assessment (LMS) — AssessmentID, CourseID FK→Course, SectionID FK→Section(nullable), Title, Type, DueAt, MaxScore, GradingRubricJSON, CreatedByFK FK→User, CreatedAt, Status
### 15. Submission (LMS) — SubmissionID, AssessmentID FK→Assessment, StudentID FK→Student, SubmittedAt, FileURI, Score, GraderID FK→User(nullable), GradedAt, PlagiarismReportURI, Status
### 16. GradeChange (LMS) — GradeChangeID, SubmissionID FK→Submission, OldScore, NewScore, ChangedByFK FK→User, ChangedAt, Reason, AuditNote
### 17. FeeSchedule (Finance) — FeeID, ProgramID FK→Program, Term, FeeItemsJSON, EffectiveFrom, EffectiveTo, Status
### 18. Invoice (Finance) — InvoiceID, StudentID FK→Student, Term, LineItemsJSON, AmountDue, IssuedAt, DueDate, Status, InvoiceURI
### 19. Payment (Finance) — PaymentID, InvoiceID FK→Invoice, PaidAt, Amount, Method, Reference, Status
### 20. Scholarship (Finance) — ScholarID, StudentID FK→Student, AwardType, Amount, AppliedAt, ValidFrom, ValidTo, Status
### 21. Report (Analytics) — ReportID, Scope, ParametersJSON, MetricsJSON, GeneratedByFK FK→User, GeneratedAt, ReportURI
### 22. KPI (Analytics) — KPIID, Name, Definition, Target, CurrentValue, ReportingPeriod
### 23. AuditPackage (Analytics) — PackageID, PeriodStart, PeriodEnd, ContentsJSON, GeneratedAt, PackageURI
### 24. Notification (Notification) — NotificationID, UserID FK→User, EntityID, Message, Category, Severity, CreatedAt, ReadAt, Status
### 25. Ticket (Notification) — TicketID, CreatedByFK FK→User, AssignedToFK FK→User(nullable), Subject, Description, Priority, CreatedAt, UpdatedAt, Status, ResolutionURI

## Current Codebase Status
- 25 entity classes: ✅ All created in EduLearn.Shared/Entities/
- 6 per-service DbContexts: ✅ With proper Ignore/ExcludeFromMigrations
- MigrationDbContext: ✅ All 25 entities, all FK relationships, InitialCreate migration
- Controllers: ⚠️ Minimal — Health + basic CRUD only (UsersController, EnrollmentsController, CoursesController)
- DTOs: ⚠️ Only Auth DTOs exist (CreateUserDto, UpdateUserDto, UserResponseDto, UpdateStatusDto)
- JWT Auth: ❌ Not implemented yet (IAM-01)
- Tests: ❌ Shell project only, no tests written

## API Endpoints (82 total — see PRD Section 7 for full reference)

### AuthService :5001 — 10 endpoints
POST /api/auth/login, /register, /refresh, /mfa/setup, /mfa/verify
GET /api/users, /users/{id}, /audit-log | PUT /api/users/{id}, /users/{id}/status

### SISService :5002 — 18 endpoints
POST /api/applicants, /enrollment/enroll, /sections, /rooms, /transcripts/generate/{studentId}, /timetable/validate-section
GET /api/applicants, /students, /students/{id}, /transcripts/student/{studentId}, /sections/course/{courseId}/term/{term}, /enrollment/student/{studentId}, /enrollment/section/{sectionId}, /timetable/student/{studentId}/{term}, /rooms
PUT /api/applicants/{id}/status, /students/{id} | DELETE /api/enrollment/{id}/drop

### LMSService :5003 — 26 endpoints
POST /api/courses, /programs, /syllabi, /content/upload, /discussions, /discussions/{id}/reply, /assessments, /submissions, /submissions/{id}/grade, /grade-changes
GET /api/courses, /courses/{id}, /programs, /programs/{id}, /syllabi/course/{courseId}, /courses/{id}/check-prerequisites/{studentId}, /content/course/{courseId}, /content/{id}, /discussions/course/{courseId}, /assessments/course/{courseId}, /submissions/assessment/{assessmentId}, /submissions/student/{studentId}, /grade-changes/submission/{submissionId}
PUT /api/courses/{id}, /assessments/{id}, /assessments/{id}/publish

### FinanceService :5006 — 11 endpoints
POST /api/fees, /invoices/generate, /payments, /scholarships
GET /api/fees/program/{programId}/term/{term}, /invoices/student/{studentId}, /invoices/{id}, /payments/invoice/{invoiceId}, /scholarships/student/{studentId}
PUT /api/fees/{id}, /scholarships/{id}

### AnalyticsService :5004 — 7 endpoints
POST /api/reports/generate, /kpis/recalculate, /audit-packages/generate
GET /api/reports, /reports/{id}/download, /kpis, /audit-packages/{id}/download

### NotificationService :5005 — 10 endpoints
GET /api/notifications, /notifications/unread-count, /tickets, /tickets/{id}
PUT /api/notifications/{id}/read, /notifications/read-all, /tickets/{id}/assign, /tickets/{id}/resolve
POST /api/tickets | WS /notificationHub

## Implementation Patterns
- Waitlist = enrollment STATUS field, auto-promote on drop
- Grade audit = auto-logging middleware on score changes
- Ticket status changes = single PUT with status field
- Prerequisite + timetable conflict checks embedded in enrollment POST
- Fee schedules immutable once active — create new version
- Invoice generation auto-applies scholarship deductions
- All AnalyticsService queries use .AsNoTracking()
