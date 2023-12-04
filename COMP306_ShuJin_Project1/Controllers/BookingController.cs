using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using COMP306_ShuJin_Project1.DTOs;
using COMP306_ShuJin_Project1.Models;
using COMP306_ShuJin_Project1.Repositories;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace COMP306_ShuJin_Project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {


        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepository, IUserRepository userRepository, IRoomRepository roomRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetAllBookings()
        {
            //var bookings = await _bookingRepository.GetAllBookingsAsync();
            //return Ok(_mapper.Map<IEnumerable<BookingDTO>>(bookings));


            var bookings = await _bookingRepository.GetAllBookingsAsync();
            var bookingDTOs = new List<BookingDTO>();

            foreach (var booking in bookings)
            {
                // Fetch room and user details for each booking
                var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId);
                var user = await _userRepository.GetUserByIdAsync(booking.UserId);

                // Map entities to DTOs
                var bookingDTO = _mapper.Map<BookingDTO>(booking);
                bookingDTO.Room = _mapper.Map<RoomDTO>(room);
                bookingDTO.User = _mapper.Map<UserDTO>(user);

                bookingDTOs.Add(bookingDTO);
            }

            return Ok(bookingDTOs);


        }

        // GET: api/Booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            //TODO: get room by booking.RoomId
            var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId); 
            //TODO: get user by booking.UserId
            var user = await _userRepository.GetUserByIdAsync(booking.UserId);

            booking.Room = room;
            booking.User = user;


            var bookingDTO = _mapper.Map<BookingDTO>(booking);
            bookingDTO.Room = _mapper.Map<RoomDTO>(room); // Map Room to RoomDTO
            bookingDTO.User = _mapper.Map<UserDTO>(user); // Map User to UserDTO

            return Ok(bookingDTO); 

        }
        
        

        // POST: api/Booking
        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] BookingDTO bookingDto)
        {  

            var room = await _roomRepository.GetRoomByIdAsync(bookingDto.RoomId);
            var user = await _userRepository.GetUserByIdAsync(bookingDto.UserId);

            if (room == null || user == null)
            {
                var msg="Room or User not found";
                System.Console.WriteLine(msg);
                return NotFound(msg);
               
            } 

            //covert date to datetime format for database
            bookingDto.StartDate = bookingDto.StartDate.ToUniversalTime().Add(DateTime.Now.TimeOfDay);
            bookingDto.EndDate = bookingDto.EndDate.ToUniversalTime().Add(DateTime.Now.TimeOfDay);


            var booking = _mapper.Map<Booking>(bookingDto);
            await _bookingRepository.AddBookingAsync(booking); 
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, _mapper.Map<BookingDTO>(booking));
        } 


        // PUT: api/Booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDTO bookingDto)
        {
            if (id != bookingDto.Id)
            {
                return BadRequest();
            }

            var bookingToUpdate = await _bookingRepository.GetBookingByIdAsync(id);
            if (bookingToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(bookingDto, bookingToUpdate);
            await _bookingRepository.UpdateBookingAsync(bookingToUpdate);
            await _bookingRepository.SaveAsync();
            return NoContent();
        }

        // PATCH: api/Booking/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchBooking(int id, [FromBody] JsonPatchDocument<BookingDTO> patchDoc)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var bookingDto = _mapper.Map<BookingDTO>(booking);
            patchDoc.ApplyTo(bookingDto, ModelState);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _mapper.Map(bookingDto, booking);
            await _bookingRepository.UpdateBookingAsync(booking);
            await _bookingRepository.SaveAsync();
            return NoContent();
        }

        // DELETE: api/Booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            await _bookingRepository.DeleteBookingAsync(id);
            await _bookingRepository.SaveAsync();
            return NoContent();
        }


        //get bookings by user id

        // //Post : api/Booking/mybookings
        [HttpGet("mybookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsForUserAsync( )
        {
            
            if (!Request.Headers.TryGetValue("UserId", out var userIdHeaderValue))
            {
                return BadRequest("UserId header is missing.");
            }

            if (!int.TryParse(userIdHeaderValue, out var userId))
            {
                return BadRequest("Invalid UserId header value.");
            }
             
            //print id
            System.Console.WriteLine("GetBookingsForUserAsync userId:"+ userId);

            var bookings = await _bookingRepository.GetBookingsForUserAsync(userId); 
            var bookingDTOs = new List<BookingDTO>();

            foreach (var booking in bookings)
            {
                // Fetch room and user details for each booking
                var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId);
                var user = await _userRepository.GetUserByIdAsync(booking.UserId);

                // Map entities to DTOs
                var bookingDTO = _mapper.Map<BookingDTO>(booking);
                bookingDTO.Room = _mapper.Map<RoomDTO>(room);
                bookingDTO.User = _mapper.Map<UserDTO>(user);

                bookingDTOs.Add(bookingDTO);
            }

            return Ok(bookingDTOs); 
        }


    }
}
