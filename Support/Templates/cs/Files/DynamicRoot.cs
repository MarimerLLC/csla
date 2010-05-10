using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRoot : BusinessBase<DynamicRoot>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(new PropertyInfo<int>("Id"));
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //BusinessRules.AddRule(new Rule(), IdProperty);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    #endregion

    #region Factory Methods

    public static DynamicRoot NewDynamicRoot()
    {
      return DataPortal.Create<DynamicRoot>();
    }

    internal static DynamicRoot GetDynamicRoot(object rootData)
    {
      return new DynamicRoot(rootData);
    }

    private DynamicRoot()
    { /* Require use of factory methods */ }

    private DynamicRoot(object rootData)
    {
      Fetch(rootData);
    }

    #endregion

    #region Data Access

    private void Fetch(object rootData)
    {
	    MarkOld();
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