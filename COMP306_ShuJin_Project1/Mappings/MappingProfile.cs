//MappingProfile.cs 
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
            //CreateMap<Booking, BookingDTO>()
            //   .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
            //   .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            //   .ReverseMap();






        }
    }
}
