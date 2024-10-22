using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventsManagingSystem.Models;

namespace EventsMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsAPIController : ControllerBase
    {
        private readonly EventsMSDBContext _context;

        public LocationsAPIController(EventsMSDBContext context)
        {
            _context = context;
        }

        // GET: api/LocationsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            return await _context.Locations.ToListAsync();
        }

        // GET: api/LocationsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        // PUT: api/LocationsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(int id, Location location)
        {
            //if (id != location.Id)
            //{
            //    return BadRequest();
            //}

            if (await LocationsController.IsLocationExist(location.Name,
                                                               location.Address,
                                                               location.Latitude,
                                                               location.Longitude,
                                                               _context))
            {
                return Conflict("User already exists.");
            }

            var existingLocation = await _context.Locations.FindAsync(id);

            if (existingLocation == null)
            {
                return NotFound();
            }

            existingLocation.Name = location.Name;
            existingLocation.Address = location.Address;
            existingLocation.Latitude = location.Latitude;
            existingLocation.Longitude = location.Longitude;

            // _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExistsById(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LocationsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            if (await LocationsController.IsLocationExist(location.Name,
                                                               location.Address,
                                                               location.Latitude,
                                                               location.Longitude,
                                                               _context))
            {
                return Conflict("User already exists.");
            }

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.Id }, location);
        }

        // DELETE: api/LocationsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExistsById(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
