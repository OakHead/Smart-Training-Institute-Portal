using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
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

	}
}