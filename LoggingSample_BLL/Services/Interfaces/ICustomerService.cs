using LoggingSample_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSample_BLL.Services.Interfaces
{
    public interface ICustomerService : IDisposable
    {
        Task<IEnumerable<CustomerModel>> GetAllCustomersAsync();
        Task<CustomerModel> GetCustomer(int customerId);
    }
}
