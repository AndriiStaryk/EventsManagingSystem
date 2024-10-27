using EventsManagingSystem.Models;
using EventsMS.Controllers;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.IO;

namespace EventsMS.Models;

public class EventVM

{
    private EventsMSDBContext _context = new EventsMSDBContext();
    public Event Event { get; set; } = null!;
    public List<User> Creators { get; set; } = new List<User>();

    public List<User> Participants { get; set; } = new List<User>();

    public List<Review> Reviews { get; set; } = new List<Review>();

    public Location Location { get; set; } = null!;


    public List<Event> EventsBySameCreators { get; set; } = new List<Event> { };

    public List<Event> EventsNearby { get; set; } = new List<Event> { };


    //public List<Location> AllLocations { get; set; }

    public EventVM(EventsMSDBContext context, Event @event)
    {
        _context = context;
        Event = @event;

        Location = context.Locations
            .Find(@event.LocationId)!;

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

        Reviews = context.Reviews.Where(r => r.EventId == @event.Id).ToList();

        var sameCreatorIds = (from c in _context.Creators
                             where (from ce in _context.CreatorsEvents
                                    join creator in _context.Creators on ce.CreatorId equals creator.Id
                                    where ce.EventId == @event.Id
                                    select creator.UserId).Contains(c.UserId)
                             select c.Id).ToList();


        EventsBySameCreators = (from e in _context.Events
                                join ce in _context.CreatorsEvents on e.Id equals ce.EventId
                                where sameCreatorIds.Contains(ce.CreatorId) && e.Id != @event.Id
                                select e).Distinct().ToList();

    }


    static public void DeleteEventRelations(Event @event, EventsMSDBContext context)
    {
        var ces = context.CreatorsEvents
            .Where(ce => ce.EventId == @event.Id).ToList();

        var cIds = ces.Select(ce => ce.CreatorId).ToList();


        foreach (var ce in ces)
        {
            if (ce != null)
            {
                context.CreatorsEvents.Remove(ce);
            }
        }

        foreach(var cId in cIds)
        {
            var c = context.Creators.Find(cId);
            if (c != null)
            {
                context.Creators.Remove(c);
            }
        }


        var pes = context.ParticipantsEvents
            .Where(pe => pe.EventId == @event.Id).ToList();

        var pIds = pes.Select(pe => pe.ParticipantId).ToList();

        foreach (var pe in pes)
        {
            if (pe != null)
            {
                context.ParticipantsEvents.Remove(pe);
            }
        }

        foreach (var pId in pIds)
        {
            var p = context.Participants.Find(pId);
            if (p != null)
            {
                context.Participants.Remove(p);
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
                                   EventsMSDBContext context,
                                   int[] creators = null,
                                   int[] participants = null)
    {
        var @event = await context.Events
           .FirstOrDefaultAsync(
               m => m.Name == name &&
               m.Description == description &&
               m.EventDate == eventDate &&
               m.LocationId == locationID &&
               m.Duration == duration);

        if (@event == null)
        {
            return false;
        }

        var cIds = (from ce in context.CreatorsEvents
                    join c in context.Creators on ce.CreatorId equals c.Id
                    join u in context.Users on c.UserId equals u.Id
                    where ce.EventId == @event.Id
                    select u.Id).ToList();

        var pIds = (from pe in context.ParticipantsEvents
                    join p in context.Participants on pe.ParticipantId equals p.Id
                    join u in context.Users on p.UserId equals u.Id
                    where pe.EventId == @event.Id
                    select u.Id).ToList();


        bool isImageProvided = true;

        byte[]? image = [];
        if (eventImage != null && eventImage.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await eventImage.CopyToAsync(memoryStream);
                image = memoryStream.ToArray();
            }

            isImageProvided = @event.Image!.SequenceEqual(image);
        }

        return @event != null &&
               creators != null &&
               participants != null &&
               isImageProvided &&
               cIds.OrderBy(x => x).SequenceEqual(creators.OrderBy(x => x)) &&
               pIds.OrderBy(x => x).SequenceEqual(participants.OrderBy(x => x));

        //TODO: maybe add that creators & participants should not have same users to the check
    }
}
