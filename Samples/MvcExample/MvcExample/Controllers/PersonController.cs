using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Csla;
using BusinessLibrary;

namespace CslaMvcExample.Controllers
{
  public class PersonController : Csla.Web.Mvc.Controller
  {
    // GET: Person
    public async Task<ActionResult> Index()
    {
      var list = await DataPortal.FetchAsync<PersonList>();
      return View(list);
    }

    // GET: Person/Details/5
    public async Task<ActionResult> Details(int id)
    {
      var obj = await DataPortal.FetchAsync<PersonInfo>(id);
      return View(obj);
    }

    // GET: Person/Create
    public async Task<ActionResult> Create()
    {
      var obj = await DataPortal.CreateAsync<PersonEdit>();
      return View(obj);
    }

    // POST: Person/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(PersonEdit person)
    {
      try
      {
        if (await SaveObjectAsync<PersonEdit>(person, false))
          return RedirectToAction(nameof(Index));
        else
          return View(person);
      }
      catch
      {
        return View(person);
      }
    }

    // GET: Person/Edit/5
    public async Task<ActionResult> Edit(int id)
    {
      var obj = await DataPortal.FetchAsync<PersonEdit>(id);
      return View(obj);
    }

    // POST: Person/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int id, PersonEdit person)
    {
      try
      {
        LoadProperty(person, PersonEdit.IdProperty, id);
        if (await SaveObjectAsync<PersonEdit>(person, true))
          return RedirectToAction(nameof(Index));
        else
          return View(person);
      }
      catch
      {
        return View(person);
      }
    }

    // GET: Person/Delete/5
    public async Task<ActionResult> Delete(int id)
    {
      var obj = await DataPortal.FetchAsync<PersonInfo>(id);
      return View(obj);
    }

    // POST: Person/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int id, PersonInfo person)
    {
      try
      {
        await DataPortal.DeleteAsync<PersonEdit>(id);
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View(person);
      }
    }
  }
}