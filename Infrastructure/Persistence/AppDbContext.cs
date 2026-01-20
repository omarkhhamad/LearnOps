using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Instructor> Instructors { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<ClassGroup> ClassGroups { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<ExamResult> ExamResults { get; set; } = null!;
        public DbSet<Certificate> Certificates { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // Global Query Filters for soft delete
            // ===============================
            modelBuilder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Instructor>().HasQueryFilter(i => !i.IsDeleted);
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ClassGroup>().HasQueryFilter(cg => !cg.IsDeleted);
            modelBuilder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);

            // ===============================
            // Precision for decimals
            // ===============================
            modelBuilder.Entity<Course>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Instructor>().Property(i => i.HourlyRate).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

            // ===============================
            // Relationships
            // ===============================

            // Enrollment -> ClassGroup (many-to-one)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ClassGroup)
                .WithMany(cg => cg.Enrollments)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Enrollment -> Student (many-to-one)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Enrollment -> Certificate (one-to-one, optional)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Certificate)
                .WithOne(c => c.Enrollment)
                .HasForeignKey<Certificate>(c => c.EnrollmentId)
                .IsRequired(false);

            // Exam -> ClassGroup (many-to-one)
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.ClassGroup)
                .WithMany(cg => cg.Exams)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // ExamResult -> Exam (many-to-one)
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany(s => s.ExamResults) 
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Payment -> Enrollment (many-to-one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Enrollment)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Attendance -> Enrollment (many-to-one)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Enrollment)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            // Certificate -> Student (optional)
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Certificates)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Instructor -> ClassGroups (one-to-many)
            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.ClassGroups)
                .WithOne(cg => cg.Instructor)
                .HasForeignKey(cg => cg.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course -> ClassGroups (one-to-many)
            modelBuilder.Entity<Course>()
                .HasMany(c => c.ClassGroups)
                .WithOne(cg => cg.Course)
                .HasForeignKey(cg => cg.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Apply any other configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
