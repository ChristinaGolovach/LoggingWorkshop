using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services.Interfaces;
using LoggingSample_DAL.Entities;

namespace LoggingSample_BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DbContext _context;
        private readonly int wrongCustomerId = 56;

        public CustomerService(DbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException($"The {nameof(dbContext)} can not be null.");
        }

        public async Task<IEnumerable<CustomerModel>> GetAllCustomersAsync()
        {
            var result = (await _context.Set<Customer>().ToListAsync()).Select(item => item.Map());
          
            return result;
        }
        
        public Task<CustomerModel> GetCustomer(int customerId)
        {
            if (customerId == wrongCustomerId)
            {
                throw new CustomerServiceException("Wrong id has been requested",
                    CustomerServiceException.ErrorType.WrongCustomerId);
            }

            return _context.Set<Customer>().SingleOrDefaultAsync(item => item.Id == customerId).ContinueWith(task =>
            {
                var customer = task.Result;

                return customer?.Map();
            });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class CustomerServiceException : Exception
    {
        public enum ErrorType
        {
            WrongCustomerId
        }

        public ErrorType Type { get; set; }

        public CustomerServiceException(string message, ErrorType errorType): base(message)
        {
           Type = errorType;
        }
    }
}