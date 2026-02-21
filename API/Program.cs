using System;
using System.Reflection;
using System.Text;
using API.Extensions;
using API.Middleware;
using Application.Options;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails();
            // ============================
            // JWT AUTHENTICATION
            // ============================
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection(JwtSettings.SectionName)
            );

            var jwtSettings = builder.Configuration
             .GetSection(JwtSettings.SectionName)
             .Get<JwtSettings>()!;

            //var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            var secretKey = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            // ============================
            // GOOGLE AUTHENTICATION
            // ============================
            builder.Services.Configure<GoogleAuthConfig>(
                builder.Configuration.GetSection(GoogleAuthConfig.SectionName)
            );

            var googleConfig = builder.Configuration
                .GetSection(GoogleAuthConfig.SectionName)
                .Get<GoogleAuthConfig>()!;



            // ============================
            // AUTHORIZATION POLICIES
            // ============================
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin")); // Only users with Admin role can access

            });

            // ============================
            // ADD CONTROLLERS & SWAGGER
            // ============================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "LearnOps API",
                    Version = "v1",
                    Description = "API documentation for frontend developers"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // ============================
            // DATABASE CONTEXT (EF CORE)
            // ============================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Infrastructure")
                          .EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                )
            );
            // ============================
            // ASP.NET IDENTITY CONFIGURATION
            // ============================
            builder.Services
             .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
             {
                 // Password settings
                 options.Password.RequireDigit = true;
                 options.Password.RequireLowercase = true;
                 options.Password.RequireUppercase = true;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequiredLength = 6;

                 // User settings
                 options.User.RequireUniqueEmail = true;
             })
             .AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders();

            // Overriding Default Authentication Scheme to JWT (After Identity)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ClockSkew = TimeSpan.Zero
                };
            });


            // ============================
            // CUSTOM APPLICATION SERVICES
            // ============================
            builder.Services.AddApplicationServices();

            // ============================
            // CORS POLICY
            // ============================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://localhost:3000", "http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Allow cookies (HttpOnly) to be sent from the frontend
                });
            });

            // ============================
            // BUILD APP
            // ============================
            var app = builder.Build();

            // ============================
            // SET HOST URLs
            // ============================
            builder.WebHost.UseUrls("http://localhost:5093", "https://localhost:7218");

            // ============================
            // GLOBAL EXCEPTION HANDLING
            // ============================
            app.UseMiddleware<ExceptionMiddleware>();

            // ============================
            // MIDDLEWARE PIPELINE
            // ============================
            app.UseHttpsRedirection();      // Redirect HTTP to HTTPS
            app.UseStaticFiles();           // Enable serving static files (for uploads)
            app.UseCors("AllowFrontend");    // Enable CORS
            app.UseAuthentication();        // Enable Authentication
            app.UseAuthorization();         // Enable Authorization

            // ============================
            // SWAGGER IN DEVELOPMENT
            // ============================
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            // ============================
            // MAP CONTROLLERS
            // ============================
            app.MapControllers();


            // ============================
            // RUN THE APPLICATION
            // ============================
            app.Run();
        }
    }
}
