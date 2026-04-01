using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.SISService.Data;

public class SISDbContext : DbContext
{
    public SISDbContext(DbContextOptions<SISDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exclude entities not owned by this context
        modelBuilder.Ignore<CourseMaterial>();
        modelBuilder.Ignore<Assessment>();
        modelBuilder.Ignore<AssessmentSubmission>();
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<ForumPost>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Ignore(u => u.FacultyCourses);
            entity.Ignore(u => u.Submissions);
            entity.Ignore(u => u.Notifications);
            entity.Ignore(u => u.ForumPosts);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", t => t.ExcludeFromMigrations());
            entity.HasOne(c => c.Faculty).WithMany().HasForeignKey(c => c.FacultyId).OnDelete(DeleteBehavior.NoAction);
            entity.Ignore(c => c.Materials);
            entity.Ignore(c => c.Assessments);
            entity.Ignore(c => c.ForumPosts);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasOne(e => e.Student).WithMany(u => u.Enrollments).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasOne(a => a.Student).WithMany(u => u.AttendanceRecords).HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(a => a.Course).WithMany(c => c.AttendanceRecords).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(a => a.MarkedByFaculty).WithMany().HasForeignKey(a => a.MarkedBy).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
