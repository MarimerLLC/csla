using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace InvLib
{
  [Serializable]
  public partial class ProductEdit : BusinessBase<ProductEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required(ErrorMessage = "Name required")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<float> PriceProperty = RegisterProperty<float>(c => c.Price);
    [System.ComponentModel.DataAnnotations.Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0")]
    public float Price
    {
      get { return GetProperty(PriceProperty); }
      set { SetProperty(PriceProperty, value); }
    }

    public static readonly PropertyInfo<int> CategoryIdProperty = RegisterProperty<int>(c => c.CategoryId);
    public int CategoryId
    {
      get { return GetProperty(CategoryIdProperty); }
      set { SetProperty(CategoryIdProperty, value); }
    }
  }
}
