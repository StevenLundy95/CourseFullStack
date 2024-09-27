using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CourseAPI.Data;
using CourseAPI.Dtos;
using CourseAPI.Models;
using CourseAPI.Hubs;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR services
builder.Services.AddSignalR();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course API", Version = "v1" });

    // Include XML comments if available for Swagger documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactAppOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

var app = builder.Build();

// Configure middleware
app.UseCors("AllowReactAppOrigin"); // Enable CORS
app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at the /swagger route
});

// Map your SignalR hubs
app.MapHub<CourseHub>("/courseHub");
// Define API endpoints

// GET all courses
app.MapGet("courses", async (AppDbContext context) =>
    await context.Courses.Select(course => new GetCourseDto
    {
        Id = course.Id,
        Name = course.Name,
        Description = course.Description,
        NoOfChapters = course.NoOfChapters,
        InstructorId = course.InstructorId
    }).ToListAsync());

// GET a specific course by ID
app.MapGet("courses/{id}", async (AppDbContext context, int id) =>
{
    var course = await context.Courses.FindAsync(id);
    if (course == null) return Results.NotFound();

    var courseDto = new GetCourseDto
    {
        Id = course.Id,
        Name = course.Name,
        Description = course.Description,
        NoOfChapters = course.NoOfChapters,
        InstructorId = course.InstructorId
    };

    return Results.Ok(courseDto);
}).WithName("GetCourse"); // Naming the route

// POST a new course
app.MapPost("courses", async (AppDbContext context, CreateCourseDto newCourse, IHubContext<CourseHub> hubContext) =>
{
    try
    {
        var course = new Course
        {
            Name = newCourse.Name,
            Description = newCourse.Description,
            NoOfChapters = newCourse.NoOfChapters,
            InstructorId = newCourse.InstructorId
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var courseDto = new GetCourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            NoOfChapters = course.NoOfChapters,
            InstructorId = course.InstructorId
        };

        // Notify clients about the new course
        await hubContext.Clients.All.SendAsync("ReceiveCourseUpdate", courseDto);

        return Results.CreatedAtRoute("GetCourse", new { id = course.Id }, courseDto);
    }
    catch (Exception ex)
    {
        // Log the exception (using a logging framework or Console for simplicity)
        Console.WriteLine($"Error: {ex.Message}");
        return Results.StatusCode(500); // Return a 500 Internal Server Error
    }
});

// DELETE a specific course by ID
app.MapDelete("courses/{id}", async (AppDbContext context, int id) =>
{
    var course = await context.Courses.FindAsync(id);
    if (course == null) return Results.NotFound();

    context.Courses.Remove(course);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE all courses
app.MapDelete("courses", async (AppDbContext context) =>
{
    var courses = await context.Courses.ToListAsync();
    context.Courses.RemoveRange(courses);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Run the application
app.Run();
