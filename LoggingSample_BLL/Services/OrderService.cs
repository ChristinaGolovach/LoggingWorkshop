using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSample_BLL.Services
{
    public class OrderService : IDisposable
    {
        //TODO DI
        private readonly AppDbContext _context = new AppDbContext();
        private readonly int wrongCustomerId = 56;

        public async Task<IEnumerable<OrderModel>> GetAllOrdersAsync(int customerId)
        {
            return (await _context.Orders.Where(item => item.CustomerId == customerId).ToListAsync()).Select(item => item.Map());
        }

        public async Task<OrderModel> GetOrderAsync(int customerId, int orderId)
        {
            if (customerId == wrongCustomerId)
            {
                throw new OrderServiceException("Wrong id has been requested", OrderServiceException.ErrorType.WrongCustomerId);
            }
            return (await _context.Orders.SingleOrDefaultAsync(item => item.Id == orderId && item.CustomerId == customerId))?.Map();
        }                  

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    //TODO move out
    public class OrderServiceException : Exception
    {
        public enum ErrorType
        {
            WrongCustomerId,
            WrongOrderId
        }

        public ErrorType Type { get; set; }

        public OrderServiceException(string message, ErrorType errorType) : base(message)
        {
            Type = errorType;
        }
    }
}
