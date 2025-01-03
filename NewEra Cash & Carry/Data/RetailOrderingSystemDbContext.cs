using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Models;

namespace NewEra_Cash___Carry.Data
{
    public class RetailOrderingSystemDbContext : DbContext
    {
        public RetailOrderingSystemDbContext(DbContextOptions<RetailOrderingSystemDbContext> options) :
        base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
