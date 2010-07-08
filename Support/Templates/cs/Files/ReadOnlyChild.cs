using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyChild : ReadOnlyBase<ReadOnlyChild>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods
    // use snippet cslapropg to create your properties

    // example with managed backing field
    private static readonly PropertyInfo<int> IdProperty = RegisterProperty(p => p.Id);
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

    internal static ReadOnlyChild GetReadOnlyChild(object childData)
    {
      return DataPortal.FetchChild<ReadOnlyChild>(childData);
    }

    private ReadOnlyChild()
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
