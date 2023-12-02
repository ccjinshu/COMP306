using CcbnbApi.Models;

namespace CcbnbApi.Repositories
{

    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsForRoomAsync(int roomId);
        Task<IEnumerable<Booking>> GetBookingsForUserAsync(int userId);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int bookingId);
        Task<bool> SaveAsync();

        //Patch
        Task PatchBookingAsync(Booking booking);

    }



}
