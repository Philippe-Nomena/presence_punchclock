using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presence.Data;
using Presence.Models;

namespace Presence.Controllers
{
    public class PostalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Postals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Postals.ToListAsync());
        }

        // GET: Postals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postal = await _context.Postals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postal == null)
            {
                return NotFound();
            }

            return View(postal);
        }

        // GET: Postals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code_Postal,Pays")] Postal postal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postal);
        }

        // GET: Postals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postal = await _context.Postals.FindAsync(id);
            if (postal == null)
            {
                return NotFound();
            }
            return View(postal);
        }

        // POST: Postals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code_Postal,Pays")] Postal postal)
        {
            if (id != postal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostalExists(postal.Id))
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
            return View(postal);
        }

        // GET: Postals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postal = await _context.Postals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postal == null)
            {
                return NotFound();
            }

            return View(postal);
        }

        // POST: Postals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postal = await _context.Postals.FindAsync(id);
            if (postal != null)
            {
                _context.Postals.Remove(postal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostalExists(int id)
        {
            return _context.Postals.Any(e => e.Id == id);
        }
    }
}
