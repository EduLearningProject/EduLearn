using EduLearn.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    // ── Owned tables (write) ──
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    // ── Referenced tables (read-only) ──
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<AuditLog>();
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

        // ── User (referenced, read-only) ──
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", t => t.ExcludeFromMigrations());
            entity.Ignore(u => u.AuditLogs);
            entity.Ignore(u => u.Student);
            entity.Ignore(u => u.InstructorSections);
        });

        // ── Notification (owned) ──
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(n => n.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(n => n.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Ticket (owned) ──
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
    }
}
