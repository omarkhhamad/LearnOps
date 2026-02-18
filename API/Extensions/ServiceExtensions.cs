using Application.Interfaces.IServices;
using Application.Services;
using Application.UnitOfWork;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Application.Interfaces.IRepositories;
using Infrastructure.Persistence.Common.Repositories;
using Application.Mappings;
using AutoMapper;
using Infrastructure.Services;

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
            services.AddScoped<IExamService, ExamService>();
            // Authentication Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFileService, FileService>();

            // Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IInstructorRepository, InstructorRepository>();
            services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddHttpClient();
		}
	}

}
