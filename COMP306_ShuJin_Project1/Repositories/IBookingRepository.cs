using COMP306_ShuJin_Project1.Models;

namespace COMP306_ShuJin_Project1.Repositories
{

    public interface IBookingRepository
    {
        Booking GetById(int id);
        IEnumerable<Booking> GetAll();
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(int id);
    }



}
