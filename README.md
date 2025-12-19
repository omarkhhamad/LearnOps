# LearnOps - Learning Management System

## Overview
LearnOps is a Learning Management System (LMS) built using .NET 8 and Clean Architecture. It supports managing students, courses, enrollments, exams, payments, attendance, and certificates.

## Architecture
The project uses Clean Architecture with separation of concerns:
- **Domain**: Contains core data models and business logic.
- **Application**: Contains application services, DTOs, and repository interfaces.
- **Infrastructure**: Contains database implementation and external services.
- **API**: Contains API controllers and application configurations.

## Project Structure

```
LearnOps/
├── diagram.html
├── LearnOps.sln
├── API/
│   ├── API.csproj
│   ├── API.csproj.user
│   ├── API.http
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Program.cs
│   ├── Controllers/
│   │   ├── BaseController.cs
│   │   └── StudentController.cs
│   ├── DependencyInjection/
│   │   └── Extensions/
│   │       └── ServiceExtensions.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── obj/
│   ├── Properties/
│   │   └── launchSettings.json
│   └── wwwroot/
│       └── Images/
├── Application/
│   ├── Application.csproj
│   ├── Application.csproj.user
│   ├── DTOs/
│   │   └── Student/
│   │       ├── AddUpdateStudentDto.cs
│   │       └── StudentDto.cs
│   ├── Interfaces/
│   │   ├── IRepositories/
│   │   │   ├── IBaseRepository.cs
│   │   │   └── IStudentRepository.cs
│   │   └── IServices/
│   │       └── IStudentService.cs
│   ├── obj/
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Result/
│   │   ├── Result.cs
│   │   └── ResultT.cs
│   ├── Services/
│   │   ├── CourseService.cs
│   │   └── StudentService.cs
│   └── UnitOfWork/
│       └── IUnitOfWork.cs
├── Domain/
│   ├── Domain.csproj
│   ├── Domain.csproj.user
│   ├── Models/
│   │   ├── Attendance.cs
│   │   ├── Certificate.cs
│   │   ├── ClassGroup.cs
│   │   ├── Course.cs
│   │   ├── Enrollment.cs
│   │   ├── Exam.cs
│   │   ├── ExamResult.cs
│   │   ├── Instructor.cs
│   │   ├── Payment.cs
│   │   └── Student.cs
│   ├── obj/
│   └── Properties/
│       └── launchSettings.json
└── Infrastructure/
    ├── Infrastructure.csproj
    ├── Infrastructure.csproj.user
    ├── obj/
    ├── Persistence/
    │   ├── AppDbContext.cs
    │   ├── Common/
    │   │   ├── UnitOfWork.cs
    │   │   └── Repositories/
    │   └── Migrations/
    │       ├── 20251214204305_InitialCreate.cs
    │       ├── 20251214204305_InitialCreate.Designer.cs
    │       └── AppDbContextModelSnapshot.cs
    └── Properties/
        └── launchSettings.json
```

## Requirements
- .NET 8.0
- SQL Server
- Visual Studio 2022 or any .NET-supporting IDE

## How to Run
1. Clone the repository.
2. Open the solution in Visual Studio.
3. Update the connection string in `appsettings.json`.
4. Run migrations to create the database.
5. Run the project from API.

## Features
- Student management
- Course management
- Enrollment system
- Exam and result management
- Payment tracking
- Attendance recording
- Certificate issuance

## Contributing
Please read the contribution guide before starting.

## License
This project is licensed under the MIT License.