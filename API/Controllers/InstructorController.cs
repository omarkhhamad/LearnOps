using Application.DTOs.Instructor;
using Application.Result;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : BaseController
    {
        private readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        /// <summary>
        /// Get all instructors
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of instructors with optional search by name or phone.
        /// </remarks>
        /// <response code="200">Instructors retrieved successfully</response>
        [HttpGet]
        //[ProducesResponseType(typeof(Result<PagedResult<InstructorDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInstructors(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _instructorService.GetAllInstructors(search, page, pageSize);
            return ToActionResult(result);
        }

        /// <summary>
        /// Create a new instructor
        /// </summary>
        /// <param name="instructorDto">Instructor data</param>
        /// <response code="201">Instructor created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        //[ProducesResponseType(typeof(Result<InstructorDto>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddInstructor([FromBody] AddUpdateInstructorDto instructorDto)
        {
            var result = await _instructorService.AddInstructor(instructorDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Update existing instructor
        /// </summary>
        /// <param name="id">Instructor ID</param>
        /// <param name="instructorDto">Updated instructor data</param>
        /// <response code="200">Instructor updated successfully</response>
        /// <response code="404">Instructor not found</response>
        [HttpPut("{id:int}")]
        //[ProducesResponseType(typeof(Result<InstructorDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInstructor(
            int id,
            [FromBody] AddUpdateInstructorDto instructorDto)
        {
            var result = await _instructorService.UpdateInstructor(id, instructorDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Delete instructor
        /// </summary>
        /// <remarks>
        /// Performs a soft delete for the instructor.
        /// </remarks>
        /// <param name="id">Instructor ID</param>
        /// <response code="200">Instructor deleted successfully</response>
        /// <response code="404">Instructor not found</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var result = await _instructorService.DeleteInstructor(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Deletes multiple instructors by their IDs
        /// </summary>
        /// <remarks>
        /// Performs a soft delete for multiple instructors.
        /// </remarks>
        /// <param name="ids">A list of instructor IDs to be deleted</param>
        /// <response code="200">Instructors deleted successfully</response>
        /// <response code="400">Invalid or empty instructor IDs list</response>
        /// <response code="404">No instructors found with the provided IDs</response>
        [HttpDelete("bulk-delete")]
        //[ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            var result = await _instructorService.DeleteInstructors(ids);
            return ToActionResult(result);
        }

        /// <summary>
        /// Get detailed instructor information with groups, courses, and students
        /// </summary>
        [HttpGet("{id}")]
        //[ProducesResponseType(typeof(Result<InstructorDetailedDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInstructorDetailed(int id)
        {
            var result = await _instructorService.GetInstructorDetailedById(id);
            return ToActionResult(result);
        }

    }
}
