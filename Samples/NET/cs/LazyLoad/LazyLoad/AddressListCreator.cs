using System;
using Csla;
using Csla.Serialization;

namespace SilverlightApplication9
{
  [Serializable]
  public class AddressListCreator : ReadOnlyBase<AddressListCreator>
  {
    public static readonly PropertyInfo<AddressEditList> ResultProperty = RegisterProperty<AddressEditList>(c => c.Result);
    public AddressEditList Result
    {
      get { return ReadProperty(ResultProperty); }
      private set
      {
        LoadProperty(ResultProperty, value);
      }
    }

    public static void GetAddressListCreator(EventHandler<DataPortalResult<AddressListCreator>> callback)
    {
      DataPortal.BeginFetch<AddressListCreator>(callback);
    }

    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<AddressListCreator>.CompletedHandler handler)
    {
      var bw = new Csla.Threading.BackgroundWorker();
      bw.DoWork += (a, b) =>
      {
        b.Result = AddressEditList.GetList();
      };
      bw.RunWorkerCompleted += (a, b) =>
      {
        Result = (AddressEditList)b.Result;
        handler(this, b.Error);
      };
      bw.RunWorkerAsync();
    }
  }
}

