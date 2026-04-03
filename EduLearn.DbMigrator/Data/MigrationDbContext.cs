using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.DbMigrator.Data;

/// <summary>
/// Master DbContext for database migrations ONLY.
/// Maps all 25 entities with all FK constraints in a single migration.
/// Individual services use their own DbContexts at runtime.
/// This context is never injected into any service — only used by dotnet ef.
/// </summary>
public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }

    // ── AuthService entities ──
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // ── SISService entities ──
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Room> Rooms => Set<Room>();

    // ── LMSService entities ──
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<EduLearn.Shared.Entities.Program> Programs => Set<EduLearn.Shared.Entities.Program>();
    public DbSet<Syllabus> Syllabi => Set<Syllabus>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<Discussion> Discussions => Set<Discussion>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<GradeChange> GradeChanges => Set<GradeChange>();

    // ── FinanceService entities ──
    public DbSet<FeeSchedule> FeeSchedules => Set<FeeSchedule>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();

    // ── NotificationService entities ──
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    // ── AnalyticsService entities ──
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<KPI> KPIs => Set<KPI>();
    public DbSet<AuditPackage> AuditPackages => Set<AuditPackage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ════════════════════════════════════════
        // AuthService entities
        // ════════════════════════════════════════

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.AuditLogs)
                  .WithOne(a => a.User)
                  .HasForeignKey(a => a.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(u => u.Student)
                  .WithOne(s => s.User)
                  .HasForeignKey<Student>(s => s.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(u => u.InstructorSections)
                  .WithOne(s => s.Instructor)
                  .HasForeignKey(s => s.InstructorID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(u => u.Notifications)
                  .WithOne(n => n.User)
                  .HasForeignKey(n => n.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // AuditLog — FK configured via User above

        // ════════════════════════════════════════
        // SISService entities
        // ════════════════════════════════════════

        modelBuilder.Entity<Student>(entity =>
        {
            // User FK: configured in User entity block
            // Program FK: configured in Program entity block

            entity.HasMany(s => s.Enrollments)
                  .WithOne(e => e.Student)
                  .HasForeignKey(e => e.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(s => s.Submissions)
                  .WithOne(sub => sub.Student)
                  .HasForeignKey(sub => sub.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(s => s.Transcripts)
                  .WithOne(t => t.Student)
                  .HasForeignKey(t => t.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(s => s.Invoices)
                  .WithOne(i => i.Student)
                  .HasForeignKey(i => i.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(s => s.Scholarships)
                  .WithOne(sc => sc.Student)
                  .HasForeignKey(sc => sc.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Applicant — no FKs

        modelBuilder.Entity<Section>(entity =>
        {
            // Course FK: configured in Course entity block
            // Instructor FK: configured in User entity block

            entity.HasOne(sec => sec.Room)
                  .WithMany(r => r.Sections)
                  .HasForeignKey(sec => sec.RoomID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(sec => sec.Enrollments)
                  .WithOne(e => e.Section)
                  .HasForeignKey(e => e.SectionID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(sec => sec.Assessments)
                  .WithOne(a => a.Section)
                  .HasForeignKey(a => a.SectionID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Enrollment — FKs configured via Student and Section above
        // Transcript — FK configured via Student above
        // Room — no outgoing FKs

        // ════════════════════════════════════════
        // LMSService entities
        // ════════════════════════════════════════

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasMany(c => c.Sections)
                  .WithOne(s => s.Course)
                  .HasForeignKey(s => s.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(c => c.Contents)
                  .WithOne(ct => ct.Course)
                  .HasForeignKey(ct => ct.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(c => c.Discussions)
                  .WithOne(d => d.Course)
                  .HasForeignKey(d => d.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(c => c.Assessments)
                  .WithOne(a => a.Course)
                  .HasForeignKey(a => a.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(c => c.Syllabi)
                  .WithOne(sy => sy.Course)
                  .HasForeignKey(sy => sy.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<EduLearn.Shared.Entities.Program>(entity =>
        {
            entity.HasMany(p => p.Students)
                  .WithOne(s => s.Program)
                  .HasForeignKey(s => s.ProgramID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(p => p.FeeSchedules)
                  .WithOne(f => f.Program)
                  .HasForeignKey(f => f.ProgramID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Syllabus>(entity =>
        {
            // Course FK configured via Course above

            entity.HasOne(sy => sy.CreatedBy)
                  .WithMany()
                  .HasForeignKey(sy => sy.CreatedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Content>(entity =>
        {
            // Course FK configured via Course above

            entity.HasOne(ct => ct.UploadedBy)
                  .WithMany()
                  .HasForeignKey(ct => ct.UploadedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Discussion>(entity =>
        {
            // Course FK configured via Course above

            entity.HasOne(d => d.ThreadStarter)
                  .WithMany()
                  .HasForeignKey(d => d.ThreadStarterID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            // Course FK configured via Course above
            // Section FK configured via Section above

            entity.HasOne(a => a.CreatedBy)
                  .WithMany()
                  .HasForeignKey(a => a.CreatedByFK)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(a => a.Submissions)
                  .WithOne(s => s.Assessment)
                  .HasForeignKey(s => s.AssessmentID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            // Assessment FK configured via Assessment above
            // Student FK configured via Student above

            entity.HasOne(s => s.Grader)
                  .WithMany()
                  .HasForeignKey(s => s.GraderID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(s => s.GradeChanges)
                  .WithOne(gc => gc.Submission)
                  .HasForeignKey(gc => gc.SubmissionID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<GradeChange>(entity =>
        {
            // Submission FK configured via Submission above

            entity.HasOne(gc => gc.ChangedBy)
                  .WithMany()
                  .HasForeignKey(gc => gc.ChangedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ════════════════════════════════════════
        // FinanceService entities
        // ════════════════════════════════════════

        // FeeSchedule — Program FK configured via Program above

        modelBuilder.Entity<Invoice>(entity =>
        {
            // Student FK configured via Student above

            entity.HasMany(i => i.Payments)
                  .WithOne(p => p.Invoice)
                  .HasForeignKey(p => p.InvoiceID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Payment — Invoice FK configured via Invoice above
        // Scholarship — Student FK configured via Student above

        // ════════════════════════════════════════
        // NotificationService entities
        // ════════════════════════════════════════

        // Notification — User FK configured via User above

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.CreatedBy)
                  .WithMany()
                  .HasForeignKey(t => t.CreatedByFK)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.AssignedTo)
                  .WithMany()
                  .HasForeignKey(t => t.AssignedToFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ════════════════════════════════════════
        // AnalyticsService entities
        // ════════════════════════════════════════

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasOne(r => r.GeneratedBy)
                  .WithMany()
                  .HasForeignKey(r => r.GeneratedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // KPI — no FKs
        // AuditPackage — no FKs
    }
}
