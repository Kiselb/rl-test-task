using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UsersAdmin.Models;
using UsersAdmin.Controllers.DTOs;

namespace UsersAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AdminDbContext _DbContext;

        public RolesController(AdminDbContext DbContext) {
            _DbContext = DbContext;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerator<RoleDTO>>> GetAll() {
            var roles = await _DbContext.Role.ToArrayAsync();
            return Ok(roles.Select(r => r.ToDTO()));
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _DbContext.Role.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }
            return Ok(role.ToDTO());
        }
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoleDTO>> Create([FromBody] RoleDTO roleDTO)
        {
            if (string.IsNullOrEmpty(roleDTO.Name))
                return BadRequest();

            // var existingRole = await _DbContext.Role.FirstAsync<Role>(r => r.Name == roleDTO.Name,);
            // if (existingRole != null)
            //     return Conflict();
            
            var roles = await _DbContext.Role.Where(r => r.Name == roleDTO.Name).ToListAsync();
            if (roles.Count != 0)
            {
                 return Conflict();
            }

            var roleToAdd = roleDTO.ToModel();
            _DbContext.Add(roleToAdd);
            await _DbContext.SaveChangesAsync();

            var createdRoleDTO = roleToAdd.ToDTO();

            return CreatedAtAction(nameof(Get), new { Id = createdRoleDTO.Id }, createdRoleDTO);
        }
    }
    public static class RoleExtensions
    {
        public static Role ToModel(this RoleDTO roleDTO)
        {
            return new Role {
                Id = roleDTO.Id,
                Name = roleDTO.Name,
            };
        }

        public static void Update(this Role roleToUpdate, RoleDTO roleDTO)
        {
            if (roleDTO.Id != roleToUpdate.Id) throw new NotSupportedException();
            roleToUpdate.Name = roleDTO.Name;
        }

        public static RoleDTO ToDTO(this Role role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
            };
        }        
    }
}
