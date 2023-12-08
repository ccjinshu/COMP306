namespace CCBnb_Admin_Web.Models
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }

        //details infomation
        public string  Desc  { get; set; }

        //status
        public string Status { get; set; }


        //imgurl
        public string ImgUrl { get; set; }
    }
}
