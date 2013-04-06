using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CslaMVC.Library;
using CslaMVC.ViewModels;
using Csla.Data;
using Csla.Rules;
using Csla.Web.Mvc;

namespace CslaMVC.Web.Controllers
{
    [Authorize] //must be authenicated for any action
    [HandleError]
    public class CustomersController : Csla.Web.Mvc.Controller //, Csla.Web.Mvc.IModelCreator
    {
        //
        // GET: /Customers/

        public ActionResult Index()
        {
            ViewData.Model = new CustomerListViewModel();
            return View();
        }

        //
        // GET: /Customers/IndexList

        public ActionResult IndexList()
        {
            //project business object to necessary form for JSON response for AJAX consumption
            var data = from c in new CustomerListViewModel().Customers
                       select new 
                       {
                           Name = c.Name 
                       };
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Customers/Details/5

        public ActionResult Details(int id)
        {
            ViewData.Model = new CustomerViewModel(id);
            return View();
        }

        //
        // GET: /Customers/Create

        public ActionResult Create()
        {
            ViewData.Model = new CustomerViewModel();
            return View();
        } 

        //
        // POST: /Customers/Create

        [HttpPost]
        public ActionResult Create(CustomerViewModel viewmodel)
        {
            if (SaveObject<Customer>(viewmodel.ModelObject, false))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewData.Model = viewmodel;
                return View();
            }
        }
        
        //
        // GET: /Customers/Edit/5
 
        [HasPermission(AuthorizationActions.EditObject, typeof(Customer))]
        public ActionResult Edit(int id)
        {
            ViewData.Model = new CustomerViewModel(id);
            return View();
        }

        //
        // POST: /Customers/Edit/5

        [HttpPost]
        [HasPermission(AuthorizationActions.EditObject, typeof(Customer))]
        public ActionResult Edit(int id, CustomerViewModel viewmodel)
        {
            viewmodel.CustomerNo = id; //not set by binder
            if (SaveObject<Customer>(viewmodel.ModelObject, true))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewData.Model = viewmodel;
                return View();
            }
        }

        [HasPermission(AuthorizationActions.DeleteObject, typeof(Customer))]
        public JsonResult TestAction()
        {
            return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { Message = "Success!" }
                };
        }

        //
        // GET: /Customers/Delete/5

        [HasPermission(AuthorizationActions.DeleteObject, typeof(Customer))]
        public ActionResult Delete(int id)
        {
            ViewData.Model = new CustomerViewModel(id);
            return View();
        }

        //
        // POST: /Customers/Delete/5

        [HttpPost]
        [HasPermission(AuthorizationActions.DeleteObject, typeof(Customer))]
        public ActionResult Delete(int id, CustomerViewModel viewmodel)
        {
            viewmodel.CustomerNo = id; //not set by binder
            if (SaveObject<Customer>(viewmodel.ModelObject, c => c.Delete(), true))
                return RedirectToAction("Index");
            else
            {
                ViewData.Model = viewmodel;
                return View();
            }
        }
    }
}
