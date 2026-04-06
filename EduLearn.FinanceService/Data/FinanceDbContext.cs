using EduLearn.Shared.Entities;
using EduLearn.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduLearn.FinanceService.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    // ── Owned tables (write) ──
    public DbSet<FeeSchedule> FeeSchedules => Set<FeeSchedule>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();

    // ── Referenced tables (read-only) ──
    public DbSet<Student> Students => Set<Student>();
    public DbSet<EduLearn.Shared.Entities.Program> Programs => Set<EduLearn.Shared.Entities.Program>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Ignore entities not in this context ──
        modelBuilder.Ignore<AuditLog>();
        modelBuilder.Ignore<Applicant>();
        modelBuilder.Ignore<Transcript>();
        modelBuilder.Ignore<Course>();
        modelBuilder.Ignore<Syllabus>();
        modelBuilder.Ignore<Section>();
        modelBuilder.Ignore<Enrollment>();
        modelBuilder.Ignore<Room>();
        modelBuilder.Ignore<Content>();
        modelBuilder.Ignore<Discussion>();
        modelBuilder.Ignore<Assessment>();
        modelBuilder.Ignore<Submission>();
        modelBuilder.Ignore<GradeChange>();
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
            entity.Ignore(u => u.InstructorSections);
            entity.Ignore(u => u.Notifications);
        });

        // ── Student (referenced, read-only) ──
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students", t => t.ExcludeFromMigrations());
            entity.Property(s => s.EnrollmentStatus).HasConversion<string>().HasMaxLength(20);
            entity.Ignore(s => s.Enrollments);
            entity.Ignore(s => s.Submissions);
            entity.Ignore(s => s.Transcripts);

            entity.HasOne(s => s.User)
                  .WithOne(u => u.Student)
                  .HasForeignKey<Student>(s => s.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Program)
                  .WithMany(p => p.Students)
                  .HasForeignKey(s => s.ProgramID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Program (referenced, read-only) ──
        modelBuilder.Entity<EduLearn.Shared.Entities.Program>(entity =>
        {
            entity.ToTable("Programs", t => t.ExcludeFromMigrations());
        });

        // ── FeeSchedule (owned) ──
        modelBuilder.Entity<FeeSchedule>(entity =>
        {
            entity.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(f => f.Program)
                  .WithMany(p => p.FeeSchedules)
                  .HasForeignKey(f => f.ProgramID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Invoice (owned) ──
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(i => i.Student)
                  .WithMany(s => s.Invoices)
                  .HasForeignKey(i => i.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Payment (owned) ──
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.Method).HasConversion<string>().HasMaxLength(30);
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(p => p.Invoice)
                  .WithMany(i => i.Payments)
                  .HasForeignKey(p => p.InvoiceID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Scholarship (owned) ──
        modelBuilder.Entity<Scholarship>(entity =>
        {
            entity.Property(sc => sc.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(sc => sc.Student)
                  .WithMany(s => s.Scholarships)
                  .HasForeignKey(sc => sc.StudentID)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
