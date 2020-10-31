using System;
using System.Collections.Generic;

namespace UsersAdmin.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserRole> UserRole { get; set; }
    }
}
