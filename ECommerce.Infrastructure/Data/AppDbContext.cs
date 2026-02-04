using ECommerce.Core.Account.Entites;
using ECommerce.Core.Entites;
using ECommerce.Core.Entites.Order;
using ECommerce.Core.Entites.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _configuration;
        //
        public DbSet<Product> Products { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var ADMIN_ROLE_ID = "fab4fac1-c546-41de-aebc-a14da6895711";
            var USER_ROLE_ID = "b74ddd14-6340-4840-95c2-db12554843e5";

            var ADMIN1_USER_ID = "5f2d9e8a-b1c4-4e7a-9d6b-3f2a1c5d8e90";
            var ADMIN2_USER_ID = "a1b2c3d4-e5f6-4a5b-bc6d-7e8f9a0b1c2d";

            var admin1Email = _configuration["AdminData:Admin1:Email"];
            var admin1Pass = _configuration["AdminData:Admin1:Password"];
            var admin2Email = _configuration["AdminData:Admin2:Email"];
            var admin2Pass = _configuration["AdminData:Admin2:Password"];

            // 1. Roles Seed
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = ADMIN_ROLE_ID, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = USER_ROLE_ID, Name = "User", NormalizedName = "USER" }
            );

            var ph = new PasswordHasher<AppUser>();

            // 2. Admin 1 Seed
            var admin1 = new AppUser
            {
                Id = ADMIN1_USER_ID,
                UserName = admin1Email,
                NormalizedUserName = admin1Email.ToUpper(),
                Email = admin1Email,
                NormalizedEmail = admin1Email.ToUpper(),
                EmailConfirmed = true,
                IsActive = true, 
                SecurityStamp = "STATIC_STAMP_ADMIN_1",
                ConcurrencyStamp = "STATIC_CONCURRENCY_1" 
            };
            admin1.PasswordHash = ph.HashPassword(admin1, admin1Pass);

            // 3. Admin 2 Seed
            var admin2 = new AppUser
            {
                Id = ADMIN2_USER_ID,
                UserName = admin2Email,
                NormalizedUserName = admin2Email.ToUpper(),
                Email = admin2Email,
                NormalizedEmail = admin2Email.ToUpper(),
                EmailConfirmed = true,
                IsActive = true, 
                SecurityStamp = "STATIC_STAMP_ADMIN_2",
                ConcurrencyStamp = "STATIC_CONCURRENCY_2" 
            };
            admin2.PasswordHash = ph.HashPassword(admin2, admin2Pass);

            modelBuilder.Entity<AppUser>().HasData(admin1, admin2);

            // 4. Map Roles
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = ADMIN_ROLE_ID, UserId = ADMIN1_USER_ID },
                new IdentityUserRole<string> { RoleId = ADMIN_ROLE_ID, UserId = ADMIN2_USER_ID }
            );

            // 5. Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Devices and Gadgets" },
                new Category { Id = 2, Name = "Home Appliances", Description = "Kitchen and Home tools" }
            );

            // 6. Delivery Methods
            modelBuilder.Entity<DeliveryMethod>().HasData(
                new DeliveryMethod { Id = 1, Name = "FST", DeliveryTime = "only one week", Description = "the best", Price = 15m },
                new DeliveryMethod { Id = 2, Name = "DBL", DeliveryTime = "two week", Description = "safe product", Price = 10m }
            );
        }

    }

}

