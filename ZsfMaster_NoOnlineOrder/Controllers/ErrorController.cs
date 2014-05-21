using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZsfProject.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult General(Exception error)
        {
            ViewBag.Title = "网站内部错误";
            ViewBag.Description = "<h1>我们会尽快解决这个问题的！</h1><h2>现在让我们回到<a href=\"/\">主页</a>吧</h2>";
            ViewBag.ImageUrl = "/Contents/Images/500_error.gif";
            return View("Error");
        }

        public ActionResult HttpError404(Exception error)
        {
            ViewBag.Title = "该页面不存在";
            ViewBag.Description = "<h1>您找错地方了，这里没有好吃的亲！</h1><h2>让我们去<a href=\"/\">主页</a>看看</h2>";
            ViewBag.ImageUrl = "/Contents/Images/404_error.gif";
            return View("Error");
        }

        public ActionResult HttpError500(Exception error)
        {
            ViewBag.Title = "网站内部错误";
            ViewBag.Description = "<h1>我们会尽快解决这个问题的！</h1><h2>现在让我们回到<a href=\"/\">主页</a>吧</h2>";
            ViewBag.ImageUrl = "/Contents/Images/500_error.gif";
            return View("Error");
        }
    }
}
