using System;
using Csla;
using Csla.Serialization;

namespace SilverlightApplication9
{
  [Serializable]
  public class AddressEdit : BusinessBase<AddressEdit>
  {
    public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(c => c.City);
    public string City
    {
      get { return GetProperty(CityProperty); }
      set { SetProperty(CityProperty, value); }
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    protected override void CopyStateComplete()
    {
      base.CopyStateComplete();
      OnPropertyChanged("EditLevel");
    }

    protected override void AcceptChangesComplete()
    {
      base.AcceptChangesComplete();
      OnPropertyChanged("EditLevel");
    }

    protected override void UndoChangesComplete()
    {
      base.UndoChangesComplete();
      OnPropertyChanged("EditLevel");
    }

    public static AddressEdit GetAddress(string city)
    {
      return DataPortal.FetchChild<AddressEdit>(city);
    }

    public void Child_Fetch(string city)
    {
      LoadProperty(CityProperty, city);
    }
  }
}
