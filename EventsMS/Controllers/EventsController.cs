using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsManagingSystem.Models;
using EventsMS.Models;

namespace EventsMS.Controllers;

public class EventsController : Controller
{
    private readonly EventsMSDBContext _context;
    private EventVM _eventVM;
    private Event _event = new Event();

    public EventsController(EventsMSDBContext context)
    {
        _context = context;
        _eventVM = new EventVM(context, _event);
    }

    // GET: Events
    public async Task<IActionResult> Index()
    {
        var eventsMSDBContext = _context.Events.Include(e => e.Location);
        return View(await eventsMSDBContext.ToListAsync());
    }


    public JsonResult GetLocations()
    {
        var eventLocations = _context.Events
            .Select(e => new
            {
                EventName = e.Name,
                EventDesc = e.Description,
                EventDate = e.EventDate,
                Location = e.Location
            })
            .ToList();
            
        return Json(eventLocations);
    }

    

    // GET: Events/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .Include(e => e.Location)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null)
        {
            return NotFound();
        }

        _eventVM = new EventVM(_context, @event);
        return View(_eventVM);
    }

    // GET: Events/Create
    public IActionResult Create()
    {
        _eventVM = new EventVM(_context, _event);
        ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address");
        ViewData["Creators"] = new SelectList(_context.Users, "Id", "Name");
        ViewData["Participants"] = new SelectList(_context.Users, "Id", "Name");
        
        return View(_eventVM);
    }

    // POST: Events/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,EventDate,Duration,LocationId,Image")] Event @event,
                                            int[] creators, int[] participants, IFormFile? image)
    {

        if (ModelState.IsValid)
        {
            if (!await EventVM.IsEventExist(@event.Name,
                                            @event.Description,
                                            @event.EventDate,
                                            @event.LocationId,
                                            @event.Duration,
                                            image,
                                            _context))
            {
                _eventVM.Event = @event;

                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        @event.Image = memoryStream.ToArray();
                    }
                }
                else
                {
                    string imagePath = "C:\\Users\\Andrii\\source\\repos\\MoviePickerWebApplication_v2\\src\\MoviePickerMVC\\MoviePickerInfrastructure\\wwwroot\\Images\\no_movie_image.jpg";
                    if (System.IO.File.Exists(imagePath))
                    {
                        byte[] defaultImageBytes = System.IO.File.ReadAllBytes(imagePath);
                        @event.Image = defaultImageBytes;
                    }
                }

                _context.Add(@event);

                foreach (var creatorId in creators)
                {
                    Creator c = new Creator();
                    c.UserId = creatorId;
                    _context.Creators.Add(c);
                    await _context.SaveChangesAsync();
                    _eventVM.Event.CreatorEvents.Add(new CreatorEvent { EventId = @event.Id, CreatorId = c.Id });
                }

                

                foreach (var participantId in participants)
                {
                    Participant p = new Participant();
                    p.UserId = participantId;
                    _context.Participants.Add(p);
                    await _context.SaveChangesAsync();
                    _eventVM.Event.ParticipantEvents.Add(new ParticipantEvent { EventId = @event.Id, ParticipantId = p.Id });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "This event already exists.");
            }
        }

        ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address");
        ViewData["Creators"] = new SelectList(_context.Users, "Id", "Name");
        ViewData["Participants"] = new SelectList(_context.Users, "Id", "Name");

        return View(_eventVM);
    }

    // GET: Events/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
        {
            return NotFound();
        }


        _eventVM = new EventVM(_context, @event);
        _event = @event;
        ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", @event.LocationId);
        ViewData["Creators"] = new SelectList(_context.Users, "Id", "Name");
        ViewData["Participants"] = new SelectList(_context.Users, "Id", "Name");
        return View(_eventVM);
     
    }

    // POST: Events/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Description,EventDate,Duration,LocationId,Image,CreatedAt,Id")] Event @event,
                                            int[] creators, int[] participants, IFormFile? image)
    {
        if (id != @event.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (!await EventVM.IsEventExist(@event.Name,
                                            @event.Description,
                                            @event.EventDate,
                                            @event.LocationId,
                                            @event.Duration,
                                            image,
                                            _context))
            {
                try
                {
                    //EventVM.DeleteEventRelations(@event, _context);
                    _eventVM.Event = @event;

                    foreach (var creatorId in creators)
                    {

                        var existingCreatorsIDs = _context.CreatorsEvents
                            .Where(ce => ce.EventId == @event.Id) // Filter for the specific event
                            .Select(ce => ce.CreatorId) // Select the CreatorId
                            .ToList(); // Convert the result to a list


                        if (existingCreatorsIDs.Count() > 0)
                        {

                            foreach (var crId in existingCreatorsIDs)
                            {
                                var cr = _context.Creators.Find(crId);

                                if (cr != null)
                                {
                                    cr.UserId = creatorId;
                                    _context.Update(cr);
                                    await _context.SaveChangesAsync();

                                }
                                else
                                {
                                    Creator c = new Creator();
                                    c.UserId = creatorId;
                                    _context.Creators.Add(c);
                                    await _context.SaveChangesAsync();
                                    _eventVM.Event.CreatorEvents.Add(new CreatorEvent { EventId = @event.Id, CreatorId = c.Id });
                                }
                            }
                        }
                        else
                        {
                            Creator c = new Creator();
                            c.UserId = creatorId;
                            _context.Creators.Add(c);
                            await _context.SaveChangesAsync();
                            _eventVM.Event.CreatorEvents.Add(new CreatorEvent { EventId = @event.Id, CreatorId = c.Id });
                        }
                    }


                    foreach (var participantId in participants)
                    {

                        var existingParticipantsIDs = _context.ParticipantsEvents
                            .Where(pe => pe.EventId == @event.Id) 
                            .Select(pe => pe.ParticipantId) 
                            .ToList(); 

                        if (existingParticipantsIDs.Count() > 0)
                        {

                            foreach (var prId in existingParticipantsIDs)
                            {
                                var pr = _context.Participants.Find(prId);

                                if (pr != null)
                                {
                                    pr.UserId = participantId;
                                    _context.Update(pr);
                                    await _context.SaveChangesAsync();

                                }
                                else
                                {
                                    Participant p = new Participant();
                                    p.UserId = participantId;
                                    _context.Participants.Add(p);
                                    await _context.SaveChangesAsync();
                                    _eventVM.Event.ParticipantEvents.Add(new ParticipantEvent { EventId = @event.Id, ParticipantId = p.Id });
                                }
                            }
                        }
                        else
                        {
                            Participant p = new Participant();
                            p.UserId = participantId;
                            _context.Participants.Add(p);
                            await _context.SaveChangesAsync();
                            _eventVM.Event.ParticipantEvents.Add(new ParticipantEvent { EventId = @event.Id, ParticipantId = p.Id });
                        }
                    }


                    var existingEvent = await _context.Events.FindAsync(id);

                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    _context.Entry(existingEvent).State = EntityState.Detached;

                    if (image != null && image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            @event.Image = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        @event.Image = existingEvent.Image;
                    }


                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
                ModelState.AddModelError(string.Empty, "This event already exists.");
            }
        }

        ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", @event.LocationId);
        ViewData["Creators"] = new SelectList(_context.Users, "Id", "Name");
        ViewData["Participants"] = new SelectList(_context.Users, "Id", "Name");
        return View(_eventVM);
    }

    // GET: Events/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .Include(e => e.Location)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // POST: Events/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            _context.Events.Remove(@event);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}
