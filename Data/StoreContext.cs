using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions options) : base(options)
        {
        }
        // represent products table in DB
        public DbSet<Product> Products { get; set; }

        public DbSet<Basket> Baskets { get; set; }
    }
}