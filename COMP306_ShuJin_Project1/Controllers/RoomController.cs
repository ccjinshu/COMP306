using COMP306_ShuJin_Project1.DTOs;
using COMP306_ShuJin_Project1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace COMP306_ShuJin_Project1.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        // 获取所有房间
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            // 转换为DTO
            return Ok(rooms);
        }

        // 获取单个房间详情
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }
    }

}