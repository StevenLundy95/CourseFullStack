using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseAPI.Data;
using CourseAPI.Dtos;
using CourseAPI.Models;

namespace CourseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // Existing methods...

        [HttpGet("{id}", Name = "GetCourse")] // Specify a name for the route
        public async Task<ActionResult<GetCourseDto>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            var courseDto = new GetCourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                NoOfChapters = course.NoOfChapters,
                InstructorId = course.InstructorId
            };

            return courseDto;
        }

        [HttpPost]
        public async Task<ActionResult<GetCourseDto>> PostCourse(CreateCourseDto newCourse)
        {
            var course = new Course
            {
                Name = newCourse.Name,
                Description = newCourse.Description,
                NoOfChapters = newCourse.NoOfChapters,
                InstructorId = newCourse.InstructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var addedCourseDto = new GetCourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                NoOfChapters = course.NoOfChapters,
                InstructorId = course.InstructorId
            };

            return CreatedAtRoute("GetCourse", new { id = addedCourseDto.Id }, addedCourseDto); // Using the named route
        }

        // Other methods...
    }
}
