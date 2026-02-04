using Iyzico3D.Entities;

namespace Iyzico3D.Data
{
    public interface IOrderRepository
    {
        Task<bool> CreateAsync(Order order);
        Task<bool> UpdateAsync(Order order);
        Task<Order?> GetByOrderNo(string orderNo); 
    }
}
