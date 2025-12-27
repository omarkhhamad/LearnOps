using Application.Interfaces.IServices;
using Application.Services;
using Application.UnitOfWork;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Application.Interfaces.IRepositories;
using Infrastructure.Persistence.Common.Repositories;

namespace API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IInstructorService, InstructorService>();
            // Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IInstructorRepository, InstructorRepository>();
        }
    }

}
