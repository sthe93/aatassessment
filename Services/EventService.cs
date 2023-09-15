using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using EventRegistrationApp.Data;
using EventRegistrationApp.Models;
using Microsoft.EntityFrameworkCore;
using EventRegistrationApp.Services;

public enum EventCreationResult
{
    Success,
    EventDateInPast,
    EmptyEventName,
    InvalidTotalSeats,
    EventAlreadyExists,
    NoAvailableSeats,
    Error,
    EventNotFound
}


namespace EventRegistrationApp.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _dbContext;

        public EventService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<EventCreationResult> CreateEventAsync(Event newEvent)
        {
            // Check if the event date is in the past
            if (newEvent.Date < DateTime.Today)
            {
                return EventCreationResult.EventDateInPast;
            }

            // Check if the event name is empty
            if (string.IsNullOrWhiteSpace(newEvent.Name))
            {
                return EventCreationResult.EmptyEventName;
            }

            // Check if the total seats is greater than 0
            if (newEvent.TotalSeats <= 0)
            {
                return EventCreationResult.InvalidTotalSeats;
            }

            // Check if the event already exists
            if (await _dbContext.Events.AnyAsync(e => e.Name == newEvent.Name))
            {
                return EventCreationResult.EventAlreadyExists;
            }

            // Check if there are open seats for the event
            var openSeats = newEvent.TotalSeats - await _dbContext.Registrations.CountAsync(r => r.EventId == newEvent.Id);
            if (openSeats <= 0)
            {
                return EventCreationResult.NoAvailableSeats;
            }

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            return EventCreationResult.Success;
        }


        public async Task<EventCreationResult> UpdateEventAsync(Event updatedEvent)
        {
            // Check if the updatedEvent is null (not found)
            if (updatedEvent == null)
            {
                // You can return an appropriate result indicating that the event was not found
                return EventCreationResult.EventNotFound;
            }

            // Check if the event date is in the past
            if (updatedEvent.Date < DateTime.Today)
            {
                return EventCreationResult.EventDateInPast;
            }

            // Validate the event properties here, similar to how you did in the CreateEventAsync method

            // Check if the event name is empty
            if (string.IsNullOrEmpty(updatedEvent.Name))
            {
                return EventCreationResult.EmptyEventName;
            }

            // Check if total seats is invalid (e.g., less than or equal to 0)
            if (updatedEvent.TotalSeats <= 0)
            {
                return EventCreationResult.InvalidTotalSeats;
            }

            // Check if an event with the same name already exists
            if (await _dbContext.Events.AnyAsync(e => e.Id != updatedEvent.Id && e.Name == updatedEvent.Name))
            {
                return EventCreationResult.EventAlreadyExists;
            }

            // Update the event and save changes
            _dbContext.Entry(updatedEvent).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return EventCreationResult.Success; // Event update was successful
            }
            catch
            {
                // Handle any exceptions or errors that occur during the update
                return EventCreationResult.Error;
            }
        }


        public async Task DeleteEventAsync(int eventId)
        {
            var eventToDelete = await _dbContext.Events.FindAsync(eventId);
            if (eventToDelete != null)
            {
                _dbContext.Events.Remove(eventToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
