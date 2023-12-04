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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _mapper.Map<UserDTO>(user));
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            var userToUpdate = await _userRepository.GetUserByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(userDto, userToUpdate);
            await _userRepository.UpdateUserAsync(userToUpdate);
            await _userRepository.SaveAsync();
            return NoContent();
        }

        // PATCH: api/User/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] JsonPatchDocument<UserDTO> patchDoc)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDTO>(user);
            patchDoc.ApplyTo(userDto, ModelState);
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _mapper.Map(userDto, user);
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveAsync();

            return NoContent();
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(id);
            await _userRepository.SaveAsync();
            return NoContent();
        }






        //login         Task<User> LoginAsync(string email, string password); 
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _userRepository.LoginAsync(loginDTO.Email, loginDTO.Password); 
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
             
        }

         
        //register
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _mapper.Map<UserDTO>(user));
        }








    }
}
