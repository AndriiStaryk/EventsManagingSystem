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
    public class LocationsController : Controller
    {
        private readonly EventsMSDBContext _context;

        public LocationsController(EventsMSDBContext context)
        {
            _context = context;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Locations.ToListAsync());
        }


        public JsonResult GetLocations()
        {
            var locations = _context.Locations
                .Select(loc => new
                {
                    lat = loc.Latitude,
                    lng = loc.Longitude,
                    name = loc.Name
                }).ToList();


            return Json(locations);
        }


        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Latitude,Longitude,CreatedAt")] Location location)
        {
            if (ModelState.IsValid)
            {

                if (!await LocationsController.IsLocationExist(location.Name,
                                                               location.Address,
                                                               location.Latitude,
                                                               location.Longitude,
                                                               _context))
                    {
                    _context.Add(location);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                } 
                else
                {
                    ModelState.AddModelError(string.Empty, "This location already exists.");
                }
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Latitude,Longitude,CreatedAt")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await LocationsController.IsLocationExist(location.Name,
                                                               location.Address,
                                                               location.Latitude,
                                                               location.Longitude,
                                                               _context))
                {
                    try
                    {
                        _context.Update(location);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LocationExists(location.Id))
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
                else
                {
                    ModelState.AddModelError(string.Empty, "This location already exists.");
                }
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }


        public static async Task<bool> IsLocationExist(string name,
                                                       string address,
                                                       double latitude,
                                                       double longitude,
                                                       EventsMSDBContext context)
        {
            var location = await context.Locations
                .FirstOrDefaultAsync(l => 
                                       l.Name == name &&
                                       l.Address == address &&
                                       l.Latitude == latitude &&
                                       l.Longitude == longitude);

            return location != null;
        }
    }
}
