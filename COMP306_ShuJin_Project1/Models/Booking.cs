
using System;

namespace COMP306_ShuJin_Project1.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Room Room { get; set; }


        //guestName
        public string GuestName { get; set; }
        //guestPhone
        public string GuestPhone { get; set; }
        //price 
        //status
        public string Status { get; set; }

        //remarks
        public string Remark { get; set; }
 

    }
}
