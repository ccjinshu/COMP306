using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP306_ShuJin_Project1.Data;
using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{
    // UserRepository handles the data access logic for User entities.
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor that accepts the database context.
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all users from the database.
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Retrieves a single user by their ID.
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Adds a new user to the database.
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Updates an existing user's data in the database.
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Deletes a user from the database.
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Saves changes to the database and returns a boolean indicating if the operation was successful.
        public async Task<bool> SaveAsync()
        {
            // Returns true if one or more entities were changed.
            return (await _context.SaveChangesAsync()) > 0;
        }

        //PatchUser
        public async Task PatchUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
