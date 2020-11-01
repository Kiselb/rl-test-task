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
            user.UserRole = await _DbContext.UserRole.Include(ur => ur.Role).Where(r => r.UserId == id).ToListAsync();
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
            _DbContext.User.Add(userToAdd);
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

        [HttpPost("{id}/Roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddRole(int id, [FromBody] UserRoleDTO userRoleDTO)
        {
            if (userRoleDTO.UserId != id)
                return BadRequest();

            var user = await _DbContext.User.FindAsync(userRoleDTO.UserId);
            if (user == null)
                return NotFound();

            var role = await _DbContext.User.FindAsync(userRoleDTO.RoleId);
            if (user == null)
                return NotFound();

            var userRole = await _DbContext.UserRole.FindAsync(userRoleDTO.UserId, userRoleDTO.RoleId);
            if (userRole != null)
                return Conflict();

            var userRoleToAdd = userRoleDTO.ToModel();
            _DbContext.UserRole.Add(userRoleToAdd);
            await _DbContext.SaveChangesAsync();

            return NoContent();
        }
    }
    public static class UserRoleExtensions
    {
        public static UserRole ToModel(this UserRoleDTO userRoleDTO)
        {
            return new UserRole {
                UserId = userRoleDTO.UserId,
                RoleId = userRoleDTO.RoleId
            };
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
            List<RoleDTO> roles = null;
            if (user.UserRole != null)
            {
                roles = user.UserRole.Select(r => new RoleDTO { Id = r.RoleId, Name = r.Role.Name }).ToList<RoleDTO>();
            }

            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Roles = roles
            };
        }        
    }
}
