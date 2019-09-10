using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CslaMVC.Library;
using CslaMVC.ViewModels;
using Csla.Data;
using Csla.Web.Mvc;

namespace CslaMVC.Web.Controllers
{
    public class OrdersController : Csla.Web.Mvc.Controller, IModelCreator //, Controller
    {
        //
        // GET: /Orders/

        public ActionResult Index(int? id = 1)
        {
            ViewData.Model = OrdersList.GetOrdersList((int)id);
            return View();
        }

        //
        // GET: /Orders/Details/5

        public ActionResult Details(Guid id)
        {
            ViewData.Model = Order.GetOrder(id);
            return View();
        }

        //
        // GET: /Orders/Create

        public ActionResult Create(int id)
        {
            var order = Order.NewOrder();
            order.CustomerNo = id;
            ViewData.Model = order;
            return View();
        } 

        //
        // POST: /Orders/Create

        [HttpPost]
        public ActionResult Create(Order order)
        {
            if (SaveObject<Order>(order, false))
            {
                return RedirectToAction("Index", new { id = order.CustomerNo });
            }
            else
            {
                ViewData.Model = order;
                return View();
            }
        }
        
        //
        // GET: /Orders/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Orders/Edit/5

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
        // GET: /Orders/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Orders/Delete/5

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

        #region IModelCreator Members

        public object CreateModel(Type modelType)
        {
            return Order.NewOrder();
        }

        #endregion
    }
}
