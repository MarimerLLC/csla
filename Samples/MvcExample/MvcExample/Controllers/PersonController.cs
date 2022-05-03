using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Csla;
using BusinessLibrary;

namespace CslaMvcExample.Controllers
{
  public class PersonController : Csla.Web.Mvc.Controller
  {
    public PersonController(
      ApplicationContext applicationContext,
      IDataPortal<PersonList> personListPortal,
      IDataPortal<PersonEdit> personEditPortal,
      IDataPortal<PersonInfo> personInfoPortal)
      : base(applicationContext)
    {
      PersonListPortal = personListPortal;
      PersonEditPortal = personEditPortal;
      PersonInfoPortal = personInfoPortal;
    }

    private IDataPortal<PersonList> PersonListPortal;
    private IDataPortal<PersonEdit> PersonEditPortal;
    private IDataPortal<PersonInfo> PersonInfoPortal;

    // GET: Person
    public async Task<ActionResult> Index()
    {
      var list = await PersonListPortal.FetchAsync();
      return View(list);
    }

    // GET: Person/Details/5
    public async Task<ActionResult> Details(int id)
    {
      var obj = await PersonInfoPortal.FetchAsync(id);
      return View(obj);
    }

    // GET: Person/Create
    public async Task<ActionResult> Create()
    {
      var obj = await PersonEditPortal.CreateAsync();
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
      var obj = await PersonEditPortal.FetchAsync(id);
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
      var obj = await PersonInfoPortal.FetchAsync(id);
      return View(obj);
    }

    // POST: Person/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int id, PersonInfo person)
    {
      try
      {
        await PersonEditPortal.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View(person);
      }
    }
  }
}