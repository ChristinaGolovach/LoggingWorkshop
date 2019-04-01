using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using LoggingSample_BLL.Services.Interfaces;
using NLog;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException($"The {nameof(customerService)} can not be null.");
        }

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            Logger.Info("Start getting all customers.");

            try
            {
                var customers = (await _customerService.GetAllCustomersAsync()).Select(InitCustomer);

                return Ok(customers);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Some error occured while getting all customers.");

                throw;
            }
        }

        [Route("{customerId}", Name = "Customer")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting customer with id {customerId}.");

            try
            {
                var customer = await _customerService.GetCustomer(customerId);

                if (customer == null)
                {
                    Logger.Info($"No customer with id {customerId} was found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving customer with id {customerId} to response.");

                return Ok(InitCustomer(customer));
            }
            catch (CustomerServiceException ex)
            {
                if (ex.Type == CustomerServiceException.ErrorType.WrongCustomerId)
                {
                    Logger.Warn($"Wrong customerId has been request: {customerId}", ex);
                    return BadRequest($"Wrong customerId has been request: {customerId}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting customerId {customerId}");
                throw;
            }
        }

        private object InitCustomer(CustomerModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Customer", new {customerId = model.Id}),
                orders = new UrlHelper(Request).Link("Orders", new { customerId = model.Id }),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                _customerService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}