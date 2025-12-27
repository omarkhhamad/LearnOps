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

        public UnitOfWork(AppDbContext context,
                            IStudentRepository studentRepo, 
                            ICourseRepository courseRepo,
                            IInstructorRepository instructors)
        {
            _context = context;
            Students = studentRepo;
            Courses = courseRepo;
            Instructors = instructors;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
