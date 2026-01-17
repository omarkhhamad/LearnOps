using Application.DTOs.Exam;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        /// <summary>
        /// Get all exams with their class group information
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var result = await _examService.GetAllExams();

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get a specific exam by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var result = await _examService.GetExamById(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new exam
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDto exam)
        {
            var result = await _examService.CreateExam(exam);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return StatusCode(201, result.Data);
        }

        /// <summary>
        /// Delete an exam by ID
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExam(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Update an existing exam
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] ExamDto exam)
        {
            var result = await _examService.UpdateExam(exam, id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all exams for a specific group
        /// </summary>
        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetExamsByGroupId(int groupId)
        {
            var result = await _examService.GetExamsByGroupIdAsync(groupId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all exams for a specific course
        /// </summary>
        [HttpGet("course/{courseId:int}")]
        public async Task<IActionResult> GetExamsByCourseId(int courseId)
        {
            var result = await _examService.GetExamsByCourseIdAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get exam with all its results
        /// </summary>
        [HttpGet("{examId:int}/results")]
        public async Task<IActionResult> GetExamWithResults(int examId)
        {
            var result = await _examService.GetExamWithResultsAsync(examId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            return Ok(result.Data);
        }
    }
}