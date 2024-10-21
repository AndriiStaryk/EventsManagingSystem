using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EventsManagingSystem.Models;


public class Location
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The field must not be empty")]
    [StringLength(50, ErrorMessage = "The name cannot be longer than 50 characters.")]
    public string Name { get; set; } = null!;


    [Required(ErrorMessage = "The field must not be empty")]
    [StringLength(100, ErrorMessage = "The name cannot be longer than 100 characters.")]

    public string Address { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
