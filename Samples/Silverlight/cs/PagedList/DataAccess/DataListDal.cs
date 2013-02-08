using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace DataAccess
{
  public class DataListDal : Csla.Server.ObjectFactory
  {
    public Library.DataList Fetch(Library.PagedCriteria criteria)
    {
      var result = (Library.DataList)Activator.CreateInstance(typeof(Library.DataList), true);
      
      if (criteria.GetTotalRowCount)
        result.TotalRowCount = MockData.Data.Count;

      var list = (from c in MockData.Data
                  orderby c.Id
                  select DataPortal.FetchChild<Library.DataItem>(c.Id, c.Name))
                 .Skip(criteria.PageSize * criteria.Page).Take(criteria.PageSize);
      foreach (var item in list)
        result.Add(item);
      MarkOld(result);
      System.Threading.Thread.Sleep(500);
      return result;
    }
  }
}
