using System;
using System.Collections.Generic;
using Csla;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ : 
    BusinessListBase<$safeitemname$, $childitem$>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof($safeitemname$), "Role");
    }

    #endregion

    #region Factory Methods

    public static $safeitemname$ New$safeitemname$()
    {
      return DataPortal.Create<$safeitemname$>();
    }

    public static $safeitemname$ Get$safeitemname$(int id)
    {
      return DataPortal.Fetch<$safeitemname$>(
        new SingleCriteria<$safeitemname$, int>(id));
    }

    private $safeitemname$()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(
      SingleCriteria<$safeitemname$, int> criteria)
    {
      RaiseListChangedEvents = false;
      // TODO: load values into memory
      object childData = null;
      foreach (var item in (List<object>)childData)
        this.Add($childitem$.Get$childitem$(childData));
      RaiseListChangedEvents = true;
    }

    #endregion
  }


  [Serializable]
  public class $childitem$ : BusinessBase<$childitem$>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<int>("Id"));
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(RuleMethod, "");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowWrite("Name", "Role");
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowEdit(typeof($childitem$), "Role");
    }

    #endregion

    #region Factory Methods

    internal static $childitem$ New$childitem$()
    {
      return DataPortal.CreateChild<$childitem$>();
    }

    internal static $childitem$ Get$childitem$(object childData)
    {
      return DataPortal.FetchChild<$childitem$>(childData);
    }

    private $childitem$()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    protected override void Child_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.Child_Create();
    }

    private void Child_Fetch(object childData)
    {
      // TODO: load values
    }

    private void Child_Insert(object parent)
    {
      // TODO: insert values
    }

    private void Child_Update(object parent)
    {
      // TODO: update values
    }

    private void Child_DeleteSelf(object parent)
    {
      // TODO: delete values
    }

    #endregion
  }
}
