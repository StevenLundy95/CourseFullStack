namespace CourseAPI.Dtos
{
    public class GetCourseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int NoOfChapters { get; set; }
        public required string InstructorId { get; set; }
    }
}
