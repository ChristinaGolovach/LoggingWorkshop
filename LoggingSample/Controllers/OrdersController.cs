using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using NLog;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        //TODO DI
        private readonly OrderService _orderService = new OrderService();
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("customers/{customerId}/orders", Name = "Orders")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info("Start getting all orders.");

            try
            {
                var orders = (await _orderService.GetAllOrdersAsync(customerId)).Select(InitOrder);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Some error occured while getting all orders.");

                throw;
            }
        }

        [Route("customers/{customerId}/orders/{orderId}", Name = "Order")]
        public async Task<IHttpActionResult> Get(int customerId, int orderId)
        {
            Logger.Info($"Start getting order with id {orderId} and customer {customerId}.");

            try
            {
                var order = await _orderService.GetOrderAsync(customerId, orderId);

                if (order == null)
                {
                    Logger.Info($"No customorder with id {orderId} and customerId {customerId} was found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving corder with id {orderId} and customer {customerId} to response.");

                return Ok(InitOrder(order));
            }
            catch (OrderServiceException ex)
            {
                if (ex.Type == OrderServiceException.ErrorType.WrongCustomerId)
                {
                    Logger.Warn($"Wrong customerId has been request: {customerId}", ex);
                    return BadRequest($"Wrong customerId has been request: {customerId}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting order {orderId} and customer {customerId}.");
                throw;
            }
        }

        private object InitOrder(OrderModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Order", new {customerId = model.CustomerId, orderId = model.Id}),
                customer = new UrlHelper(Request).Link("Customer", new {customerId = model.CustomerId}),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _orderService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}