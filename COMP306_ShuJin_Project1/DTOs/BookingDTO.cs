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
        public string? GuestName { get; set; }
        //guestPhone
        public string? GuestPhone { get; set; }
        //price
        public decimal TotalPrice { get; set; }
        //status
        public string? Status { get; set; }

        //remarks
        public string? Remark { get; set; }

        //how room info
        public   RoomDTO? Room { get; set; }

        //how user info
        public   UserDTO? User { get; set; }
         
    }
}
