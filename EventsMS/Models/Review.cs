namespace EventsMS.Models;

public class Review
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Rating { get; set; }
}
