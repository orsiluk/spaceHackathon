using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace RoverServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            GlobalConfiguration.Configuration.Properties.TryGetValue("RobotMessage", out object objMessage);
            ViewBag.RobotMessage = (string) objMessage ?? "No message yet";

            return View();
        }
    }
}
