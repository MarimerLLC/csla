using System;
using System.Collections.Generic;
using Csla;

namespace $rootnamespace$

{
  [Serializable]
  public class $safeitemname$ : 
    ReadOnlyListBase<$safeitemname$, $childitem$>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof($safeitemname$), "Role");
    }

    #endregion

    #region Factory Methods

    public static $safeitemname$ Get$safeitemname$(string filter)
    {
      return DataPortal.Fetch<$safeitemname$>(
        new SingleCriteria<$safeitemname$, string>(filter));
    }

    private $safeitemname$()
    { /* require use of factory methods */ }

    #endregion


    #region Data Access

    private void DataPortal_Fetch(
      SingleCriteria<$safeitemname$, string> criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      object objectData = null;
      foreach (var child in (List<object>)objectData)
        Add($childitem$.Get$childitem$(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }


  [Serializable]
  public class $childitem$ : ReadOnlyBase<$childitem$>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with managed backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    // example with private backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<string>("Name", "Name"));
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
    }

    #endregion

    #region Factory Methods

    internal static $childitem$ Get$childitem$(object childData)
    {
      return DataPortal.FetchChild<$childitem$>(childData);
    }

    private $childitem$()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void Child_Fetch(object childData)
    {
      // TODO: load values from childData
    }

    #endregion
  }
}
