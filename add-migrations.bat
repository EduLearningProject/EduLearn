@echo off
REM ============================================
REM EduLearn — Generate Migration (DbMigrator)
REM Uses a single MigrationDbContext that knows
REM all 25 entities. No FK ordering issues.
REM ============================================

cd /d "%~dp0"

echo ========================================
echo EduLearn — Generate Migration
echo ========================================
echo.

dotnet ef --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: dotnet-ef global tool is not installed.
    echo     dotnet tool install --global dotnet-ef
    pause
    exit /b 1
)

echo Generating InitialCreate migration...
dotnet ef migrations add InitialCreate --project EduLearn.DbMigrator --startup-project EduLearn.DbMigrator
if %ERRORLEVEL% NEQ 0 (
    echo FAILED: Migration generation failed.
    pause
    exit /b 1
)
echo DONE
echo.

echo ========================================
echo Migration generated successfully.
echo Next: Run migrate-database.bat
echo ========================================
pause
