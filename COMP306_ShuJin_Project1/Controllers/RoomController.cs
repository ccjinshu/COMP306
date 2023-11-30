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
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomController(IRoomRepository roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        // GET: api/Room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            return Ok(_mapper.Map<IEnumerable<RoomDTO>>(rooms));
        }

        // GET: api/Room/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<RoomDTO>(room));
        }

        // POST: api/Room
        [HttpPost]
        public async Task<ActionResult<RoomDTO>> CreateRoom([FromBody] RoomDTO roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);
            await _roomRepository.AddRoomAsync(room);
            await _roomRepository.SaveAsync();
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, _mapper.Map<RoomDTO>(room));
        }

        // PUT: api/Room/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDTO roomDto)
        {
            if (id != roomDto.Id)
            {
                return BadRequest();
            }

            var roomToUpdate = await _roomRepository.GetRoomByIdAsync(id);
            if (roomToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(roomDto, roomToUpdate);
            await _roomRepository.UpdateRoomAsync(roomToUpdate);
            await _roomRepository.SaveAsync();
            return NoContent();
        }

        // PATCH: api/Room/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchRoom(int id, [FromBody] JsonPatchDocument<RoomDTO> patchDoc)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var roomDto = _mapper.Map<RoomDTO>(room);
            patchDoc.ApplyTo(roomDto, ModelState);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _mapper.Map(roomDto, room);
            await _roomRepository.UpdateRoomAsync(room);
            await _roomRepository.SaveAsync();
            return NoContent();
        }

        // DELETE: api/Room/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            await _roomRepository.DeleteRoomAsync(id);
            await _roomRepository.SaveAsync();
            return NoContent();
        }
    }
}
