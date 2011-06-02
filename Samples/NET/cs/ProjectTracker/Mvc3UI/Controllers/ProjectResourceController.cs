using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library;

namespace Mvc3UI.Controllers
{
  [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectEdit))]
  public class ProjectResourceController : Controller
  {
    //
    // GET: /ProjectResource/

    public ActionResult Index(int id)
    {
      var project = ProjectEdit.GetProject(id);
      ViewData.Add("ProjectId", project.Id);
      ViewData.Add("Title", project.Name);
      ViewData.Model = project.Resources;
      return View();
    }

    //
    // GET: /ProjectResource/Create

    public ActionResult Create(int projectId)
    {
      ViewData.Add("ProjectId", projectId);
      ViewData.Add("Roles", new SelectList(ProjectTracker.Library.RoleList.GetList(), "Key", "Value"));
      ViewData.Add("Resources", new SelectList(ProjectTracker.Library.ResourceList.GetResourceList(), "Id", "Name"));
      return View();
    }

    //
    // POST: /ProjectResource/Create

    [HttpPost]
    public ActionResult Create(int projectId, FormCollection collection)
    {
      try
      {
        var resourceId = int.Parse(collection["ResourceId"]);
        var model = ProjectResourceEditCreator.GetProjectResourceEditCreator(resourceId).Result;
        model.Role = int.Parse(collection["Role"]);
        model = ProjectResourceUpdater.Update(projectId, model);
        return RedirectToAction("Index", new { id = projectId });
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
        return Create(projectId);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
        return Create(projectId);
      }
    }

    //
    // GET: /ProjectResource/Edit/5

    public ActionResult Edit(int projectId, int resourceId)
    {
      ViewData.Add("ProjectId", projectId);
      var model = ProjectResourceEditCreator.GetProjectResourceEditCreator(projectId, resourceId).Result;
      ViewData.Model = model;
      return View();
    }

    //
    // POST: /ProjectResource/Edit/5

    [HttpPost]
    public ActionResult Edit(int projectId, int resourceId, FormCollection collection)
    {
      var model = ProjectResourceEditCreator.GetProjectResourceEditCreator(projectId, resourceId).Result;
      try
      {
        model.Role = int.Parse(collection["Role"]);
        model = ProjectResourceUpdater.Update(projectId, model);
        return RedirectToAction("Index", new { id = projectId });
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
        ViewData.Add("ProjectId", projectId);
        ViewData.Model = model;
        return View();
      }
      catch (Exception ex)
      {
        ViewData.Add("ProjectId", projectId);
        ViewData.Model = model;
        ModelState.AddModelError("", ex.Message);
        return View();
      }
    }

    //
    // GET: /ProjectResource/Delete/5

    public ActionResult Delete(int projectId, int resourceId)
    {
      var project = ProjectEdit.GetProject(projectId);
      ViewData.Add("ProjectId", project.Id);
      ViewData.Model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      return View();
    }

    //
    // POST: /ProjectResource/Delete/5

    [HttpPost]
    public ActionResult Delete(int projectId, int resourceId, FormCollection collection)
    {
      var project = ProjectEdit.GetProject(projectId);
      var model = project.Resources.Where(r => r.ResourceId == resourceId).First();
      try
      {
        project.Resources.Remove(model);
        project = project.Save();
        return RedirectToAction("Index", new { id = project.Id });
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
        return Delete(projectId, resourceId);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
        return Delete(projectId, resourceId);
      }
    }
  }
}
