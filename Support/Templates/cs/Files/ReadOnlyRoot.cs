using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyRoot : ReadOnlyBase<ReadOnlyRoot>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods
    // use snippets to create your properties

    // example with private backing field
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id, RelationshipTypes.PrivateField);
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      private set { _Id = value); }
    }

    // example with managed backing field
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
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
