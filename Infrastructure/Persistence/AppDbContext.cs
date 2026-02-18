using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
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
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                    var compareExpression = System.Linq.Expressions.Expression.Equal(isDeletedProperty, System.Linq.Expressions.Expression.Constant(false));
                    var lambda = System.Linq.Expressions.Expression.Lambda(compareExpression, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            // ===============================
            // Precision for decimals
            // ===============================
            modelBuilder.Entity<Course>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Instructor>().Property(i => i.HourlyRate).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

            // ===============================
            // Relationships
            // ===============================

            // RefreshToken as separate entity with FK to ApplicationUser
            modelBuilder.Entity<RefreshToken>(t =>
            {
                t.HasKey(rt => rt.Id);
                t.Property(rt => rt.Token).IsRequired();
                t.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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

            // ApplicationUser -> Student (one-to-one)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> Instructor (one-to-one)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.InstructorProfile)
                .WithOne(i => i.User)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Apply any other configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
