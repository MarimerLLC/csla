using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace PTWebMvc.Controllers
{
  [HandleError]
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewData["Title"] = "Home Page";
      ViewData["Message"] = "Welcome to Project Tracker";

      return View();
    }

    public ActionResult ProjectList()
    {
      var list = ProjectTracker.Library.ProjectList.GetProjectList();

      ViewData["Title"] = "Project List";
      ViewData["Data"] = list;
      if (Csla.Security.AuthorizationRules.CanEditObject(typeof(ProjectTracker.Library.Project)))
        ViewData["Page"] = "EditProject";
      else
        ViewData["Page"] = "ShowProject";
      return View();
    }

    public ActionResult About()
    {
      ViewData["Title"] = "About Page";

      return View();
    }

    public ActionResult ShowProject(Guid id)
    {
      if (Csla.Security.AuthorizationRules.CanGetObject(
        typeof(ProjectTracker.Library.Project)))
      {
        var project = ProjectTracker.Library.Project.GetProject(id);
        ViewData["Title"] = "Project " + project.Id.ToString();
        ViewData["Data"] = project;

        if (Csla.Security.AuthorizationRules.CanEditObject(
                typeof(ProjectTracker.Library.Project)))
          return View("EditProject");
        else
          return View();
      }
      else
      {
        return View("NoAccess");
      }
    }

    [AcceptVerbs("GET")]
    public ActionResult EditProject(Guid id)
    {
      var project = ProjectTracker.Library.Project.GetProject(id);
      ViewData["Title"] = "Project " + project.Id.ToString();
      ViewData["Data"] = project;
      return View();
    }

    [AcceptVerbs("POST")]
    public ActionResult EditProject(Guid id, string Name, string Started)
    {
      var project = ProjectTracker.Library.Project.GetProject(id);
      project.Name = Name;
      project.Started = Started;
      project = project.Save();
      ViewData["Title"] = "Project " + project.Id.ToString();
      ViewData["Data"] = project;
      return View();
    }
  }
}
