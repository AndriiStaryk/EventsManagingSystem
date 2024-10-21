namespace EventsManagingSystem.Models;

public class Creator
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<CreatorEvent> CreatorEvents { get; set; } = new List<CreatorEvent>();
}
