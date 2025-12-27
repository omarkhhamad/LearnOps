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

            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Instructor>()
                .Property(i => i.HourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ClassGroup>()
                .HasKey(cg => cg.GroupId);

            modelBuilder.Entity<ExamResult>()
                .HasKey(e => e.ResultId);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


    }
}
