using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyRoot : ReadOnlyBase<ReadOnlyRoot>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods
    // use snippet cslapropg to create your properties

    // example with managed backing field
    private static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    // example with private backing field
    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyRoot GetReadOnlyRoot(int id)
    {
      return DataPortal.Fetch<ReadOnlyRoot>(id);
    }

    private ReadOnlyRoot()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(int criteria)
    {
      // TODO: load values
    }

    #endregion
  }
}
