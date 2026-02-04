using Iyzico3D.Entities;
using Microsoft.EntityFrameworkCore;

namespace Iyzico3D.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IyzicoContext _context;

        public OrderRepository(IyzicoContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Order?> GetByOrderNo(string orderNo)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
