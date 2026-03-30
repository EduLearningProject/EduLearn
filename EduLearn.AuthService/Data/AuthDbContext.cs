using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.Ignore(u => u.FacultyCourses);
            entity.Ignore(u => u.Enrollments);
            entity.Ignore(u => u.Submissions);
            entity.Ignore(u => u.AttendanceRecords);
            entity.Ignore(u => u.Notifications);
            entity.Ignore(u => u.ForumPosts);
        });

        // Exclude entities not owned by this context
        modelBuilder.Ignore<Course>();
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<CourseMaterial>();
        modelBuilder.Ignore<Assessment>();
        modelBuilder.Ignore<AssessmentSubmission>();
        modelBuilder.Ignore<Attendance>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<ForumPost>();
    }
}
