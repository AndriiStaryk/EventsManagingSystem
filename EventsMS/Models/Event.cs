using EventsMS.Models;
using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public byte[]? Image { get; set; }

    [JsonIgnore]
    public virtual Location? Location { get; set; }

    [JsonIgnore]
    public virtual ICollection<CreatorEvent> CreatorEvents { get; set; } = new List<CreatorEvent>();

    [JsonIgnore]
    public virtual ICollection<ParticipantEvent> ParticipantEvents { get; set; } = new List<ParticipantEvent>();


    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
