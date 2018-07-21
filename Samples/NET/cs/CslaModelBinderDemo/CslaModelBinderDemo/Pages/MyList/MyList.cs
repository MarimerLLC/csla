using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csla;

namespace WebApplication3.Pages.MyList
{
  public static class MockDb
  {
    public static List<DbItem> Items = new List<DbItem>
    {
      new DbItem { Id = 1, Name = "Rocky", City = "Aitkin"},
      new DbItem { Id = 3, Name = "Teresa", City = "Bemidji"},
      new DbItem { Id = 2, Name = "Tim", City = "St. Paul"}
    };

    public class DbItem
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public string City { get; set; }
    }
  }

  [Serializable]
  public class MyList : BusinessListBase<MyList, MyItem>
  {
    private void DataPortal_Fetch()
    {
      AddRange(MockDb.Items.Select(i => DataPortal.FetchChild<MyItem>(i)));
    }

    protected override void DataPortal_Update()
    {
      Child_Update();
    }
  }
}
