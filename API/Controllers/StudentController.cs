using Application.DTOs.Student;
using Application.Interfaces.IServices;
using Application.Result;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : BaseController
    {
        public IStudentService _studentService;
        public StudentController(IStudentService studentService ) {
            _studentService = studentService;
        }
        /// <summary>
        /// Get all students
        /// </summary>
        /// <remarks>
        /// Returns a list of all students in the system.
        /// </remarks>
        [HttpGet]
        //[ProducesResponseType(typeof(Result<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStudents([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var students = await _studentService.GetAllStudents(search, page, pageSize);
            return ToActionResult(students);
        }
        /// <summary>
        /// Update existing student
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="studentDto">Updated student data</param>
        [HttpPut("{id}")]
        //[ProducesResponseType(typeof(Result<StudentDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] AddUpdateStudentDto studentDto)
        {
            var result = await _studentService.UpdateStudent(id, studentDto);
            return ToActionResult(result); 
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        /// <param name="studentDto">Student data</param>
        [HttpPost]
        //[ProducesResponseType(typeof(Result<StudentDto>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddStudent([FromBody] AddUpdateStudentDto studentDto)
        {
            var result = await _studentService.AddStudent(studentDto);
            return ToActionResult(result); 
        }

        /// <summary>
        /// Delete student
        /// </summary>
        /// <param name="id">Student ID</param>

        [HttpDelete("{id}")]
        //[ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);
            return ToActionResult(result);
        }
        /// <summary>
        /// Deletes multiple students by their IDs.
        /// </summary>
        /// <param name="ids">A list of student IDs to be deleted.</param>
        [HttpDelete("bulk-delete")]
        //[ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            var result = await _studentService.DeleteStudents(ids);
            return ToActionResult(result);
        }

        /// <summary>
        /// Get detailed student information with enrollments, courses, and groups
        /// </summary>
        [HttpGet("{id}")]
        //[ProducesResponseType(typeof(Result<StudentDetailedDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentDetailed(int id)
        {
            var result = await _studentService.GetStudentDetailedById(id);
            return ToActionResult(result);
        }

    }
}
