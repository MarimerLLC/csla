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
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    public static PropertyInfo<float> PriceProperty = RegisterProperty<float>(c => c.Price);
    public float Price
    {
      get { return GetProperty(PriceProperty); }
      private set { LoadProperty(PriceProperty, value); }
    }

    public static PropertyInfo<int> QohProperty = RegisterProperty<int>(c => c.Qoh);
    public int Qoh
    {
      get { return GetProperty(QohProperty); }
      private set { LoadProperty(QohProperty, value); }
    }

    public void UpdateItem(ProductEdit product)
    {
      Name = product.Name;
      OnPropertyChanged(NameProperty);
      Price = product.Price;
      OnPropertyChanged(PriceProperty);
    }
  }
}
