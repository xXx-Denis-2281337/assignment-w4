using Microsoft.EntityFrameworkCore;

namespace KmaOoad18.Assignments.Week4.Data
{
    public class LoyaltyContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=loyalty.db");
        }
    }
}
