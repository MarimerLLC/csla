using System;
using System.Linq;
using Csla;

namespace BusinessRuleDemo
{
  [Serializable]
  public class CountryNVL : NameValueListBase<string, string>
  {
    public const string UnitedStates = "US";

    #region Factory Methods

    private static CountryNVL _list;

    public static CountryNVL GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<CountryNVL>();
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    public CountryNVL()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;

      var data = System.IO.File.ReadAllLines("CountryCodes.txt");
      foreach (var x in data.Select(s => s.Split(',')))
      {
        this.Add(new NameValuePair(x[0].Trim(), x[1].Trim()));
      }

      // TODO: load values
      //object listData = null;
      //foreach (var item in listData)
      //  Add(new NameValueListBase<int, string>.
      //    NameValuePair(item.Key, item.Value));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
