using CcbnbApi.Models;

namespace CcbnbApi.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<bool> SaveAsync();


        //Patch
        Task PatchUserAsync(User user);


        //Login
        Task<User> LoginAsync(string email, string password); 


        //Register
        Task<User> RegisterAsync(string name, string email, string phoneNumber, string password);

    }
}
