using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MVCFoodProject.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Courier>()
                .HasMany(o => o.Order)
                .WithOne(c => c.Courier)
                .HasForeignKey(x => x.CourierId)
                .OnDelete(DeleteBehavior.NoAction);
           
            modelBuilder.Entity<Users>()
                .HasMany(o => o.UserOrders)
                .WithOne(c => c.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<ProductsDetails> ProductsDetails { get; set; }
        public DbSet<Orders> Order { get; set; }
        public DbSet<ProductOrders> ProductOrder { get; set; }
        public DbSet<Courier> Courier { get; set; }
        public DbSet<Users> User { get; set; }
    }
}
