using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Prerequisites
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prerequisites.ToListAsync());
        }

        // GET: Prerequisites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prerequisite = await _context.Prerequisites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prerequisite == null)
            {
                return NotFound();
            }

            return View(prerequisite);
        }

        // GET: Prerequisites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prerequisites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] Prerequisite prerequisite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prerequisite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prerequisite);
        }

        // GET: Prerequisites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prerequisite = await _context.Prerequisites.FindAsync(id);
            if (prerequisite == null)
            {
                return NotFound();
            }
            return View(prerequisite);
        }

        // POST: Prerequisites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,CreatedDate,UpdatedDate,DeleteDate,IsDeleted")] Prerequisite prerequisite)
        {
            if (id != prerequisite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prerequisite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrerequisiteExists(prerequisite.Id))
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
            return View(prerequisite);
        }

        // GET: Prerequisites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prerequisite = await _context.Prerequisites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prerequisite == null)
            {
                return NotFound();
            }

            return View(prerequisite);
        }

        // POST: Prerequisites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prerequisite = await _context.Prerequisites.FindAsync(id);
            if (prerequisite != null)
            {
                _context.Prerequisites.Remove(prerequisite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrerequisiteExists(int id)
        {
            return _context.Prerequisites.Any(e => e.Id == id);
        }
    }
}
