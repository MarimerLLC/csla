using System;
using Csla;
using Csla.Serialization;

namespace SilverlightApplication9
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<AddressEditList> AddressesProperty = RegisterProperty<AddressEditList>(c => c.Addresses, RelationshipTypes.Child | RelationshipTypes.LazyLoad); 
    public AddressEditList Addresses 
    { 
      get 
      { 
        if (!FieldManager.FieldExists(AddressesProperty)) 
        { 
          AddressListCreator.GetAddressListCreator((o, e) => 
          { 
            if (e.Error != null) 
              throw e.Error; 
            else 
              Addresses = e.Object.Result; 
          }); 
          return null; 
        } 
        else 
        { 
          return GetProperty(AddressesProperty); 
        } 
      } 
      private set 
      { 
        LoadProperty(AddressesProperty, value); 
        OnPropertyChanged(AddressesProperty); 
      } 
    }

    public static void GetRoot(EventHandler<DataPortalResult<Root>> callback)
    {
      DataPortal.BeginFetch<Root>(callback);
    }

    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<Root>.CompletedHandler handler)
    {
      var bw = new Csla.Threading.BackgroundWorker();
      bw.DoWork += (a, b) =>
        {
          Id = 123;
          //Addresses = AddressEditList.GetList();
        };
      bw.RunWorkerCompleted += (a, b) =>
        {
          handler(this, b.Error);
        };
      bw.RunWorkerAsync();
    }

  }
}
