using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exclude entities not owned by this context
        modelBuilder.Ignore<Course>();
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<CourseMaterial>();
        modelBuilder.Ignore<Assessment>();
        modelBuilder.Ignore<AssessmentSubmission>();
        modelBuilder.Ignore<Attendance>();
        modelBuilder.Ignore<ForumPost>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Ignore(u => u.FacultyCourses);
            entity.Ignore(u => u.Enrollments);
            entity.Ignore(u => u.Submissions);
            entity.Ignore(u => u.AttendanceRecords);
            entity.Ignore(u => u.ForumPosts);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
