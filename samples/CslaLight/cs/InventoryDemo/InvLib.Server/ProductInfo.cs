using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public class ProductInfo : ReadOnlyBase<ProductInfo>
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

    public static PropertyInfo<float> PriceProperty = RegisterProperty<float>(c => c.Price);
    public float Price
    {
      get { return GetProperty(PriceProperty); }
    }

    public static PropertyInfo<int> QohProperty = RegisterProperty<int>(c => c.Qoh);
    public int Qoh
    {
      get { return GetProperty(QohProperty); }
    }
  }
}
