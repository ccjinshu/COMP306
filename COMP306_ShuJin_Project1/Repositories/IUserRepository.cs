using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}
