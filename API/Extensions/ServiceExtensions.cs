using Application.Interfaces.IServices;
using Application.Services;
using Application.UnitOfWork;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Application.Interfaces.IRepositories;
using Infrastructure.Persistence.Common.Repositories;
using Application.Mappings;
using AutoMapper;

namespace API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<IClassGroupService, ClassGroupService>();
            // Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IInstructorRepository, InstructorRepository>();
            services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        }
    }

}
