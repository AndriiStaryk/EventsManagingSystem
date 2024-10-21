namespace EventsManagingSystem.Models;

public class Participant
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<ParticipantEvent> ParticipantEvents { get; set; } = new List<ParticipantEvent>();
}
