using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Infrastructure.Data
{
    public class RetailOrderingSystemDbContext : DbContext
    {
        public RetailOrderingSystemDbContext(DbContextOptions<RetailOrderingSystemDbContext> options) :
        base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define composite primary key for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Specify column types for decimal properties
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Customer" }
            );

            // Hash passwords using Bcrypt
            var hashedPassword1 = BCrypt.Net.BCrypt.HashPassword("password1");
            var hashedPassword2 = BCrypt.Net.BCrypt.HashPassword("password2");

            // Seed users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, PhoneNumber = "1234567890", PasswordHash = hashedPassword1 },
                new User { Id = 2, PhoneNumber = "0987654321", PasswordHash = hashedPassword2 }
            );

            // Seed user roles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 },
                new UserRole { UserId = 2, RoleId = 2 }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}

