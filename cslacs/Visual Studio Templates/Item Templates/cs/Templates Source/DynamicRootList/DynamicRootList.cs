using System;
using System.Collections.Generic;
using Csla;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ :
    EditableRootListBase<$childitem$>
  {
    #region Business Methods

    protected override object AddNewCore()
    {
      $childitem$ item = $childitem$.New$childitem$();
      Add(item);
      return item;
    }

    #endregion

    #region  Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof($safeitemname$), "Role");
      //AuthorizationRules.AllowEdit(typeof($safeitemname$), "Role");
    }

    #endregion

    #region  Factory Methods

    public static $safeitemname$ New$safeitemname$()
    {
      return DataPortal.Create<$safeitemname$>();
    }

    public static $safeitemname$ Get$safeitemname$()
    {
      return DataPortal.Fetch<$safeitemname$>();
    }

    private $safeitemname$()
    {
      // require use of factory methods
      AllowNew = true;
    }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      // TODO: load values
      RaiseListChangedEvents = false;
      object listData = null;
      foreach (var item in (List<object>)listData)
        Add($childitem$.Get$childitem$(item));
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
      //AuthorizationRules.AllowEdit(typeof($safeitemname$), "Role");
    }

    #endregion

    #region Factory Methods

    public static $childitem$ New$childitem$()
    {
      return DataPortal.Create<$childitem$>();
    }

    internal static $childitem$ Get$childitem$(object rootData)
    {
      return new $childitem$(rootData);
    }

    private $childitem$()
    { /* Require use of factory methods */ }

    private $childitem$(object rootData)
    {
      Fetch(rootData);
    }

    #endregion

    #region Data Access

    private void Fetch(object rootData)
    {
      // TODO: load values from rootData
    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert values
    }

    protected override void DataPortal_Update()
    {
      // TODO: update values
    }

    protected override void DataPortal_DeleteSelf()
    {
      // TODO: delete values
    }

    #endregion
  }
}