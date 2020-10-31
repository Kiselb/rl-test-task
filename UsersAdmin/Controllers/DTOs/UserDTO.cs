using System;

namespace UsersAdmin.Controllers.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
