using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace PTWebMvc.Controllers
{
  public class ProjectController : Csla.Web.Mvc.Controller, Csla.Web.Mvc.IModelCreator
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

    //[AcceptVerbs(HttpVerbs.Post)]
    //public ActionResult Create(FormCollection collection)
    //{
    //  var project = ProjectTracker.Library.Project.NewProject();
    //  try
    //  {
    //    project.Name = collection["Name"];
    //    project.Started = collection["Started"];
    //    project.Ended = collection["Ended"];
    //    project.Description = collection["Description"];
    //    project = project.Save();

    //    return RedirectToAction("Index");
    //  }
    //  catch
    //  {
    //    ViewData.Model = project;
    //    return View();
    //  }
    //}
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Create(ProjectTracker.Library.Project project)
    {
      try
      {
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

    //[AcceptVerbs(HttpVerbs.Post)]
    //public ActionResult Edit(Guid id, FormCollection collection)
    //{
    //  var project = ProjectTracker.Library.Project.GetProject(id);
    //  try
    //  {
    //    project.Name = collection["Name"];
    //    project.Started = collection["Started"];
    //    project.Ended = collection["Ended"];
    //    project.Description = collection["Description"];
    //    project = project.Save();

    //    return RedirectToAction("Index");
    //  }
    //  catch
    //  {
    //    ViewData.Model = project;
    //    return View();
    //  }
    //}
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Edit(Guid id, FormCollection collection)
    {
      var project = ProjectTracker.Library.Project.GetProject(id);
      if (TryUpdateModel<ProjectTracker.Library.Project>(project) && 
        SaveObject<ProjectTracker.Library.Project>(project, true))
      {
        return RedirectToAction("Index");
      }
      else
      {
        ViewData.Model = project;
        return View();
      }
    }

    object Csla.Web.Mvc.IModelCreator.CreateModel(Type modelType)
    {
      if (modelType.Equals(typeof(ProjectTracker.Library.Project)))
        return ProjectTracker.Library.Project.NewProject();
      else
        return null;
    }
  }
}
