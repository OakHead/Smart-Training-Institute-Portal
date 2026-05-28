using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
	[Authorize(Roles = "Admin")]
	public class DepartmentsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DepartmentsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var departments = await _context.Departments
				.Include(d => d.Courses)
				.ToListAsync();

			return View(departments);
		}

		public async Task<IActionResult> Details(int id)
		{
			var department = await _context.Departments
				.Include(d => d.Courses)
				.FirstOrDefaultAsync(d => d.Id == id);

			if (department == null)
				return NotFound();

			return View(department);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Department department)
		{
			if (!ModelState.IsValid)
				return View(department);

			_context.Departments.Add(department);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int id)
		{
			var department = await _context.Departments.FindAsync(id);

			if (department == null)
				return NotFound();

			return View(department);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Department department)
		{
			if (id != department.Id)
				return BadRequest();

			if (!ModelState.IsValid)
				return View(department);

			_context.Departments.Update(department);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int id)
		{
			var department = await _context.Departments
				.FirstOrDefaultAsync(d => d.Id == id);

			if (department == null)
				return NotFound();

			return View(department);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var department = await _context.Departments.FindAsync(id);

			if (department == null)
				return NotFound();

			_context.Departments.Remove(department);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}