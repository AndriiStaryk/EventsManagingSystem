using EventsManagingSystem.Models;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EventsMS.Models;

public class EventVM

{
    private EventsMSDBContext _context = new EventsMSDBContext();
    public Event Event { get; set; } = null!;
    public List<User> Creators { get; set; } = new List<User>();

    public List<User> Participants { get; set; } = new List<User>();

    public Location Location { get; set; } = null!;

    public List<Location> AllLocations { get; set; }

    public EventVM(EventsMSDBContext context, Event @event)
    {
        _context = context;
        Event = @event;

        Location = context.Locations
            .Find(@event.LocationId)!;


        //var userCIds = context.CreatorsEvents
        //        .Where(ce => ce.EventId == @event.Id)
        //        .Select(ce => ce.CreatorId)
        //        .ToList();


        //Creators = context.Users
        //    .Where(u => userIds.Contains(u.Id))
        //    .ToList();


        //var userPIds = context.ParticipantsEvents
        //       .Where(pe => pe.EventId == @event.Id)
        //       .Select(pe => pe.Creator.UserId)
        //       .ToList();


        //Creators = context.Users
        //    .Where(u => userIds.Contains(u.Id))
        //    .ToList();




        Creators = (from ce in context.CreatorsEvents
                    join c in context.Creators on ce.CreatorId equals c.Id
                    join u in context.Users on c.UserId equals u.Id
                    where ce.EventId == @event.Id
                    select u).ToList();

        Participants = (from pe in context.ParticipantsEvents
                        join p in context.Participants on pe.ParticipantId equals p.Id
                        join u in context.Users on p.UserId equals u.Id
                        where pe.EventId == @event.Id
                        select u).ToList();





        //Participants = context.ParticipantsEvents
        //   .Where(pe => pe.EventId == @event.Id)
        //   .Select(pe => pe.Participant).ToList()!;




        AllLocations = context.Locations.ToList()!;
    }


    static public void DeleteEventRelations(Event @event, EventsMSDBContext context)
    {
        var ces = context.CreatorsEvents
            .Where(ce => ce.EventId == @event.Id).ToList();

        foreach (var ce in ces)
        {
            if (ce != null)
            {
                context.CreatorsEvents.Remove(ce);
            }
        }


        var pes = context.ParticipantsEvents
            .Where(pe => pe.EventId == @event.Id).ToList();

        foreach (var pe in pes)
        {
            if (pe != null)
            {
                context.ParticipantsEvents.Remove(pe);
            }
        }

        context.SaveChanges();
    }

    public static void DeleteEvent(Event @event, EventsMSDBContext context)
    {
        EventVM.DeleteEventRelations(@event, context);
        context.Events.Remove(@event);
        context.SaveChanges();
    }

   
    static public async Task<bool> IsEventExist(
                                   string? name,
                                   string? description,
                                   DateTime eventDate,
                                   int locationID,
                                   double duration,
                                   IFormFile? eventImage,
                                   EventsMSDBContext context)
    {

        byte[]? image = [];
        if (eventImage != null && eventImage.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await eventImage.CopyToAsync(memoryStream);
                image = memoryStream.ToArray();
            }
        }


        var @event = await context.Events
            .FirstOrDefaultAsync(
                m => m.Name == name &&
                m.Description == description &&
                m.EventDate == eventDate &&
                m.LocationId == locationID &&
                m.Duration == duration);

        if (@event != null && image != null && @event.Image!.SequenceEqual(image))
        {
            return true;
        }

        return false;
    }


}
