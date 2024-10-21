namespace EventsManagingSystem.Models;

public class ParticipantEvent
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public int EventId { get; set; }

    public virtual Participant? Participant { get; set; }
    public virtual Event? Event { get; set; }
}
