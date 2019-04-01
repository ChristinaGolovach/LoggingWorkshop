using LoggingSample_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSample_BLL.Services.Interfaces
{
    public interface IOrderService : IDisposable
    {
        Task<IEnumerable<OrderModel>> GetAllOrdersAsync(int customerId);
        Task<OrderModel> GetOrderAsync(int customerId, int orderId);
    }
}
