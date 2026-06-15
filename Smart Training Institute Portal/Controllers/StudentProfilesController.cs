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

	public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

		public StudentProfilesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

        // GET: StudentProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentProfiles.Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // GET: StudentProfiles/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: StudentProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,ImageUrl,DateOfBirth,GPA,UserId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] StudentProfile studentProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentProfile);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student profile created successfully.";
				return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.UserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.UserId);
            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,ImageUrl,DateOfBirth,GPA,UserId,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentProfile);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Student profile updated successfully.";
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentProfileExists(studentProfile.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.UserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // POST: StudentProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student profile deleted successfully.";
			return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> MyCourses()
		{
			var user = await _userManager.GetUserAsync(User);

			var student = await _context.StudentProfiles
				.FirstOrDefaultAsync(s => s.UserId == user.Id);

			if (student == null)
			{
				return NotFound();
			}

			var enrollments = await _context.StudentEnrollments
				.Include(e => e.Course)
					.ThenInclude(c => c.Department)
				.Include(e => e.Course)
					.ThenInclude(c => c.Prerequisites)
				.Where(e => e.StudentProfileId == student.Id)
				.ToListAsync();

			return View(enrollments);
		}
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> PerformanceSummary()
		{
			var user = await _userManager.GetUserAsync(User);

			var student = await _context.StudentProfiles
				.Include(s => s.User)
				.Include(s => s.Enrollments)
					.ThenInclude(e => e.Course)
						.ThenInclude(c => c.Department)
				.FirstOrDefaultAsync(s => s.UserId == user.Id);

			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}
		
	}
}
