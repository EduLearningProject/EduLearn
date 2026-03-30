using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.AnalyticsService.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<AssessmentSubmission> AssessmentSubmissions => Set<AssessmentSubmission>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exclude entities not in this context at all
        modelBuilder.Ignore<CourseMaterial>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<ForumPost>();

        // READ-ONLY context — all tables excluded from migrations (owned by other services)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Ignore(u => u.FacultyCourses);
            entity.Ignore(u => u.Notifications);
            entity.Ignore(u => u.ForumPosts);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", t => t.ExcludeFromMigrations());
            entity.Property(e => e.MaxCapacity).HasDefaultValue(30);
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(c => c.Faculty).WithMany().HasForeignKey(c => c.FacultyId).OnDelete(DeleteBehavior.NoAction);
            entity.Ignore(c => c.Materials);
            entity.Ignore(c => c.ForumPosts);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments", t => t.ExcludeFromMigrations());
            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.Student).WithMany(u => u.Enrollments).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.ToTable("Assessments", t => t.ExcludeFromMigrations());
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(a => a.Course).WithMany(c => c.Assessments).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssessmentSubmission>(entity =>
        {
            entity.ToTable("AssessmentSubmissions", t => t.ExcludeFromMigrations());
            entity.Property(e => e.IsGraded).HasDefaultValue(false);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(s => s.Assessment).WithMany(a => a.Submissions).HasForeignKey(s => s.AssessmentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(s => s.Student).WithMany(u => u.Submissions).HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance", t => t.ExcludeFromMigrations());
            entity.HasOne(a => a.Student).WithMany(u => u.AttendanceRecords).HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(a => a.Course).WithMany(c => c.AttendanceRecords).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(a => a.MarkedByFaculty).WithMany().HasForeignKey(a => a.MarkedBy).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
