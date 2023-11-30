 
using AutoMapper;
using COMP306_ShuJin_Project1.Models;
using COMP306_ShuJin_Project1.DTOs;

namespace COMP306_ShuJin_Project1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to DTO
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();

            // Add more mappings here
        }
    }
}
