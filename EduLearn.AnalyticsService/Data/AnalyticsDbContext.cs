using EduLearn.Shared.Entities;
using EduLearn.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.AnalyticsService.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    // ── Owned tables (write) ──
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<KPI> KPIs => Set<KPI>();
    public DbSet<AuditPackage> AuditPackages => Set<AuditPackage>();

    // ── Referenced tables (read-only for reporting queries) ──
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<EduLearn.Shared.Entities.Program> Programs => Set<EduLearn.Shared.Entities.Program>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<AuditLog>();
        modelBuilder.Ignore<Applicant>();
        modelBuilder.Ignore<Transcript>();
        modelBuilder.Ignore<Room>();
        modelBuilder.Ignore<Syllabus>();
        modelBuilder.Ignore<Content>();
        modelBuilder.Ignore<Discussion>();
        modelBuilder.Ignore<GradeChange>();
        modelBuilder.Ignore<FeeSchedule>();
        modelBuilder.Ignore<Invoice>();
        modelBuilder.Ignore<Payment>();
        modelBuilder.Ignore<Scholarship>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<Ticket>();

        // ── All referenced tables use ExcludeFromMigrations ──

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            entity.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(u => u.AuditLogs);
            entity.Ignore(u => u.Notifications);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students", t => t.ExcludeFromMigrations());
            entity.Property(s => s.EnrollmentStatus).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(s => s.Transcripts);
            entity.Ignore(s => s.Invoices);
            entity.Ignore(s => s.Scholarships);

            entity.HasOne(s => s.User)
                  .WithOne(u => u.Student)
                  .HasForeignKey<Student>(s => s.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Program)
                  .WithMany(p => p.Students)
                  .HasForeignKey(s => s.ProgramID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", t => t.ExcludeFromMigrations());
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(c => c.Contents);
            entity.Ignore(c => c.Discussions);
            entity.Ignore(c => c.Syllabi);
        });

        modelBuilder.Entity<EduLearn.Shared.Entities.Program>(entity =>
        {
            entity.ToTable("Programs", t => t.ExcludeFromMigrations());
            entity.Ignore(p => p.FeeSchedules);
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.ToTable("Sections", t => t.ExcludeFromMigrations());
            entity.Ignore(sec => sec.Room);

            entity.HasOne(sec => sec.Course)
                  .WithMany(c => c.Sections)
                  .HasForeignKey(sec => sec.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(sec => sec.Instructor)
                  .WithMany(u => u.InstructorSections)
                  .HasForeignKey(sec => sec.InstructorID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments", t => t.ExcludeFromMigrations());
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Section)
                  .WithMany(sec => sec.Enrollments)
                  .HasForeignKey(e => e.SectionID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.ToTable("Assessments", t => t.ExcludeFromMigrations());
            entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(a => a.Course)
                  .WithMany(c => c.Assessments)
                  .HasForeignKey(a => a.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.Section)
                  .WithMany(sec => sec.Assessments)
                  .HasForeignKey(a => a.SectionID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.CreatedBy)
                  .WithMany()
                  .HasForeignKey(a => a.CreatedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("Submissions", t => t.ExcludeFromMigrations());
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(s => s.GradeChanges);

            entity.HasOne(s => s.Assessment)
                  .WithMany(a => a.Submissions)
                  .HasForeignKey(s => s.AssessmentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Student)
                  .WithMany(st => st.Submissions)
                  .HasForeignKey(s => s.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Grader)
                  .WithMany()
                  .HasForeignKey(s => s.GraderID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Owned tables ──

        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(r => r.Scope).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(r => r.GeneratedBy)
                  .WithMany()
                  .HasForeignKey(r => r.GeneratedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<KPI>(entity =>
        {
            entity.Property(k => k.ReportingPeriod).HasConversion<string>().HasMaxLength(20);
        });

        // AuditPackage — no FKs, no special config
    }
}
