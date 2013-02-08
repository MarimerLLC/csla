using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class ProductDetail : ReadOnlyBase<ProductDetail>
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

    public static PropertyInfo<int> CategoryIdProperty = RegisterProperty<int>(c => c.CategoryId, "Quantity on hand");
    public int CategoryId
    {
      get { return GetProperty(CategoryIdProperty); }
    }

    public static PropertyInfo<int> QohProperty = RegisterProperty<int>(c => c.Qoh, "Quantity on hand");
    public int Qoh
    {
      get { return GetProperty(QohProperty); }
    }

    public static PropertyInfo<ProductLocations> LocationsProperty = RegisterProperty<ProductLocations>(c => c.Locations);
    public ProductLocations Locations
    {
      get 
      {
        if (!FieldManager.FieldExists(LocationsProperty))
          LoadProperty(LocationsProperty, new ProductLocations());
        return GetProperty(LocationsProperty); 
      }
    }

    #region Factory Methods

    public static void GetProductDetail(int id, EventHandler<DataPortalResult<ProductDetail>> callback)
    {
      var dp = new DataPortal<ProductDetail>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(new SingleCriteria<ProductDetail, int>(id));
    }

    #endregion
  }

  [Serializable]
  public class ProductLocations : ReadOnlyListBase<ProductLocations, ProductLocationInfo>
  {
  }

  [Serializable]
  public class ProductLocationInfo : ReadOnlyBase<ProductLocationInfo>
  {
    public static PropertyInfo<string> WarehouseNameProperty = RegisterProperty<string>(c => c.WarehouseName, "Warehouse name");
    public string WarehouseName
    {
      get { return GetProperty(WarehouseNameProperty); }
    }

    public static PropertyInfo<int> QohProperty = RegisterProperty<int>(c => c.Qoh, "Quantity on hand");
    public int Qoh
    {
      get { return GetProperty(QohProperty); }
    }
  }
}
