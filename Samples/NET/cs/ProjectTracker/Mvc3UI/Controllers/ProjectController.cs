using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  public class ProjectController : Csla.Web.Mvc.Controller
  {
    //
    // GET: /Project/

    public ActionResult Index()
    {
      ViewData.Model = ProjectList.GetProjectList();
      return View();
    }

    //
    // GET: /Project/Details/5

    public ActionResult Details(int id)
    {
      ViewData.Model = ProjectEdit.GetProject(id);
      return View();
    }

    //
    // GET: /Project/Create

    public ActionResult Create()
    {
      ViewData.Model = ProjectEdit.NewProject();
      return View();
    }

    //
    // POST: /Project/Create

    [HttpPost]
    public ActionResult Create(ProjectEdit project)
    {
      if (SaveObject(project, false))
      {
        return RedirectToAction("Index", new { id = project.Id });
      }
      else
      {
        ViewData.Model = project;
        return View();
      }
    }

    //
    // GET: /Project/Edit/5

    public ActionResult Edit(int id)
    {
      return View();
    }

    //
    // POST: /Project/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add update logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Project/Delete/5

    public ActionResult Delete(int id)
    {
      return View();
    }

    //
    // POST: /Project/Delete/5

    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }
  }
}
