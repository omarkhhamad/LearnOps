using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Infrastructure.Persistence;

namespace Application.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IStudentRepository Students { get; }
        public ICourseRepository Courses { get; }
        public IInstructorRepository Instructors { get; }
        public IClassGroupRepository ClassGroups { get; }
        public IEnrollmentRepository Enrollments { get; }
        public IExamRepository Exams { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public IUserRepository Users { get; }
        public UnitOfWork(AppDbContext context,
                            IStudentRepository studentRepo,
                            ICourseRepository courseRepo,
                            IInstructorRepository instructors,
                            IClassGroupRepository classGroups,
                            IEnrollmentRepository enrollments,
                            IExamRepository exam,
                            IRefreshTokenRepository refreshToken,
                            IUserRepository users)
        {
            _context = context;
            Students = studentRepo;
            Courses = courseRepo;
            Instructors = instructors;
            ClassGroups = classGroups;
            Enrollments = enrollments;
            Exams = exam;
            RefreshTokens = refreshToken;
            Users = users;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
