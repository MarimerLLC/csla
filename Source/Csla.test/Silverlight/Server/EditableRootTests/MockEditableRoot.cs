//-----------------------------------------------------------------------
// <copyright file="MockEditableRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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

    private static PropertyInfo<Guid> IdProperty = RegisterProperty(
      typeof(MockEditableRoot), 
      new PropertyInfo<Guid>("Id"));

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(MockEditableRoot),
      new PropertyInfo<string>("Name", RelationshipTypes.PrivateField));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty<string>(
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

    public static void CreateNew(EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginCreate<MockEditableRoot>(completed);
    }

    public static void Fetch(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginFetch<MockEditableRoot>(id, completed);
    }

    public static void Delete(Guid id) { Delete(id, null); }
    public static void Delete(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      Csla.DataPortal.BeginDelete<MockEditableRoot>(id, completed);
    }
  }
}