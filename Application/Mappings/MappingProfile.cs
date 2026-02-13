using AutoMapper;
using Application.DTOs.Student;
using Application.DTOs.Instructor;
using Application.DTOs.Course;
using Application.DTOs.ClassGroup;
using Application.DTOs.Exam;
using Domain.Models;
using System.Linq;


namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ============ ============
            CreateMap<AddUpdateStudentDto, Student>();
            CreateMap<AddUpdateInstructorDto, Instructor>();
            CreateMap<AddUpdateCourseDto, Course>();
            CreateMap<AddUpdateClassGroupDto, ClassGroup>();

            // ============ Student ============
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));

            CreateMap<Student, StudentDetailedDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.TotalCourses, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.ActiveCourses, opt => opt.MapFrom(src => src.Enrollments.Count(e => e.Status == "Active")))
                .ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments));

            CreateMap<Enrollment, StudentCourseDetailsDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.ClassGroup.Course.CourseId))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.ClassGroup.Course.Title))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.ClassGroup.GroupId))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.ClassGroup.Name))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.EnrollmentDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // ============ Instructor ============
            CreateMap<Instructor, InstructorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.HourlyRate));

            CreateMap<Instructor, InstructorDetailedDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src =>
                    src.ClassGroups.GroupBy(g => g.Course)
                       .Select(g => new CourseWithGroupsDto
                       {
                           CourseId = g.Key.CourseId,
                           CourseName = g.Key.Title,
                           Groups = g.Select(grp => new GroupInfoDto
                           {
                               GroupId = grp.GroupId,
                               GroupName = grp.Name,
                               Room = grp.Room,
                               Days = grp.Days,
                               Time = grp.Time,
                               StartDate = grp.StartDate,
                               EndDate = grp.EndDate,
                               StudentsCount = grp.Enrollments.Count
                           }).ToList()
                       }).ToList()))
                .ForMember(dest => dest.TotalStudents, opt => opt.MapFrom(src => src.ClassGroups.Sum(g => g.Enrollments.Count)))
                .ForMember(dest => dest.ActiveGroups, opt => opt.MapFrom(src => src.ClassGroups.Count(g => g.EndDate > System.DateTime.Now)));

            // ============ Course ============
            CreateMap<Course, CourseDto>();
            CreateMap<Course, CourseDetailedDto>()
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.ClassGroups))
                .ForMember(dest => dest.TotalEnrolledStudents, opt => opt.MapFrom(src => src.ClassGroups.Sum(g => g.Enrollments.Count)))
                .ForMember(dest => dest.ActiveGroups, opt => opt.MapFrom(src => src.ClassGroups.Count(g => g.EndDate > System.DateTime.Now)));

            CreateMap<Course, CourseWithGroupsDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.ClassGroups));

            // ============ ClassGroup ============
            // 1. ClassGroupDto - Lists/Dashboard 
            CreateMap<ClassGroup, ClassGroupDto>()
                .ForMember(dest => dest.CourseName,
                    opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Instructor != null && src.Instructor.User != null ? src.Instructor.User.FullName : null))
                .ForMember(dest => dest.StudentsCount,
                    opt => opt.MapFrom(src => src.Enrollments.Count));

            // 2. ClassGroupDetailedDto -(GET /api/ClassGroup/{id})
            CreateMap<ClassGroup, ClassGroupDetailedDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src =>
                    src.Enrollments.Select(e => e.Student).ToList()));

            // 3. For CourseDetailedDto groups
            CreateMap<ClassGroup, ClassGroupWithInstructorDto>()
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Enrollments.Count));

            // 4. For InstructorDetailedDto groups
            CreateMap<ClassGroup, GroupInfoDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Enrollments.Count));

            // ============ Exam ==================
            // Map Exam -> ExamDto and reverse for create/update
            CreateMap<Exam, ExamDto>();
            CreateMap<ExamDto, Exam>();

            // Map ExamResult -> ClassGroupExamResult for nested exam results
            //CreateMap<ExamResult, ClassGroupExamResult>();

            CreateMap<ExamResult, ClassGroupExamResult>()
            .ForMember(dest => dest.StudentName,
                opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.FullName
                        : "Deleted Student"
                ));


            // Map Exam -> ExamWithClassGroupDto including nested ClassGroup and ExamResults
            CreateMap<Exam, ExamWithClassGroupDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamDate))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                //.ForMember(dest => dest.ClassGroup, opt => opt.MapFrom(src => src.ClassGroup))
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.ExamResults));
        }
    }
}