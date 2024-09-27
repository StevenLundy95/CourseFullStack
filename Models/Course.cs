// File: Models/Course.cs
namespace CourseAPI.Models
{
    public class Course
    {
        public int Id { get; set; } // Primary Key
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int NoOfChapters { get; set; }
        public required string InstructorId { get; set; } // Adjust type as needed
    }
}
