namespace EventsManagingSystem.Models;

public class CreatorEvent
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public int EventId { get; set; }

    public virtual Creator? Creator { get; set; }
    public virtual Event? Event { get; set; }
}
