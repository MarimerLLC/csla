using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectTracker.Library.Admin;

namespace Mvc3UI.Controllers
{
  public class RoleController : Csla.Web.Mvc.Controller
  {
    //
    // GET: /Role/

    public ActionResult Index()
    {
      ViewData.Model = RoleEditList.GetRoles();
      return View();
    }

    //
    // GET: /Role/Create

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(RoleEditList))]
    public ActionResult Create()
    {
      ViewData.Model = RoleEditManager.NewRoleEdit();
      return View();
    }

    //
    // POST: /Role/Create

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(RoleEditList))]
    public ActionResult Create(RoleEdit role)
    {
      try
      {
        RoleEditManager.SaveRoleEdit(role);
        return RedirectToAction("Index");
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
      }
      ViewData.Model = role;
      return View();
    }

    //
    // GET: /Role/EditList

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(RoleEditList))]
    public ActionResult EditList()
    {
      ViewData.Model = RoleEditList.GetRoles();
      return View();
    }

    //
    // POST: /Role/EditList

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(RoleEditList))]
    public ActionResult EditList(FormCollection collection)
    {
      var list = RoleEditList.GetRoles();
      UpdateModel(list, collection);
      SaveObject(list, false);
      return View();
    }

    //
    // GET: /Role/Edit/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(RoleEditList))]
    public ActionResult Edit(int id)
    {
      ViewData.Model = RoleEditManager.GetRoleEdit(id);
      return View();
    }

    //
    // POST: /Role/Edit/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(RoleEditList))]
    public ActionResult Edit(int id, RoleEdit role)
    {
      LoadProperty(role, RoleEdit.IdProperty, id);
      try
      {
        RoleEditManager.SaveRoleEdit(role);
        return RedirectToAction("Index");
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
      }
      // failure condition
      ViewData.Model = role;
      return View();
    }

    //
    // GET: /Role/Delete/5

    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(RoleEditList))]
    public ActionResult Delete(int id)
    {
      ViewData.Model = RoleEditManager.GetRoleEdit(id);
      return View();
    }

    //
    // POST: /Role/Delete/5

    [HttpPost]
    [Csla.Web.Mvc.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(RoleEditList))]
    public ActionResult Delete(int id, RoleEdit role)
    {
      try
      {
        var list = RoleEditList.GetRoles();
        var item = list.GetRoleById(id);
        list.Remove(item);
        list.Save();
        return RedirectToAction("Index");
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
      }
      // failure condition
      LoadProperty(role, RoleEdit.IdProperty, id);
      ViewData.Model = role;
      return View();
    }
  }
}
