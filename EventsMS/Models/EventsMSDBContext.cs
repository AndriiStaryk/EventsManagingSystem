using System.Collections.Generic;
using EventsMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventsManagingSystem.Models;

public class EventsMSDBContext: DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<CreatorEvent> CreatorsEvents { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<ParticipantEvent> ParticipantsEvents { get; set; }

    public EventsMSDBContext()
    {
        //Database.EnsureCreated();
    }

    public EventsMSDBContext(DbContextOptions<EventsMSDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

    //       => optionsBuilder.UseSqlServer("Server= ANDRII-PC\\SQLEXPRESS; Database=EventsMSDB; Trusted_Connection=True; MultipleActiveResultSets=true");





}

