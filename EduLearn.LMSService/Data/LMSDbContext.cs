using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.LMSService.Data;

public class LMSDbContext : DbContext
{
    public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseMaterial> CourseMaterials => Set<CourseMaterial>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<AssessmentSubmission> AssessmentSubmissions => Set<AssessmentSubmission>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exclude entities not owned by this context
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<Attendance>();
        modelBuilder.Ignore<Notification>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Ignore(u => u.Enrollments);
            entity.Ignore(u => u.AttendanceRecords);
            entity.Ignore(u => u.Notifications);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.MaxCapacity).HasDefaultValue(30);
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(c => c.Faculty).WithMany(u => u.FacultyCourses).HasForeignKey(c => c.FacultyId).OnDelete(DeleteBehavior.NoAction);
            entity.Ignore(c => c.Enrollments);
            entity.Ignore(c => c.AttendanceRecords);
        });

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.Property(e => e.ModuleNumber).HasDefaultValue(1);
            entity.Property(e => e.OrderIndex).HasDefaultValue(0);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(m => m.Course).WithMany(c => c.Materials).HasForeignKey(m => m.CourseId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(a => a.Course).WithMany(c => c.Assessments).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssessmentSubmission>(entity =>
        {
            entity.Property(e => e.IsGraded).HasDefaultValue(false);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(s => s.Assessment).WithMany(a => a.Submissions).HasForeignKey(s => s.AssessmentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(s => s.Student).WithMany(u => u.Submissions).HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(f => f.Course).WithMany(c => c.ForumPosts).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(f => f.Author).WithMany(u => u.ForumPosts).HasForeignKey(f => f.AuthorId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(f => f.Parent).WithMany(f => f.Replies).HasForeignKey(f => f.ParentId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
