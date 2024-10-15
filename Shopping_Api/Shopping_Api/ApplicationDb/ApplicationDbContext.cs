using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore; 
using Shopping_Api.Models; 

namespace Shopping_Api.ApplicationDb
{
    // ApplicationDbContext class that extends IdentityDbContext for ASP.NET Core Identity
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        // Constructor that takes DbContextOptions and passes it to the base IdentityDbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for Products table in the database, representing the Product model
        public DbSet<Product> Products { get; set; }

        // DbSet for ProductImages table in the database, representing the ProductImage model
        public DbSet<ProductImage> ProductImages { get; set; }

        // DbSet for Orders table in the database, representing the Order model
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
