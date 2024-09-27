using Microsoft.EntityFrameworkCore;
using CourseAPI.Models; // This should match the namespace in Course.cs

namespace CourseAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; } // Ensure this points to your Course model
    }
}
