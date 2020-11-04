using System;
using System.Collections.Generic;

namespace UsersAdmin.Controllers.DTOs
{
    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
