using Application.DTOs.Student;
using Application.Interfaces.IServices;
using Application.Result;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        public IStudentService _studentService;
        public StudentController(IStudentService studentService ) {
            _studentService = studentService;
        }
        [HttpGet]
        [ Route("/api/GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllStudents();
            return ToActionResult(students);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var result = await _studentService.GetStudentById(id);
            return ToActionResult(result); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, AddUpdateStudentDto studentDto)
        {
            var result = await _studentService.UpdateStudent(id, studentDto);
            return ToActionResult(result); 
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(AddUpdateStudentDto studentDto)
        {
            var result = await _studentService.AddStudent(studentDto);
            return ToActionResult(result); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudent(id);
            return ToActionResult(result); 
        }


    }
}
