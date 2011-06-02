using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  public class ProjectController : Csla.Web.Mvc.Controller, Csla.Web.Mvc.IModelCreator
  {
    public object CreateModel(Type modelType)
    {
      // this CreateModel method is entirely optional, and
      // exists to demonstrate how you implement the
      // IModelCreator interface
      if (modelType.Equals(typeof(ProjectEdit)))
        return ProjectEdit.NewProject();
      else
        return Activator.CreateInstance(modelType);
    }

    //
    // GET: /Project/

    public ActionResult Index()
    {
      ViewData.Model = ProjectList.GetProjectList();
      return View();
    }

    //
    // GET: /Project/Details/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ProjectEdit))]
    public ActionResult Details(int id)
    {
      ViewData.Model = ProjectEdit.GetProject(id);
      return View();
    }

    //
    // GET: /Project/Create

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectEdit))]
    public ActionResult Create()
    {
      ViewData.Model = ProjectEdit.NewProject();
      return View();
    }

    //
    // POST: /Project/Create

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectEdit))]
    public ActionResult Create(ProjectEdit project)
    {
      if (SaveObject(project, false))
        return RedirectToAction("Index", new { id = project.Id });
      else
        return View();
    }

    //
    // GET: /Project/Edit/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectEdit))]
    public ActionResult Edit(int id)
    {
      ViewData.Model = ProjectEdit.GetProject(id);
      return View();
    }

    //
    // POST: /Project/Edit/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectEdit))]
    public ActionResult Edit(int id, ProjectEdit project)
    {
      LoadProperty(project, ProjectEdit.IdProperty, id);
      if (SaveObject(project, true))
        return RedirectToAction("Index");
      else
        return View();
    }

    //
    // GET: /Project/Delete/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectEdit))]
    public ActionResult Delete(int id)
    {
      ViewData.Model = ProjectEdit.GetProject(id);
      return View();
    }

    //
    // POST: /Project/Delete/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectEdit))]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        ProjectEdit.DeleteProject(id);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        ViewData.Model = ProjectEdit.GetProject(id);
        ModelState.AddModelError("", ex.Message);
        return View();
      }
    }
  }
}
