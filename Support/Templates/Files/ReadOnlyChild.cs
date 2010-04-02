using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyChild : ReadOnlyBase<ReadOnlyChild>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with managed backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    // example with private backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(new PropertyInfo<string>("Name", "Name"));
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
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
