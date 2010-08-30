using System;
using Csla;

namespace InvLib
{
  public partial class ProductEdit
  {
    #region Factory Methods

    public static void NewProductEdit(EventHandler<DataPortalResult<ProductEdit>> callback)
    {
      var dp = new DataPortal<ProductEdit>(DataPortal.ProxyModes.LocalOnly);
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    public static void GetProductEdit(int id, EventHandler<DataPortalResult<ProductEdit>> callback)
    {
      var dp = new DataPortal<ProductEdit>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(new SingleCriteria<ProductEdit, int>(id));
    }

    public override void BeginSave(bool forceUpdate, EventHandler<Csla.Core.SavedEventArgs> handler, object userState)
    {
      base.BeginSave(
        forceUpdate,
        (o, e1) =>
        {
          ProductList.ClearCache();
          if (handler != null)
            handler(o, e1);
        },
        userState);
    }

    #endregion

    #region Data Access

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Create(Csla.DataPortalClient.LocalProxy<ProductEdit>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        CategoryId = 3;
      }
      BusinessRules.CheckRules();
      handler(this, null);
    }

    #endregion
  }
}
