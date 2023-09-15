// Data/ApplicationDbContext.cs
using EventRegistrationApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EventRegistrationApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for the Event table
            List<Event> eventsToSeed = new List<Event>();

            for (int i = 1; i <= 5; i++)
            {
                eventsToSeed.Add(new Event
                {
                    Id = i,
                    Name = $"Event {i}",
                    Date = DateTime.Now.AddDays(i * 7), // Adjust the date as needed
                    TotalSeats = 50 + (i * 10) // Adjust the number of seats as needed
                });
            }

            modelBuilder.Entity<Event>().HasData(eventsToSeed);

            // Seed data for the Registration table
            List<Registration> registrationsToSeed = new List<Registration>();

            for (int i = 1; i <= 10; i++)
            {
                registrationsToSeed.Add(new Registration
                {
                    Id = i,
                    EventId = i % 5 + 1, // Link registrations to events (1 to 5)
                    UserId = $"User{i}",
                    ReferenceNumber = $"Ref{i}"
                });
            }

            modelBuilder.Entity<Registration>().HasData(registrationsToSeed);
        }

    }
}
