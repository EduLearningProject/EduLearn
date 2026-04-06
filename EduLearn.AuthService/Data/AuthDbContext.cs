using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<Student>();
        modelBuilder.Ignore<Applicant>();
        modelBuilder.Ignore<Transcript>();
        modelBuilder.Ignore<Course>();
        modelBuilder.Ignore<EduLearn.Shared.Entities.Program>();
        modelBuilder.Ignore<Syllabus>();
        modelBuilder.Ignore<Section>();
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<Room>();
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

        // ── User (owned) ──
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            entity.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);

            // Ignore nav properties pointing to entities not in this context
            entity.Ignore(u => u.Student);
            entity.Ignore(u => u.InstructorSections);
            entity.Ignore(u => u.Notifications);
        });

        // ── AuditLog (owned) ──
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
