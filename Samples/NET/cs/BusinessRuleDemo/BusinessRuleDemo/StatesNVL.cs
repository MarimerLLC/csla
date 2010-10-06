using System;
using System.Linq;
using Csla;

namespace BusinessRuleDemo
{
  [Serializable]
  public class StatesNVL : NameValueListBase<string, string>
  {
    #region Factory Methods

    private static StatesNVL _list;

    public static StatesNVL GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<StatesNVL>();
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    private StatesNVL()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;

      var data = System.IO.File.ReadAllLines("AmericanStates.txt");
      foreach (var x in data.Select(s => s.Split(',')))
      {
        this.Add(new NameValuePair(x[0].Trim(), x[1].Trim()));
      }

      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
