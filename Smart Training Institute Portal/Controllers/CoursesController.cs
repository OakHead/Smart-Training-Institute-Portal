using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;
using Microsoft.AspNetCore.Authorization;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
		public async Task<IActionResult> Catalog()
		{
			var courses = await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.Prerequisites)
				.Where(c => c.IsPublished && c.IsDeleted != true)
				.ToListAsync();

			return View(courses);
		}

		// GET: Courses
		public async Task<IActionResult> Index()
		{
			var courses = await _context.Courses
				.Include(c => c.Department)
				.Where(c => c.IsDeleted != true)
				.ToListAsync();

			return View(courses);
		}

		// GET: Courses/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
			var course = await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.Prerequisites)
				.Include(c => c.Instructors)
					.ThenInclude(ci => ci.InstructorProfile)
						.ThenInclude(ip => ip.User)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save([Bind("Id,CourseCode,Title,Description,CreditHours,HoursPerWeek,Level,IsPublished,DepartmentId")] Course course,int[] SelectedPrerequisiteIds,int[] SelectedInstructorIds)
		{
			if (!ModelState.IsValid)
			{
				ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", course.DepartmentId);
				ViewData["PrerequisiteIds"] = new MultiSelectList(_context.Prerequisites, "Id", "Name", SelectedPrerequisiteIds);

				return View(course.Id == 0 ? "Create" : "Edit", course);
			}

			if (course.Id == 0)
			{
				var selectedPrerequisites = await _context.Prerequisites
					.Where(p => SelectedPrerequisiteIds.Contains(p.Id))
					.ToListAsync();

				foreach (var prerequisite in selectedPrerequisites)
				{
					course.Prerequisites.Add(prerequisite);
				}

				foreach (var instructorId in SelectedInstructorIds)
				{
					course.Instructors.Add(new CourseInstructor
					{
						InstructorProfileId = instructorId
					});
				}

				_context.Courses.Add(course);
			}
			else
			{
				var existingCourse = await _context.Courses
					.Include(c => c.Prerequisites)
					.FirstOrDefaultAsync(c => c.Id == course.Id);

				if (existingCourse == null)
				{
					return NotFound();
				}

				existingCourse.CourseCode = course.CourseCode;
				existingCourse.Title = course.Title;
				existingCourse.Description = course.Description;
				existingCourse.CreditHours = course.CreditHours;
				existingCourse.HoursPerWeek = course.HoursPerWeek;
				existingCourse.Level = course.Level;
				existingCourse.IsPublished = course.IsPublished;
				existingCourse.DepartmentId = course.DepartmentId;

				existingCourse.Prerequisites.Clear();

				var selectedPrerequisites = await _context.Prerequisites
					.Where(p => SelectedPrerequisiteIds.Contains(p.Id))
					.ToListAsync();

				foreach (var prerequisite in selectedPrerequisites)
				{
					existingCourse.Prerequisites.Add(prerequisite);
				}
				existingCourse.Instructors.Clear();

				foreach (var instructorId in SelectedInstructorIds)
				{
					existingCourse.Instructors.Add(new CourseInstructor
					{
						CourseId = existingCourse.Id,
						InstructorProfileId = instructorId
					});
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Create()
		{

			ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
			ViewData["PrerequisiteIds"] = new MultiSelectList(_context.Prerequisites, "Id", "Name");
			ViewBag.Prerequisites = _context.Prerequisites.ToList();
			ViewBag.Instructors = _context.InstructorProfiles.Include(i => i.User).ToList();

			return View("Save", new Course());
		}
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var course = await _context.Courses
				.Include(c => c.Prerequisites)
				.Include(c => c.Instructors)
					.ThenInclude(ci => ci.InstructorProfile)
						.ThenInclude(ip => ip.User)
				.FirstOrDefaultAsync(c => c.Id == id);

			ViewBag.Instructors = await _context.InstructorProfiles
				.Include(i => i.User)
				.ToListAsync();

			if (course == null)
			{
				return NotFound();
			}

			ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", course.DepartmentId);
			ViewBag.Prerequisites = await _context.Prerequisites.ToListAsync();

			return View("Save", course);
		}
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
