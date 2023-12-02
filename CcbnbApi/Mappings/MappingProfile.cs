 
using AutoMapper;
using CcbnbApi.Models;
using CcbnbApi.DTOs;

namespace CcbnbApi.Mappings
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
