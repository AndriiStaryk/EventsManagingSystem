using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsManagingSystem.Models;

namespace EventsMS.Controllers
{
    public class CreatorsController : Controller
    {
        private readonly EventsMSDBContext _context;

        public CreatorsController(EventsMSDBContext context)
        {
            _context = context;
        }

        // GET: Creators
        public async Task<IActionResult> Index()
        {
            var eventsMSDBContext = _context.Creators.Include(c => c.User);
            return View(await eventsMSDBContext.ToListAsync());
        }

        // GET: Creators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creator = await _context.Creators
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creator == null)
            {
                return NotFound();
            }

            return View(creator);
        }

        // GET: Creators/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Creators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Creator creator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(creator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", creator.UserId);
            return View(creator);
        }

        // GET: Creators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creator = await _context.Creators.FindAsync(id);
            if (creator == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", creator.UserId);
            return View(creator);
        }

        // POST: Creators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId")] Creator creator)
        {
            if (id != creator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreatorExists(creator.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", creator.UserId);
            return View(creator);
        }

        // GET: Creators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creator = await _context.Creators
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creator == null)
            {
                return NotFound();
            }

            return View(creator);
        }

        // POST: Creators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creator = await _context.Creators.FindAsync(id);
            if (creator != null)
            {
                _context.Creators.Remove(creator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreatorExists(int id)
        {
            return _context.Creators.Any(e => e.Id == id);
        }
    }
}
