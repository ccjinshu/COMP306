using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{
    // BookingRepository manages data access for Booking entities.
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        // Accepting the database context via dependency injection.
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets all bookings from the database.
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        // Gets a specific booking based on the booking ID.
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        // Retrieves all bookings associated with a specific room.
        public async Task<IEnumerable<Booking>> GetBookingsForRoomAsync(int roomId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
        }

        // Retrieves all bookings made by a specific user.
        public async Task<IEnumerable<Booking>> GetBookingsForUserAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        // Adds a new booking for a room.
        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        // Updates an existing booking.
        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        // Deletes a specific booking from the database.
        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        // Commits changes to the database and returns true if successful.
        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
