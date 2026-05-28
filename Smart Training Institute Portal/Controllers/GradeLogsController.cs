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
    [Authorize(Roles = "Admin , Instructor")]
	public class GradeLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradeLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var gradeLogs = _context.GradeLogs
                .Include(g => g.StudentEnrollment)
                .ThenInclude(e => e.StudentProfile)
				.Include(g => g.StudentEnrollment)
                .ThenInclude(e => e.Course)
                .Include(g => g.UpdatedBy)
				.OrderByDescending(g => g.CreatedDate)
                .ToListAsync();
            return View(await gradeLogs);
		}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gradeLog = await _context.GradeLogs
				.Include(g => g.StudentEnrollment)
				.ThenInclude(e => e.StudentProfile)
				.Include(g => g.StudentEnrollment)
				.ThenInclude(e => e.Course)
				.Include(g => g.UpdatedBy)
				.FirstOrDefaultAsync(m => m.Id == id);
            if (gradeLog == null)
            {
                return NotFound();
            }

            return View(gradeLog);
        }
    }
}
