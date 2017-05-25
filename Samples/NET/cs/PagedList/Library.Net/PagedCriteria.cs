using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Library
{
  [Serializable]
  public class PagedCriteria : CriteriaBase<PagedCriteria>
  {
    public static PropertyInfo<int> PageSizeProperty = RegisterProperty<int>(c => c.PageSize);
    public int PageSize
    {
      get { return ReadProperty(PageSizeProperty); }
      set { LoadProperty(PageSizeProperty, value); }
    }

    public static PropertyInfo<int> PageProperty = RegisterProperty<int>(c => c.Page);
    public int Page
    {
      get { return ReadProperty(PageProperty); }
      set { LoadProperty(PageProperty, value); }
    }

    public static PropertyInfo<bool> GetTotalRowCountProperty = RegisterProperty<bool>(c => c.GetTotalRowCount);
    public bool GetTotalRowCount
    {
      get { return ReadProperty(GetTotalRowCountProperty); }
      set { LoadProperty(GetTotalRowCountProperty, value); }
    }

    public PagedCriteria()
    { }

    public PagedCriteria(int page, int pageSize)
      : this(page, pageSize, false)
    { }

    public PagedCriteria(int page, int pageSize, bool getRowCount)
    {
      PageSize = pageSize;
      Page = page;
      GetTotalRowCount = getRowCount;
    }
  }
}
