using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  public class ResourceController : Csla.Web.Mvc.Controller
  {
    //
    // GET: /Resource/

    public ActionResult Index()
    {
      ViewData.Model = ResourceList.GetResourceList();
      return View();
    }

    //
    // GET: /Resource/Details/5

    public ActionResult Details(int id)
    {
      ViewData.Model = ResourceEdit.GetResource(id);
      return View();
    }

    //
    // GET: /Resource/Create

    public ActionResult Create()
    {
      ViewData.Model = ResourceEdit.NewResource();
      return View();
    }

    //
    // POST: /Resource/Create

    [HttpPost]
    public ActionResult Create(ResourceEdit resource)
    {
      if (SaveObject(resource, false))
      {
        return RedirectToAction("Index", new { id = resource.Id });
      }
      else
      {
        ViewData.Model = resource;
        return View();
      }
    }

    //
    // GET: /Resource/Edit/5

    public ActionResult Edit(int id)
    {
      ViewData.Model = ResourceEdit.GetResource(id);
      return View();
    }

    //
    // POST: /Resource/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, ResourceEdit resource)
    {
      if (SaveObject(resource, true))
      {
        return RedirectToAction("Index", new { id = resource.Id });
      }
      else
      {
        ViewData.Model = resource;
        return View();
      }
    }

    //
    // GET: /Resource/Delete/5

    public ActionResult Delete(int id)
    {
      ViewData.Model = ResourceEdit.GetResource(id);
      return View();
    }

    //
    // POST: /Resource/Delete/5

    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        ResourceEdit.DeleteResource(id);
        return RedirectToAction("Index");
      }
      catch
      {
        ViewData.Model = ResourceEdit.GetResource(id);
        return View();
      }
    }
  }
}
