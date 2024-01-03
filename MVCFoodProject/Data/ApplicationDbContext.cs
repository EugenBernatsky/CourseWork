using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MVCFoodProject.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Products> Products { get; set; }   
        public DbSet<Orders> Order { get; set; }
        public DbSet<ProductOrders> ProductOrder { get; set; }
        public DbSet<Courier> Courier { get; set; }
        public DbSet<Users> User { get; set; }
    }
}
