using System;
using Microsoft.EntityFrameworkCore;

namespace UsersAdmin.Models
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.Id);
                user.Property(u => u.Login).IsRequired();
                user.Property(u => u.Name).IsRequired();
                user.Property(u => u.Email).IsRequired();
                user.Property(u => u.Password).IsRequired();
                //user.HasMany(u => u.Roles).WithOne();
            });

            modelBuilder.Entity<Role>(role =>
            {
                role.HasKey(r => r.Id);
                role.Property(r => r.Name).IsRequired();
            });
        }
    }
}
