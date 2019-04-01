using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using LoggingSample_BLL.Services;
using LoggingSample_BLL.Services.Interfaces;
using Ninject;
using Ninject.Web.Common;
using LoggingSample_DAL.Context;

namespace DependencyResolver
{
    public static class ResolverConfig
    {
        public static void ConfigurateResolver(this IKernel kernel)
        {
            kernel.Bind<ICustomerService>().To<CustomerService>().InRequestScope();
            kernel.Bind<IOrderService>().To<OrderService>().InRequestScope();
            kernel.Bind<DbContext>().To<AppDbContext>().InSingletonScope();
        }
    }
}
