using Application.DTOs.Course;
using Application.Interfaces.IServices;
using Application.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Get all courses
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of all courses in the system. You can search by title or description.
        /// </remarks>
        /// <param name="search">Optional search term to filter courses</param>
        /// <param name="page">Page number (default is 1)</param>
        /// <param name="pageSize">Number of items per page (default is 10)</param>
        /// <response code="200">Courses retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(Result<PagedResult<CourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCourses([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var courses = await _courseService.GetAllCourses(search, page, pageSize);
            return ToActionResult(courses);
        }

        /// <summary>
        /// Get course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <response code="200">Course found</response>
        /// <response code="404">Course not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var result = await _courseService.GetCourseById(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="courseDto">Course data</param>
        /// <response code="201">Course created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCourse([FromBody] AddUpdateCourseDto courseDto)
        {
            var result = await _courseService.AddCourse(courseDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Update an existing course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="courseDto">Updated course data</param>
        /// <response code="200">Course updated successfully</response>
        /// <response code="404">Course not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] AddUpdateCourseDto courseDto)
        {
            var result = await _courseService.UpdateCourse(id, courseDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Delete a course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <response code="200">Course deleted successfully</response>
        /// <response code="404">Course not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourse(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Delete multiple courses by their IDs
        /// </summary>
        /// <param name="ids">List of course IDs to delete</param>
        /// <response code="200">Courses deleted successfully</response>
        /// <response code="400">Invalid or empty list of IDs</response>
        /// <response code="404">No courses found for the provided IDs</response>
        [HttpDelete("bulk-delete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkDeleteCourses([FromBody] List<int> ids)
        {
            var result = await _courseService.DeleteCourses(ids);
            return ToActionResult(result);
        }
    }
}
