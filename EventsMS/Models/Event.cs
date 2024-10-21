using EventsMS.Models;

namespace EventsManagingSystem.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime EventDate { get; set; }

    public double Duration { get; set; }
    public int LocationId { get; set; }
    //public int CreatorId { get; set; }
    public byte[]? Image { get; set; }
    
    public virtual Location? Location { get; set; }
    public virtual ICollection<CreatorEvent> CreatorEvents { get; set; } = new List<CreatorEvent>();
    public virtual ICollection<ParticipantEvent> ParticipantEvents { get; set; } = new List<ParticipantEvent>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
