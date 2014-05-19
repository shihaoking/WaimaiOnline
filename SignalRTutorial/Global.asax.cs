using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

namespace SignalRTutorial
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            RouteTable.Routes.MapHubs(hubConfiguration);

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public class ErrorHandlingPipelineModule : HubPipelineModule
        {
            protected override void OnIncomingError(Exception ex, IHubIncomingInvokerContext context)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
                if(ex.InnerException != null)
                {
                    Debug.WriteLine("=> Inner Exception " + ex.InnerException.Message);
                }
                base.OnIncomingError(ex, context);
            }
        }
    }
}