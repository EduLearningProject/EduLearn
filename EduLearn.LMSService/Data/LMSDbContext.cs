using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.LMSService.Data;

public class LMSDbContext : DbContext
{
    public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options) { }

    // ── Owned tables (write) ──
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<EduLearn.Shared.Entities.Program> Programs => Set<EduLearn.Shared.Entities.Program>();
    public DbSet<Syllabus> Syllabi => Set<Syllabus>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<Discussion> Discussions => Set<Discussion>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<GradeChange> GradeChanges => Set<GradeChange>();

    // ── Referenced tables (read-only) ──
    public DbSet<User> Users => Set<User>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<AuditLog>();
        modelBuilder.Ignore<Applicant>();
        modelBuilder.Ignore<Transcript>();
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<Room>();
        modelBuilder.Ignore<FeeSchedule>();
        modelBuilder.Ignore<Invoice>();
        modelBuilder.Ignore<Payment>();
        modelBuilder.Ignore<Scholarship>();
        modelBuilder.Ignore<Report>();
        modelBuilder.Ignore<KPI>();
        modelBuilder.Ignore<AuditPackage>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<Ticket>();

        // ── User (referenced) ──
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Ignore(u => u.AuditLogs);
            entity.Ignore(u => u.Notifications);
        });

        // ── Section (referenced) ──
        modelBuilder.Entity<Section>(entity =>
        {
            entity.ToTable("Sections", t => t.ExcludeFromMigrations());
            entity.Ignore(sec => sec.Room);
            entity.Ignore(sec => sec.Enrollments);

            entity.HasOne(sec => sec.Course)
                  .WithMany(c => c.Sections)
                  .HasForeignKey(sec => sec.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(sec => sec.Instructor)
                  .WithMany(u => u.InstructorSections)
                  .HasForeignKey(sec => sec.InstructorID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Student (referenced) ──
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students", t => t.ExcludeFromMigrations());
            entity.Ignore(s => s.Enrollments);
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

        // ── Program (owned) ──
        modelBuilder.Entity<EduLearn.Shared.Entities.Program>(entity =>
        {
            entity.Ignore(p => p.FeeSchedules);
        });

        // ── Course (owned) ──

        // ── Syllabus (owned) ──
        modelBuilder.Entity<Syllabus>(entity =>
        {
            entity.HasOne(sy => sy.Course)
                  .WithMany(c => c.Syllabi)
                  .HasForeignKey(sy => sy.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(sy => sy.CreatedBy)
                  .WithMany()
                  .HasForeignKey(sy => sy.CreatedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Content (owned) ──
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasOne(ct => ct.Course)
                  .WithMany(c => c.Contents)
                  .HasForeignKey(ct => ct.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(ct => ct.UploadedBy)
                  .WithMany()
                  .HasForeignKey(ct => ct.UploadedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Discussion (owned) ──
        modelBuilder.Entity<Discussion>(entity =>
        {
            entity.HasOne(d => d.Course)
                  .WithMany(c => c.Discussions)
                  .HasForeignKey(d => d.CourseID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.ThreadStarter)
                  .WithMany()
                  .HasForeignKey(d => d.ThreadStarterID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Assessment (owned) ──
        modelBuilder.Entity<Assessment>(entity =>
        {
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

        // ── Submission (owned) ──
        modelBuilder.Entity<Submission>(entity =>
        {
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

        // ── GradeChange (owned) ──
        modelBuilder.Entity<GradeChange>(entity =>
        {
            entity.HasOne(gc => gc.Submission)
                  .WithMany(s => s.GradeChanges)
                  .HasForeignKey(gc => gc.SubmissionID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(gc => gc.ChangedBy)
                  .WithMany()
                  .HasForeignKey(gc => gc.ChangedByFK)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
