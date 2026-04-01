@echo off
REM ============================================
REM EduLearn Database Migration Script
REM Run this ONCE after cloning to create the database
REM Order matters: Auth creates Users table first,
REM then LMS creates Courses, then SIS and Notification
REM ============================================

echo ========================================
echo EduLearn Database Migration
echo Server: (localdb)\MSSQLLocalDB
echo Database: EduLearnDb
echo ========================================
echo.

echo [1/4] Creating Users table (AuthService)...
dotnet ef database update --project EduLearn.AuthService
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: AuthService migration. Fix errors above before continuing.
    pause
    exit /b 1
)
echo [1/4] DONE
echo.

echo [2/4] Creating Courses, Materials, Assessments, Submissions, ForumPosts (LMSService)...
dotnet ef database update --project EduLearn.LMSService
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: LMSService migration. Fix errors above before continuing.
    pause
    exit /b 1
)
echo [2/4] DONE
echo.

echo [3/4] Creating Enrollments, Attendance (SISService)...
dotnet ef database update --project EduLearn.SISService
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: SISService migration. Fix errors above before continuing.
    pause
    exit /b 1
)
echo [3/4] DONE
echo.

echo [4/4] Creating Notifications (NotificationService)...
dotnet ef database update --project EduLearn.NotificationService
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: NotificationService migration. Fix errors above before continuing.
    pause
    exit /b 1
)
echo [4/4] DONE
echo.

echo ========================================
echo ALL MIGRATIONS COMPLETE
echo Database: EduLearnDb
echo Tables: 9 + __EFMigrationsHistory
echo ========================================
echo.
echo Next: Open EduLearn.slnx in VS 2022 and press F5
pause
