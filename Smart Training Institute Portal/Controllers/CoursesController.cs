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
				.Where(c => c.IsPublished == true && c.IsDeleted != true)
				.ToListAsync();

			return View(courses);
		}

		// GET: Courses
		public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Courses.Include(c => c.Department);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save([Bind("Id,CourseCode,Title,Description,CreditHours,HoursPerWeek,Level,IsPublished,DepartmentId")] Course course)
		{

			if (!ModelState.IsValid)
			{
				ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", course.DepartmentId);
				return View(course.Id == 0 ? "Create" : "Edit", course);
			}

			if (course.Id == 0)
			{
				_context.Courses.Add(course);
			}
			else
			{
				var existingCourse = await _context.Courses.FindAsync(course.Id);

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
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Create()
		{
			ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
			return View("Save", new Course());
		}
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var course = await _context.Courses.FindAsync(id);

			if (course == null)
			{
				return NotFound();
			}

			ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", course.DepartmentId);

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
