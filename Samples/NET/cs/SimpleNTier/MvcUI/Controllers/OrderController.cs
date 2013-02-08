using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcUI.Controllers
{
  public class OrderController : Csla.Web.Mvc.Controller
  {
    //
    // GET: /Order/

    public ActionResult Index()
    {
      ViewData.Model = BusinessLibrary.OrderList.GetList();
      return View();
    }

    //
    // GET: /Order/Details/5

    public ActionResult Details(int id)
    {
      ViewData.Model = BusinessLibrary.OrderInfo.GetOrderInfo(id);
      return View();
    }

    //
    // GET: /Order/Create

    public ActionResult Create()
    {
      ViewData.Model = BusinessLibrary.Order.NewOrder();
      return View();
    }

    //
    // POST: /Order/Create

    [HttpPost]
    public ActionResult Create(BusinessLibrary.Order item)
    {
      if (SaveObject<BusinessLibrary.Order>(item, false))
      {
        return RedirectToAction("Index");
      }
      else
      {
        ViewData.Model = item;
        return View();
      }
    }

    //
    // GET: /Order/Edit/5

    public ActionResult Edit(int id)
    {
      ViewData.Model = BusinessLibrary.Order.GetOrder(id);
      return View();
    }

    //
    // POST: /Order/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection)
    {
      var item = BusinessLibrary.Order.GetOrder(id);
      try
      {
        UpdateModel(item, collection);
        if (SaveObject<BusinessLibrary.Order>(item, true))
        {
          return RedirectToAction("Index");
        }
        else
        {
          ViewData.Model = item;
          return View();
        }
      }
      catch
      {
        ViewData.Model = item;
        return View();
      }
    }

    //
    // GET: /Order/Delete/5

    public ActionResult Delete(int id)
    {
      ViewData.Model = BusinessLibrary.Order.GetOrder(id);
      return View();
    }

    //
    // POST: /Order/Delete/5

    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        BusinessLibrary.Order.DeleteOrder(id);
        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }
  }
}
