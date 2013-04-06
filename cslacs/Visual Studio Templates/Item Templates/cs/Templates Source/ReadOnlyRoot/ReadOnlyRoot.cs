using System;
using Csla;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ : ReadOnlyBase<$safeitemname$>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with managed backing field
    private static PropertyInfo<int> IdProperty = 
      RegisterProperty(typeof($safeitemname$), new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    // example with private backing field
    private static PropertyInfo<string> NameProperty = 
      RegisterProperty(typeof($safeitemname$), new PropertyInfo<string>("Name", "Name"));
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowRead("Name", "Role");
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof($safeitemname$), "Role");
    }

    #endregion

    #region Factory Methods

    public static $safeitemname$ GetReadOnlyRoot(int id)
    {
      return DataPortal.Fetch<$safeitemname$>(
        new SingleCriteria<$safeitemname$, int>(id));
    }

    private $safeitemname$()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(SingleCriteria<$safeitemname$, int> criteria)
    {
      // TODO: load values
    }

    #endregion
  }
}
