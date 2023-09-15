using EventRegistrationApp.Data;
using EventRegistrationApp.Models;
using Microsoft.EntityFrameworkCore;
using EventRegistrationApp.Services;

public enum RegistrationResult
{
    Success,
    EventNotFound,
    NoAvailableSeats,
    AlreadyRegistered,
    Error,
    InvalidUserIdFormat
}

public class RegistrationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Random _random;
    public string ReferenceNumber { get; private set; }
    public RegistrationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _random = new Random();
    }

    public async Task<RegistrationResult> RegisterForEventAsync(int eventId, string userId)
    {
        // Check if the event exists
        var @event = await _dbContext.Events.FindAsync(eventId);
        if (@event == null)
        {
            return RegistrationResult.EventNotFound;
        }

        // Check if the event has available seats
        if (@event.TotalSeats <= 0)
        {
            return RegistrationResult.NoAvailableSeats;
        }

        // Check if the user is already registered for the event
        var existingRegistration = await _dbContext.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (existingRegistration != null)
        {
            return RegistrationResult.AlreadyRegistered;
        }

        // Validate the user ID format
        if (!IsValidUserIdFormat(userId))
        {
            return RegistrationResult.InvalidUserIdFormat; // Return a custom result for invalid user ID format
        }

        // Generate a unique reference number (you can customize this logic)
        var referenceNumber = GenerateUniqueReferenceNumber();

        // Create a new registration entry
        var newRegistration = new Registration
        {
            EventId = eventId,
            UserId = userId,
            ReferenceNumber = referenceNumber
        };

        // Decrement the available seats for the event
        @event.TotalSeats--;

        // Save changes to the database
        _dbContext.Registrations.Add(newRegistration);
        await _dbContext.SaveChangesAsync();

        return RegistrationResult.Success;
    }


    // Custom method to validate user ID format
    private bool IsValidUserIdFormat(string userId)
    {
        // User ID must be at least 6 characters
        if (string.IsNullOrEmpty(userId) || userId.Length < 6)
        {
            return false;
        }

        // User ID must contain at least one letter and one digit
        bool hasLetter = userId.Any(char.IsLetter);
        bool hasDigit = userId.Any(char.IsDigit);

        return hasLetter && hasDigit;
    }



    private string GenerateUniqueReferenceNumber()
    {
        // Generate a unique reference number logic (you can customize this)
        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }
    public async Task<bool> IsUserRegisteredAsync(int eventId, string userId)
    {
        return await _dbContext.Registrations
            .AnyAsync(r => r.EventId == eventId && r.UserId == userId);
    }

    // Get a list of registrations for a specific event
    public async Task<List<Registration>> GetEventRegistrationsAsync(int eventId)
    {
        return await _dbContext.Registrations
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }
    public async Task<Event> GetEventByIdAsync(int eventId)
    {
        // Implement fetching an event by ID here
        return await _dbContext.Events.FindAsync(eventId);
    }
    // Remove a user's registration for a specific event
    public async Task<bool> UnregisterFromEventAsync(int eventId, string userId)
    {
        var registration = await _dbContext.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (registration != null)
        {
            _dbContext.Registrations.Remove(registration);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
