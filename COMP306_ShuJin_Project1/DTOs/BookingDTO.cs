namespace COMP306_ShuJin_Project1.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
         
        
        //guestName
        public string GuestName { get; set; }
        //guestPhone
        public string GuestPhone { get; set; }
        //price
        public decimal TotalPrice { get; set; }
        //status
        public string Status { get; set; }

         
    }
}
