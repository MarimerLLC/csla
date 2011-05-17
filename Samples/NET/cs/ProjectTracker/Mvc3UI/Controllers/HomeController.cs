using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc3UI.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Project Tracker Application";

      return View();
    }

    public ActionResult About()
    {
      return View();
    }
  }
}
