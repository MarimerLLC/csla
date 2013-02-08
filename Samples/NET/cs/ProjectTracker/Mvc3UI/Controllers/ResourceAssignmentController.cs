using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ResourceEdit))]
  public class ResourceAssignmentController : Controller
  {
    //
    // GET: /ResourceAssignment/

    public ActionResult Index(int id)
    {
      var resource = ResourceEdit.GetResourceEdit(id);
      ViewData.Add("ResourceId", resource.Id);
      ViewData.Add("Title", resource.FullName);
      ViewData.Model = resource.Assignments;
      return View();
    }

    //
    // GET: /ResourceAssignment/Create

    public ActionResult Create(int resourceId)
    {
      ViewData.Add("ResourceId", resourceId);
      return View();
    }

    //
    // POST: /ResourceAssignment/Create

    [HttpPost]
    public ActionResult Create(int resourceId, FormCollection collection)
    {
      try
      {
        var resource = ResourceEdit.GetResourceEdit(resourceId);
        var projectId = int.Parse(collection["ProjectId"]);
        var model = resource.Assignments.AssignTo(projectId);
        model.Role = int.Parse(collection["Role"]);
        resource = resource.Save();
        return RedirectToAction("Index", new { id = resource.Id });
      }
      catch (Exception ex)
      {
        ViewData.Add("Error", ex);
        ViewData.Add("ProjectId", resourceId);
        ViewData.Model = new ResourceAssignmentEdit();
        return View();
      }
    }

    //
    // GET: /ResourceAssignment/Edit/5

    public ActionResult Edit(int projectId, int resourceId)
    {
      var resource = ResourceEdit.GetResourceEdit(resourceId);
      ViewData.Add("ResourceId", resource.Id);
      ViewData.Model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
      return View();
    }

    //
    // POST: /ResourceAssignment/Edit/5

    [HttpPost]
    public ActionResult Edit(int projectId, int resourceId, FormCollection collection)
    {
      var resource = ResourceEdit.GetResourceEdit(resourceId);
      var model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
      try
      {
        model.Role = int.Parse(collection["Role"]);
        resource = resource.Save();
        return RedirectToAction("Index", new { id = resource.Id });
      }
      catch
      {
        ViewData.Add("ResourceId", resource.Id);
        ViewData.Model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
        return View();
      }
    }

    //
    // GET: /ResourceAssignment/Delete/5

    public ActionResult Delete(int projectId, int resourceId)
    {
      var resource = ResourceEdit.GetResourceEdit(resourceId);
      ViewData.Add("ResourceId", resource.Id);
      ViewData.Model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
      return View();
    }

    //
    // POST: /ResourceAssignment/Delete/5

    [HttpPost]
    public ActionResult Delete(int projectId, int resourceId, FormCollection collection)
    {
      var resource = ResourceEdit.GetResourceEdit(resourceId);
      var model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
      try
      {
        resource.Assignments.Remove(model);
        resource = resource.Save();
        return RedirectToAction("Index", new { id = resource.Id });
      }
      catch
      {
        ViewData.Add("ResourceId", resource.Id);
        ViewData.Model = resource.Assignments.Where(r => r.ProjectId == projectId).First();
        return View();
      }
    }
  }
}
