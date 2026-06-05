using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart_Training_Institute_Portal.Models;
using Smart_Training_Institute_Portal.Data;
using Microsoft.EntityFrameworkCore;

namespace Smart_Training_Institute_Portal.Controllers.API_Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CourseAPIController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetCourses()
		{
			var courses = await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.Instructors)
				.ThenInclude(ci => ci.InstructorProfile)
				.ThenInclude(ip => ip.User)
				.Include(c => c.Prerequisites)
				.Include(c => c.Enrollments)
				.Where(c => c.IsPublished == true && c.IsDeleted != true)
				.Select(c => new
				{
					c.Id,
					c.CourseCode,
					c.Title,
					c.Description,
					c.CreditHours,
					c.HoursPerWeek,
					c.Level,
					DepartmentName = c.Department.Name,
					Instructors = c.Instructors.Select(ci => ci.InstructorProfile.User.FullName),
					EnrollmentCount = c.Enrollments.Count()
				}).ToListAsync();
			return Ok(courses);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			var course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				return NotFound();
			}

			_context.Courses.Remove(course);
			await _context.SaveChangesAsync();

			return Ok();
		}
	}
}