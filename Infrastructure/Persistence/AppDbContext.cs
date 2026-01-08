using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Define DbSets 
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ClassGroup> ClassGroups { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Similar query filters for other entities with IsDeleted property

            modelBuilder.Entity<Student>()
              .HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<Enrollment>()
               .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Course>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Instructor>()
                .HasQueryFilter(i => !i.IsDeleted);

            modelBuilder.Entity<ClassGroup>()
                .HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Instructor>()
                .Property(i => i.HourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);


            // Explicit relationship configurations to avoid shadow properties and clarify optionality

            // Enrollment -> ClassGroup (many-to-one)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ClassGroup)
                .WithMany(cg => cg.Enrollments)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Enrollment -> Student (many-to-one)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Enrollment -> Certificate (one-to-one, optional)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Certificate)
                .WithOne(c => c.Enrollment)
                .HasForeignKey<Certificate>(c => c.EnrollmentId)
                .IsRequired(false);

            // Certificate -> Student (many-to-one, optional)
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Certificates)
                .HasForeignKey(c => c.StudentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Exam -> ClassGroup (many-to-one)
            modelBuilder.Entity<Exam>()
                .HasOne(x => x.ClassGroup)
                .WithMany(g => g.Exams)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // ExamResult -> Enrollment (many-to-one)
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Enrollment)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // ExamResult -> Exam (many-to-one)
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(ex => ex.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Payment -> Enrollment (many-to-one)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Enrollment)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Attendance -> Enrollment (many-to-one)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Enrollment)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


    }
}
