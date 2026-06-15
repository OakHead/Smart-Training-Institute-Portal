using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.DTOs;
using Smart_Training_Institute_Portal.Models;

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
					Prerequisites = c.Prerequisites.Select(p => p.Name),
					EnrollmentCount = c.Enrollments.Count()
				}).ToListAsync();
			return Ok(courses);
		}
		[HttpPost]
		public async Task<IActionResult> CreateCourse(CourseDto dto)
		{
			var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);

			if (!departmentExists)
				return BadRequest("Department not found");

			var course = new Course
			{
				CourseCode = dto.CourseCode,
				Title = dto.Title,
				Description = dto.Description,
				CreditHours = dto.CreditHours,
				HoursPerWeek = dto.HoursPerWeek,
				Level = dto.Level,
				IsPublished = dto.IsPublished,
				DepartmentId = dto.DepartmentId
			};

			var prerequisites = await _context.Prerequisites
				.Where(p => dto.SelectedPrerequisiteIds.Contains(p.Id))
				.ToListAsync();

			foreach (var prerequisite in prerequisites)
				course.Prerequisites.Add(prerequisite);

			foreach (var instructorId in dto.SelectedInstructorIds)
			{
				course.Instructors.Add(new CourseInstructor
				{
					InstructorProfileId = instructorId
				});
			}

			_context.Courses.Add(course);
			await _context.SaveChangesAsync();

			return Ok(course);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCourse(int id, CourseDto dto)
		{
			var course = await _context.Courses
				.Include(c => c.Prerequisites)
				.Include(c => c.Instructors)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (course == null)
				return NotFound();

			course.CourseCode = dto.CourseCode;
			course.Title = dto.Title;
			course.Description = dto.Description;
			course.CreditHours = dto.CreditHours;
			course.HoursPerWeek = dto.HoursPerWeek;
			course.Level = dto.Level;
			course.IsPublished = dto.IsPublished;
			course.DepartmentId = dto.DepartmentId;

			course.Prerequisites.Clear();

			var prerequisites = await _context.Prerequisites
				.Where(p => dto.SelectedPrerequisiteIds.Contains(p.Id))
				.ToListAsync();

			foreach (var prerequisite in prerequisites)
				course.Prerequisites.Add(prerequisite);

			course.Instructors.Clear();

			foreach (var instructorId in dto.SelectedInstructorIds)
			{
				course.Instructors.Add(new CourseInstructor
				{
					CourseId = course.Id,
					InstructorProfileId = instructorId
				});
			}

			await _context.SaveChangesAsync();

			return Ok(course);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			var course = await _context.Courses.FindAsync(id);

			if (course == null)
				return NotFound();

			course.IsDeleted = true;

			await _context.SaveChangesAsync();

			return Ok();
		}
	}
}