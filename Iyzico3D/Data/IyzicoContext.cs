using Iyzico3D.Entities;
using Microsoft.EntityFrameworkCore;

namespace Iyzico3D.Data
{
    public class IyzicoContext : DbContext
    {
        public IyzicoContext(DbContextOptions<IyzicoContext> options) : base(options)
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
    }
}
