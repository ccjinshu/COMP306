
using System;

namespace CcbnbApi.Models
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
    }
}
