using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DependencyResolver;
using LoggingSample.App_Start;
using LoggingSample_BLL.Services;
using LoggingSample_BLL.Services.Interfaces;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using Ninject.Web.WebApi;
using Ninject.Web.WebApi.Filter;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]
namespace LoggingSample.App_Start
{
    public class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            //RegisterServices(kernel);
            kernel.ConfigurateResolver();

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);       
            
            return kernel;
        }
        private static void RegisterServices(IKernel kernel)
        {
        }
    }
}