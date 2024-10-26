using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventsManagingSystem.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [RegularExpression(@"^\+?(\d{1,3})?[-.\s]?(\(?\d{1,4}\)?)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$",
       ErrorMessage = "Please enter a valid mobile number.")]
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
