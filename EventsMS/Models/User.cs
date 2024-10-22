using System.Text.Json.Serialization;

namespace EventsManagingSystem.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public byte[]? Avatar { get; set; }

    [JsonIgnore]
    public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();
    
    [JsonIgnore]
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
