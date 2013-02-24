using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.ViewModelTests
{
  [Serializable]
  public class TestChild : BusinessBase<TestChild>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #endregion

    #region Factory Methods

    internal static TestChild NewEditableChild()
    {
      return Csla.DataPortal.CreateChild<TestChild>();
    }

    internal static TestChild GetEditableChild(object childData)
    {
      return Csla.DataPortal.FetchChild<TestChild>(childData);
    }

    public TestChild()
    { /* Require use of factory methods */ }

    #endregion

#if !SILVERLIGHT
    #region Data Access

    [RunLocal]
    protected override void Child_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.Child_Create();
    }

    [RunLocal]
    private void Child_Fetch(object childData)
    {
      // TODO: load values
    }

    [RunLocal]
    private void Child_Insert(object parent)
    {
      // TODO: insert values
    }

    [RunLocal]
    private void Child_Update(object parent)
    {
      // TODO: update values
    }

    [RunLocal]
    private void Child_DeleteSelf(object parent)
    {
      // TODO: delete values
    }

    #endregion
#endif
  }
}
