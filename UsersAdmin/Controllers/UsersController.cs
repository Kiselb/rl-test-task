using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAdmin.Models;
using UsersAdmin.Controllers.DTOs;

namespace UsersAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AdminDbContext _DbContext;

        public UsersController(AdminDbContext DbContext) {
            _DbContext = DbContext;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerator<UserDTO>>> GetAll() {
            var users = await _DbContext.User.ToArrayAsync();
            return Ok(users.Select(u => u.ToDTO()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _DbContext.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToDTO());
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDTO>> Create([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrEmpty(userDTO.Login))
                return BadRequest();
            if (string.IsNullOrEmpty(userDTO.Name))
                return BadRequest();
            if (string.IsNullOrEmpty(userDTO.Email))
                return BadRequest();
            if (string.IsNullOrEmpty(userDTO.Password))
                return BadRequest();

            // var existingUser = await _DbContext.User.FirstAsync<User>(u => u.Login == userDTO.Login);
            // if (existingUser != null)
            //     return Conflict();

            // existingUser = await _DbContext.User.FirstAsync<User>(u => u.Email == userDTO.Email);
            // if (existingUser != null)
            //     return Conflict();

            var existingUsers = await _DbContext.User.Where(u => u.Login == userDTO.Login).ToListAsync();
            if (existingUsers.Count != 0)
            {
                 return Conflict();
            }

            existingUsers = await _DbContext.User.Where(u => u.Email == userDTO.Email).ToListAsync();
            if (existingUsers.Count != 0)
            {
                 return Conflict();
            }

            var userToAdd = userDTO.ToModel();
            _DbContext.Add(userToAdd);
            await _DbContext.SaveChangesAsync();

            var createdUserDTO = userToAdd.ToDTO();

            return CreatedAtAction(nameof(Get), new { Id = createdUserDTO.Id }, createdUserDTO);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UserDTO userDTO)
        {
            if (userDTO.Id != id)
                return BadRequest();

            var user = await _DbContext.User.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Update(userDTO);
            await _DbContext.SaveChangesAsync();

            return NoContent();
        }
    }
    public static class UserExtensions
    {
        public static User ToModel(this UserDTO userDTO)
        {
            return new User {
                Id = userDTO.Id,
                Login = userDTO.Login,
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = userDTO.Password
            };
        }

        public static void Update(this User userToUpdate, UserDTO userDTO)
        {
            if (userDTO.Id != userToUpdate.Id) throw new NotSupportedException();
            userToUpdate.Name = userDTO.Name;
            userToUpdate.Email = userDTO.Email;
            userToUpdate.Password = userDTO.Password;
        }

        public static UserDTO ToDTO(this User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password
            };
        }        
    }
}
