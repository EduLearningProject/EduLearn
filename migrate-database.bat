@echo off
REM ============================================
REM EduLearn Database Migration Script (PRD v10.0)
REM Uses a single MigrationDbContext to create
REM all 25 tables with all FK constraints in one
REM atomic migration. No FK ordering issues.
REM ============================================

cd /d "%~dp0"

echo ========================================
echo EduLearn Database Migration
echo Server: (localdb)\MSSQLLocalDB
echo Database: EduLearnDb
echo 25 Tables via single MigrationDbContext
echo ========================================
echo.

dotnet ef --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: dotnet-ef global tool is not installed.
    echo     dotnet tool install --global dotnet-ef
    pause
    exit /b 1
)

echo Applying migrations...
dotnet ef database update --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: Database migration failed. Fix errors above.
    pause
    exit /b 1
)
echo DONE
echo.

echo ========================================
echo ALL MIGRATIONS COMPLETE
echo Database: EduLearnDb
echo Tables: 25 + __EFMigrationsHistory
echo ========================================
echo.
echo Next: Open EduLearn.slnx in VS 2022 and press F5
pause
