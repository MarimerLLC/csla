using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;
using Mvc3UI.ViewModels;

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

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ResourceEdit))]
    public ActionResult Details(int id)
    {
      ViewData.Model = new ResourceViewModel(id);
      return View();
    }

    //
    // GET: /Resource/Create

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ResourceEdit))]
    public ActionResult Create()
    {
      ViewData.Model = new ResourceViewModel();
      return View();
    }

    //
    // POST: /Resource/Create

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ResourceEdit))]
    public ActionResult Create(ResourceViewModel resource)
    {
      if (resource.Save(ModelState, false))
      {
        return RedirectToAction("Index", new { id = resource.ModelObject.Id });
      }
      else
      {
        ViewData.Model = resource;
        return View();
      }
    }

    //
    // GET: /Resource/Edit/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ResourceEdit))]
    public ActionResult Edit(int id)
    {
      ViewData.Model = new ResourceViewModel(id);
      return View();
    }

    //
    // POST: /Resource/Edit/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ResourceEdit))]
    public ActionResult Edit(int id, ResourceViewModel resource)
    {
      if (resource.Save(ModelState, true))
      {
        return RedirectToAction("Index", new { id = resource.ModelObject.Id });
      }
      else
      {
        ViewData.Model = resource;
        return View();
      }
    }

    //
    // GET: /Resource/Delete/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ResourceEdit))]
    public ActionResult Delete(int id)
    {
      ViewData.Model = new ResourceViewModel(id);
      return View();
    }

    //
    // POST: /Resource/Delete/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ResourceEdit))]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        ResourceEdit.DeleteResourceEdit(id);
        return RedirectToAction("Index");
      }
      catch
      {
        ViewData.Model = ResourceEdit.GetResourceEdit(id);
        return View();
      }
    }
  }
}
