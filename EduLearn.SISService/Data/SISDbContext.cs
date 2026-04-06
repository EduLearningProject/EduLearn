using EduLearn.Shared.Entities;
using EduLearn.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.SISService.Data;

public class SISDbContext : DbContext
{
    public SISDbContext(DbContextOptions<SISDbContext> options) : base(options) { }

    // ── Owned tables (write) ──
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Room> Rooms => Set<Room>();

    // ── Referenced tables (read-only) ──
    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<EduLearn.Shared.Entities.Program> Programs => Set<EduLearn.Shared.Entities.Program>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<AuditLog>();
        modelBuilder.Ignore<Syllabus>();
        modelBuilder.Ignore<Content>();
        modelBuilder.Ignore<Discussion>();
        modelBuilder.Ignore<Assessment>();
        modelBuilder.Ignore<Submission>();
        modelBuilder.Ignore<GradeChange>();
        modelBuilder.Ignore<FeeSchedule>();
        modelBuilder.Ignore<Invoice>();
        modelBuilder.Ignore<Payment>();
        modelBuilder.Ignore<Scholarship>();
        modelBuilder.Ignore<Report>();
        modelBuilder.Ignore<KPI>();
        modelBuilder.Ignore<AuditPackage>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<Ticket>();

        // ── User (referenced, read-only) ──
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            entity.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(u => u.AuditLogs);
            entity.Ignore(u => u.Notifications);
        });

        // ── Course (referenced, read-only) ──
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", t => t.ExcludeFromMigrations());
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(c => c.Contents);
            entity.Ignore(c => c.Discussions);
            entity.Ignore(c => c.Assessments);
            entity.Ignore(c => c.Syllabi);
        });

        // ── Program (referenced, read-only) ──
        modelBuilder.Entity<EduLearn.Shared.Entities.Program>(entity =>
        {
            entity.ToTable("Programs", t => t.ExcludeFromMigrations());
            entity.Ignore(p => p.FeeSchedules);
        });

        // ── Student (owned) ──
        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(s => s.EnrollmentStatus).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(s => s.Submissions);
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

        // ── Section (owned) ──
        modelBuilder.Entity<Section>(entity =>
        {
            entity.Ignore(sec => sec.Assessments);

            entity.HasOne(sec => sec.Course)
                  .WithMany(c => c.Sections)
                  .HasForeignKey(sec => sec.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(sec => sec.Instructor)
                  .WithMany(u => u.InstructorSections)
                  .HasForeignKey(sec => sec.InstructorID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(sec => sec.Room)
                  .WithMany(r => r.Sections)
                  .HasForeignKey(sec => sec.RoomID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Applicant (owned) ──
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.Property(a => a.ApplicationStatus).HasConversion<string>().HasMaxLength(30);
        });

        // ── Transcript (owned) ──
        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(t => t.Student)
                  .WithMany(s => s.Transcripts)
                  .HasForeignKey(t => t.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Enrollment (owned) ──
        modelBuilder.Entity<Enrollment>(entity =>
        {
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

        // ── Room (owned) — no special config needed ──
    }
}
