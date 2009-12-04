using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace PTWebMvc.Controllers
{
  public class ProjectController : Controller
  {
    //
    // GET: /Project/

    public ActionResult Index()
    {
      ViewData["CanEdit"] = Csla.Security.AuthorizationRules.CanEditObject(typeof(ProjectTracker.Library.Project));
      ViewData.Model = ProjectTracker.Library.ProjectList.GetProjectList();

      return View();
    }

    //
    // GET: /Project/Details/5

    public ActionResult Details(Guid id)
    {
      ViewData.Model = ProjectTracker.Library.Project.GetProject(id);

      return View();
    }

    //
    // GET: /Project/Create

    public ActionResult Create()
    {
      ViewData.Model = ProjectTracker.Library.Project.NewProject();

      return View();
    }

    //
    // POST: /Project/Create

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Create(FormCollection collection)
    {
      var project = ProjectTracker.Library.Project.NewProject();
      try
      {
        project.Name = collection["Name"];
        project.Started = collection["Started"];
        project.Ended = collection["Ended"];
        project.Description = collection["Description"];
        project = project.Save();

        return RedirectToAction("Index");
      }
      catch
      {
        ViewData.Model = project;
        return View();
      }
    }

    //
    // GET: /Project/Edit/5

    public ActionResult Edit(Guid id)
    {
      ViewData.Model = ProjectTracker.Library.Project.GetProject(id);

      return View();
    }

    //
    // POST: /Project/Edit/5

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Edit(Guid id, FormCollection collection)
    {
      var project = ProjectTracker.Library.Project.GetProject(id);
      try
      {
        project.Name = collection["Name"];
        project.Started = collection["Started"];
        project.Ended = collection["Ended"];
        project.Description = collection["Description"];
        project = project.Save();

        return RedirectToAction("Index");
      }
      catch
      {
        ViewData.Model = project;
        return View();
      }
    }
  }
}
