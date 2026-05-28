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
using Microsoft.AspNetCore.Identity;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin , Instructor")]
	public class StudentEnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
		public StudentEnrollmentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
			_userManager = userManager;

		}

        // GET: StudentEnrollments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentEnrollments.Include(s => s.Course).Include(s => s.StudentProfile).OrderByDescending(e => e.EnrollmentDate);
			return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (studentProfile == null)
            {
                return NotFound();
            }
            var myEnrollments = await _context.StudentEnrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Department)
				.Where(e => e.StudentProfileId == studentProfile.Id)
                .ToListAsync();
            return View(myEnrollments);
		}
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyGrades()
        {
            var userId = _userManager.GetUserId(User);
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (studentProfile == null)
            {
                return NotFound();
			}
            var myEnrollments = await _context.StudentEnrollments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id)
                .ToListAsync();
            return View(myEnrollments);
		}
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyProgress()
        {
            var userId = _userManager.GetUserId(User);
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (studentProfile == null)
            {
                return NotFound();
            }
            var myEnrollments = await _context.StudentEnrollments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id && e.Mark != null)
                .ToListAsync();

            var avgMark = myEnrollments.Any() ? myEnrollments.Average(e => e.Mark ?? 0) : 0;

            ViewBag.AverageMark = avgMark;
            ViewBag.TotalCourses = myEnrollments.Count;
			return View(myEnrollments);
		}

		// GET: StudentEnrollments/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentEnrollment = await _context.StudentEnrollments
                .Include(s => s.Course)
                .Include(s => s.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentEnrollment == null)
            {
                return NotFound();
            }

            return View(studentEnrollment);
        }

        // GET: StudentEnrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "UserId");
            return View();
        }

        // POST: StudentEnrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,Grade,Mark,Status,StudentProfileId,CourseId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] StudentEnrollment studentEnrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentEnrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student enrollment created successfully.";
				return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", studentEnrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "UserId", studentEnrollment.StudentProfileId);
            return View(studentEnrollment);
        }

        // GET: StudentEnrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentEnrollment = await _context.StudentEnrollments.FindAsync(id);
            if (studentEnrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", studentEnrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "UserId", studentEnrollment.StudentProfileId);
            return View(studentEnrollment);
        }

        // POST: StudentEnrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentDate,Grade,Mark,Status,StudentProfileId,CourseId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] StudentEnrollment studentEnrollment)
        {
            if (id != studentEnrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentEnrollment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Student enrollment updated successfully.";
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentEnrollmentExists(studentEnrollment.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", studentEnrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "UserId", studentEnrollment.StudentProfileId);
            return View(studentEnrollment);
        }

        // GET: StudentEnrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentEnrollment = await _context.StudentEnrollments
                .Include(s => s.Course)
                .Include(s => s.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentEnrollment == null)
            {
                return NotFound();
            }

            return View(studentEnrollment);
        }

        // POST: StudentEnrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentEnrollment = await _context.StudentEnrollments.FindAsync(id);
            if (studentEnrollment != null)
            {
                _context.StudentEnrollments.Remove(studentEnrollment);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student enrollment deleted successfully.";
			return RedirectToAction(nameof(Index));
        }

        private bool StudentEnrollmentExists(int id)
        {
            return _context.StudentEnrollments.Any(e => e.Id == id);
        }
    }
}
