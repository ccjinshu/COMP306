using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Models; 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 


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

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
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

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public Room GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Room> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(Room room)
        {
            throw new NotImplementedException();
        }

        public void Update(Room room)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }

}