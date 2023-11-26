using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{
    public interface IRoomRepository
    {
        Room GetById(int id);
        IEnumerable<Room> GetAll();
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
    }



}
