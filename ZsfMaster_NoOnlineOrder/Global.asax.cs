using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZsfProject
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
                "ShopListPageOrder",
                "shop/list/{id}/page-{p}/orderby-{s}",
                new { controller = "Shop", action = "Index", id = UrlParameter.Optional, p = UrlParameter.Optional, s = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopListPage",
                "shop/list/{id}/page-{p}",
                new { controller = "Shop", action = "Index", id = UrlParameter.Optional, p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopList",
                "shop/list/{id}",
                new { controller = "Shop", action = "Index", id = UrlParameter.Optional}
            );

            routes.MapRoute(
                "ShopSearchPageOrder",
                "shop/search/{shopName}/{id}/page-{p}/orderby-{s}",
                new { controller = "Shop", action = "Index", shopName = UrlParameter.Optional, id = UrlParameter.Optional, p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopSearchPage",
                "shop/search/{shopName}/{id}/page-{p}",
                new { controller = "Shop", action = "Index", shopName = UrlParameter.Optional, id = UrlParameter.Optional, p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopSearchCategory",
                "shop/search/{shopName}/{id}",
                new { controller = "Shop", action = "Index", shopName = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopSearch",
                "shop/search/{shopName}",
                new { controller = "Shop", action = "Index", shopName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopQSearch",
                "shop/qsearch/{shopName}",
                new { controller = "Shop", action = "QSearch", shopName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopDetailPage",
                "shop/detail/{id}/page-{p}",
                new { controller = "Shop", action = "Detail", id = UrlParameter.Optional, p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopDetail",
                "shop/detail/{id}",
                new { controller = "Shop", action = "Detail", id = UrlParameter.Optional}
            );

            routes.MapRoute(
                "ShopCreateStep",
                "shop/create/step/{i}",
                new { controller = "Shop", action = "Create", i = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShopCreate",
                "shop/create",
                new { controller = "Shop", action = "Create" }
            );
            routes.MapRoute(
                "UserCmtPage",
                "user/comments/page-{p}",
                new { controller = "User", action = "Comments", p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "UserCmt",
                "user/comments",
                new { controller = "User", action = "Comments"}
            );

            routes.MapRoute(
                "UserMsgPage",
                "user/messages/page-{p}",
                new { controller = "User", action = "Messages", p = UrlParameter.Optional }
            );

            routes.MapRoute(
                "UserMsg",
                "user/messages",
                new { controller = "User", action = "Messages"}
            );

            routes.MapRoute(
                "UserPointsPage",
                "user/points/page-{p}/usable-{u}",
                new { controller = "User", action = "Points", p = UrlParameter.Optional, u = UrlParameter.Optional }
            );

            routes.MapRoute(
                "UserPointsUsable",
                "user/points/usable-{u}",
                new { controller = "User", action = "Points", u = UrlParameter.Optional }
            );

            routes.MapRoute(
                "UserPoints",
                "user/points",
                new { controller = "User", action = "Points" }
            );

            routes.MapRoute(
                "JobListOfShopOfMine",
                "job/mine",
                new { controller = "Job", action = "Index", mine = 1 }
            );
            routes.MapRoute(
                "JobListOfShop",
                "job/list/{id}",
                new { controller = "Job", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Shop", action = "Index", id = UrlParameter.Optional }
            );
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            ViewEngines.Engines.Remove(ViewEngines.Engines.OfType<RazorViewEngine>().First());
            ViewEngines.Engines.Add(new MobileCapableRazorViewEngine());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
        
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            
            if (httpException == null)
            {
                routeData.Values.Add("action", "General");
                ZsfProject.Tools.SendEmail.SendErrorReport(exception, Request.Url.ToString());
            }
            else
            {

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "HttpError404");
                        break;
                    case 500:
                        routeData.Values.Add("action", "HttpError500");
                        ZsfProject.Tools.SendEmail.SendErrorReport(exception, Request.Url.ToString());
                        break;
                    default:
                        routeData.Values.Add("action", "General");
                        ZsfProject.Tools.SendEmail.SendErrorReport(exception, Request.Url.ToString());
                        break;
                }
            }
            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);

            // Clear the error on server.
            Server.ClearError();

            // Call target Controller and pass the routeData.
            IController errorController = new ZsfProject.Controllers.ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}