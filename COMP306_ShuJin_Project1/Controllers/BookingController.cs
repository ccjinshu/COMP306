using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using COMP306_ShuJin_Project1.DTOs;
using COMP306_ShuJin_Project1.Models;
using COMP306_ShuJin_Project1.Repositories;

namespace COMP306_ShuJin_Project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return Ok(_mapper.Map<IEnumerable<BookingDTO>>(bookings));
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
            return Ok(_mapper.Map<BookingDTO>(booking));
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] BookingDTO bookingDto)
        {
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
    }
}
