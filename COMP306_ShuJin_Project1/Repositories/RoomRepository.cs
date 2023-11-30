using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(int roomId)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task AddRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        //PatchRoom

        public async Task PatchRoomAsync(Room room)
        {
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        //GetAvailableRoomsByStartDateAndEndDate
        public async Task<IEnumerable<Room>> GetAvailableRoomsByStartDateAndEndDate(DateTime startDate, DateTime endDate)
        {
            var rooms = await _context.Rooms.ToListAsync();
            var bookings = await _context.Bookings.ToListAsync();

            var availableRooms = new List<Room>();

            foreach (var room in rooms)
            {
                var isAvailable = true;
                foreach (var booking in bookings)
                {
                    if (booking.RoomId == room.Id)
                    {
                        if (startDate >= booking.StartDate && startDate <= booking.EndDate)
                        {
                            isAvailable = false;
                        }
                        else if (endDate >= booking.StartDate && endDate <= booking.EndDate)
                        {
                            isAvailable = false;
                        }
                        else if (startDate <= booking.StartDate && endDate >= booking.EndDate)
                        {
                            isAvailable = false;
                        }
                    }
                }
                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }
            return availableRooms;
        }


    }
}
