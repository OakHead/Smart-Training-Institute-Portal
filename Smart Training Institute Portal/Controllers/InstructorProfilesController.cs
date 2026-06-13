using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_Training_Institute_Portal.Controllers
{
    public class InstructorProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;

		public InstructorProfilesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

		// GET: InstructorProfiles
		[AllowAnonymous]
		public async Task<IActionResult> Index()
        {
			var instructors = await _context.InstructorProfiles
	            .Include(i => i.User)
	            .ToListAsync();

			return View(instructors);
		}

        // GET: InstructorProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: InstructorProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,ImageUrl,Qualifications,Experience,Specialization,OfficeLocation,OfficeHours,UserId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] InstructorProfile instructorProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructorProfile);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor profile created successfully.";
				return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.UserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles.FindAsync(id);
            if (instructorProfile == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.UserId);
            return View(instructorProfile);
        }

        // POST: InstructorProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorId,ImageUrl,Qualifications,Experience,Specialization,OfficeLocation,OfficeHours,UserId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] InstructorProfile instructorProfile)
        {
            if (id != instructorProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructorProfile);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instructor profile updated successfully.";
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorProfileExists(instructorProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.UserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        // POST: InstructorProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructorProfile = await _context.InstructorProfiles.FindAsync(id);
            if (instructorProfile != null)
            {
                _context.InstructorProfiles.Remove(instructorProfile);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Instructor profile deleted successfully.";
			return RedirectToAction(nameof(Index));
        }

        private bool InstructorProfileExists(int id)
        {
            return _context.InstructorProfiles.Any(e => e.Id == id);
        }
		[Authorize(Roles = "Admin,Instructor")]
		public async Task<IActionResult> MyCourses()
		{
			var user = await _userManager.GetUserAsync(User);

			var instructor = await _context.InstructorProfiles
				.FirstOrDefaultAsync(i => i.UserId == user.Id);

			if (instructor == null)
			{
				return NotFound();
			}

			var courses = await _context.Courses
				.Include(c => c.Department)
				.Include(c => c.Instructors)
					.ThenInclude(ci => ci.InstructorProfile)
				.Where(c => c.Instructors.Any(ci => ci.InstructorProfileId == instructor.Id))
				.ToListAsync();

			return View(courses);
		}
		[Authorize(Roles = "Admin,Instructor")]
		public async Task<IActionResult> AddStudent(int courseId)
		{
			ViewBag.Course = await _context.Courses.FindAsync(courseId);

			ViewBag.Students = new SelectList(
				await _context.StudentProfiles
					.Include(s => s.User)
					.ToListAsync(),
				"Id",
				"User.FullName"
			);

			ViewBag.CourseId = courseId;

			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Instructor")]
		public async Task<IActionResult> AddStudent(int courseId, int studentProfileId)
		{
			var exists = await _context.StudentEnrollments.AnyAsync(e =>
				e.CourseId == courseId &&
				e.StudentProfileId == studentProfileId);

			if (exists)
			{
				TempData["Error"] = "Student is already enrolled in this course.";
				return RedirectToAction(nameof(AddStudent), new { courseId });
			}

			var enrollment = new StudentEnrollment
			{
				CourseId = courseId,
				StudentProfileId = studentProfileId,
				EnrollmentDate = DateTime.Now,
				Status = "Enrolled"
			};

			_context.StudentEnrollments.Add(enrollment);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Student added successfully.";

			return RedirectToAction(nameof(MyCourses));
		}
		[Authorize(Roles = "Admin,Instructor")]
		public async Task<IActionResult> CourseStudents(int courseId)
		{
			var course = await _context.Courses
				.Include(c => c.Department)
				.FirstOrDefaultAsync(c => c.Id == courseId);

			if (course == null)
			{
				return NotFound();
			}

			var enrollments = await _context.StudentEnrollments
				.Include(e => e.StudentProfile)
					.ThenInclude(s => s.User)
				.Where(e => e.CourseId == courseId)
				.ToListAsync();

			ViewBag.Course = course;

			return View(enrollments);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Instructor")]
		public async Task<IActionResult> UpdateMarks(int courseId, int[] enrollmentIds, decimal?[] marks, string[] grades)
		{
			for (int i = 0; i < enrollmentIds.Length; i++)
			{
				var enrollment = await _context.StudentEnrollments.FindAsync(enrollmentIds[i]);

				if (enrollment != null)
				{
					enrollment.Mark = marks[i];
					enrollment.Grade = grades[i];
				}
			}

			await _context.SaveChangesAsync();

			TempData["Success"] = "Marks updated successfully.";

			return RedirectToAction(nameof(CourseStudents), new { courseId });
		}
	}
}
