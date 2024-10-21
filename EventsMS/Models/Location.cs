﻿using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

   
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    //public DateTime CreatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}