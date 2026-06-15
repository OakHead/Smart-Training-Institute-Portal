using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
	[Authorize(Roles = "Admin")]
	public class PrerequisitesController : Controller
	{
		private readonly ApplicationDbContext _context;

		public PrerequisitesController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _context.Prerequisites.ToListAsync());
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name")] Prerequisite prerequisite)
		{
			if (ModelState.IsValid)
			{
				_context.Prerequisites.Add(prerequisite);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			return View(prerequisite);
		}
		
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var prerequisite = await _context.Prerequisites
				.FirstOrDefaultAsync(p => p.Id == id);

			if (prerequisite == null) return NotFound();

			return View(prerequisite);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var prerequisite = await _context.Prerequisites.FindAsync(id);

			if (prerequisite == null) return NotFound();

			return View(prerequisite);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Prerequisite prerequisite)
		{
			if (id != prerequisite.Id) return NotFound();

			if (ModelState.IsValid)
			{
				var existingPrerequisite = await _context.Prerequisites.FindAsync(id);

				if (existingPrerequisite == null) return NotFound();

				existingPrerequisite.Name = prerequisite.Name;

				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			return View(prerequisite);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var prerequisite = await _context.Prerequisites
				.FirstOrDefaultAsync(p => p.Id == id);

			if (prerequisite == null) return NotFound();

			return View(prerequisite);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var prerequisite = await _context.Prerequisites
				.Include(p => p.Courses)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (prerequisite == null) return NotFound();

			if (prerequisite.Courses.Any())
			{
				TempData["Error"] = "Cannot delete this prerequisite because it is assigned to one or more courses.";
				return RedirectToAction(nameof(Index));
			}

			_context.Prerequisites.Remove(prerequisite);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Prerequisite deleted successfully.";
			return RedirectToAction(nameof(Index));
		}

	}
}