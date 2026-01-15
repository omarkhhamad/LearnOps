using Application.DTOs.Exam;
using Application.Interfaces.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var exams = await _examService.GetAllExams();
            return Ok(exams);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var exam = await _examService.GetExamById(id);
            return Ok(exam);
        }
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamDTO exam)
        {
            var createdExam = await _examService.CreateExam(exam);
            return Ok(createdExam);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExam(id);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateExam([FromBody] ExamDTO exam, int id)
        {
            var updatedExam = await _examService.UpdateExam(exam, id);
            return Ok(updatedExam);
        }
        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetExamsByGroupId(int groupId)
        {
            var exams = await _examService.GetExamsByGroupIdAsync(groupId);
            return Ok(exams);

        }
        [HttpGet("course/{courseId:int}")]
        public async Task<IActionResult> GetExamsByCourseId(int courseId)
        {
            var exams = await _examService.GetExamsByCourseIdAsync(courseId);
            return Ok(exams);
        }
        [HttpGet("{examId:int}/results")]
        public async Task<IActionResult> GetExamWithResults(int examId)
        {
            var exam = await _examService.GetExamWithResultsAsync(examId);
            if (exam == null)
            {
                return NotFound();
            }
            return Ok(exam);
        }
    }
}
