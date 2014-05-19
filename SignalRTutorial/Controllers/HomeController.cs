using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRTutorial.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string userName = Request.QueryString["userName"];
            HttpCookie cookie = Request.Cookies.Get("userName");

            if(cookie == null)
            {
                cookie = new HttpCookie("userName");
                cookie.Value = userName;
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Set(cookie);
            }
            return View();
        }

    }
}
