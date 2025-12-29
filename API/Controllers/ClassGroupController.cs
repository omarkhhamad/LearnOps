using Application.DTOs.ClassGroup;
using Application.Interfaces.IServices;
using Application.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassGroupController : BaseController
    {
        private readonly IClassGroupService _classGroupService;

        public ClassGroupController(IClassGroupService classGroupService)
        {
            _classGroupService = classGroupService;
        }

        /// <summary>
        /// Get all class groups
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of class groups with optional search by name or course.
        /// </remarks>
        /// <response code="200">Class groups retrieved successfully</response>
        [HttpGet]
        //[ProducesResponseType(typeof(Result<PagedResult<ClassGroupDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllGroups(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _classGroupService.GetAllGroups(search, page, pageSize);
            return ToActionResult(result);
        }

        /// <summary>
        /// Get class group by ID
        /// </summary>
        /// <param name="id">Class group ID</param>
        /// <response code="200">Class group found</response>
        /// <response code="404">Class group not found</response>
        [HttpGet("{id:int}")]
        //[ProducesResponseType(typeof(Result<ClassGroupDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var result = await _classGroupService.GetGroupsByIdAsync(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Get class groups by course ID
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <response code="200">Class groups retrieved successfully</response>
        /// <response code="404">No class groups found for the course</response>
        [HttpGet("by-course/{courseId:int}")]
        //[ProducesResponseType(typeof(Result<ClassGroupDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupsByCourse(int courseId)
        {
            var result = await _classGroupService.GetGroupsByCourseId(courseId);
            return ToActionResult(result);
        }

        /// <summary>
        /// Create a new class group
        /// </summary>
        /// <param name="groupDto">Class group data</param>
        /// <response code="201">Class group created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        //[ProducesResponseType(typeof(Result<ClassGroupDto>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddGroup([FromBody] AddUpdateClassGroupDto groupDto)
        {
            var result = await _classGroupService.AddGroup(groupDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Update existing class group
        /// </summary>
        /// <param name="id">Class group ID</param>
        /// <param name="groupDto">Updated class group data</param>
        /// <response code="200">Class group updated successfully</response>
        /// <response code="404">Class group not found</response>
        [HttpPut("{id:int}")]
        //[ProducesResponseType(typeof(Result<ClassGroupDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] AddUpdateClassGroupDto groupDto)
        {
            var result = await _classGroupService.UpdateGroup(id, groupDto);
            return ToActionResult(result);
        }

        /// <summary>
        /// Delete class group
        /// </summary>
        /// <param name="id">Class group ID</param>
        /// <response code="200">Class group deleted successfully</response>
        /// <response code="404">Class group not found</response>
        [HttpDelete("{id:int}")]
        //[ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var result = await _classGroupService.DeleteGroup(id);
            return ToActionResult(result);
        }

        /// <summary>
        /// Deletes multiple class groups by their IDs
        /// </summary>
        /// <param name="ids">A list of class group IDs to be deleted</param>
        /// <response code="200">Class groups deleted successfully</response>
        /// <response code="400">Invalid or empty IDs list</response>
        /// <response code="404">No class groups found with the provided IDs</response>
        [HttpDelete("bulk-delete")]
        //[ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(Result<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            var result = await _classGroupService.DeleteGroups(ids);
            return ToActionResult(result);
        }
    }
}
