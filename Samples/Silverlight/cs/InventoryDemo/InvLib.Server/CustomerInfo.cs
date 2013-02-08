using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public class CustomerInfo : ReadOnlyBase<CustomerInfo>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
    }

    public static PropertyInfo<string> LocationProperty = RegisterProperty<string>(c => c.Location);
    public string Location
    {
      get { return GetProperty(LocationProperty); }
    }

    public static PropertyInfo<int> OrderCountProperty = RegisterProperty<int>(c => c.OrderCount);
    public int OrderCount
    {
      get { return GetProperty(OrderCountProperty); }
    }
  }
}
