using System;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootTests
{
  [Serializable]
  public partial class MockEditableRoot : BusinessBase<MockEditableRoot>
  {
    public static readonly Guid MockEditableRootId = new Guid("{7E7127CF-F22C-4903-BDCE-1714C81A9E89}");
    
    public MockEditableRoot() { }
    public MockEditableRoot(Guid id, bool isnew)
    {
      SetProperty<Guid>(IdProperty, id);
      if (!isnew)
        MarkOld();
    }

    #region  Properties

    private static PropertyInfo<Guid> IdProperty = RegisterProperty(
      typeof(MockEditableRoot), 
      new PropertyInfo<Guid>("Id"));

    private static PropertyInfo<string> NameProperty = RegisterProperty(
      typeof(MockEditableRoot), 
      new PropertyInfo<string>("Name"));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty(
      typeof(MockEditableRoot),
      new PropertyInfo<string>("DataPortalMethod"));

    public Guid Id
    {
      get { return GetProperty(IdProperty); }
    }
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
      set { SetProperty(NameProperty, ref _name, value); }
    }

    public string DataPortalMethod
    {
      get { return GetProperty(DataPortalMethodProperty); }
      set { SetProperty(DataPortalMethodProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    #endregion

    #region  Validation Rules

    //protected override void AddBusinessRules()
    //{
    //  // TODO: add business rules
    //}

    #endregion

    #region  Authorization Rules

    //protected override void AddAuthorizationRules()
    //{
    //  // add AuthorizationRules here
    //}

    //protected static void AddObjectAuthorizationRules()
    //{
    //  // add object-level authorization rules here
    //}

    #endregion

    #region  Factory Methods

    public static void CreateNew(EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginCreate<MockEditableRoot>(completed);
    }

    public static void Fetch(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginFetch<MockEditableRoot>(
        new SingleCriteria<MockEditableRoot, Guid>(id),
        completed);
    }

    public static void Delete(Guid id) { Delete(id, null); }
    public static void Delete(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginDelete<MockEditableRoot>(
        new SingleCriteria<MockEditableRoot, Guid>(id),
        completed);
    }

    #endregion

  }
}
